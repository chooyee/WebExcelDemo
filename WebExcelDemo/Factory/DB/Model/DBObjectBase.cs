using Serilog;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Factory.DB.Model
{

    public interface IDBObjectBase
    {       
        int Id { get; set; }
    }

    public abstract class DBObjectBase : IDBObjectBase, IDisposable
    {
        private bool disposedValue;

        public DBContext dbContext { get; set; }

        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        public DBObjectBase()
        {
            if (dbContext == null) dbContext = new DBContext();
        }

        public DBObjectBase(DBContext dbContext)
        {
            this.dbContext = dbContext ?? new DBContext();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.dbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DBObjectBase()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public static class DBObjectBaseExt
    {
        private static Dictionary<string, object> GetPrimaryKey<T>(T thisObj, IEnumerable<PropertyInfo> propInfos)
        {
            var primaryKey = new Dictionary<string, object>();

            foreach (var prop in propInfos)
            {
                var paramVal = prop.GetValue(thisObj);
                if (paramVal != null)
                {
                    var customAttribute = prop.GetCustomAttribute<SqlPropertyAttribute>();
                    if (customAttribute == null) continue;
                    var colName = customAttribute.PropertyName ?? prop.Name;
                    if (prop.GetCustomAttribute<SqlPrimaryKey>() != null)
                    {
                        primaryKey.Add(colName, paramVal);
                        break;
                    }
                }
            }

            return primaryKey;
        }

        public static async Task<IList> LoadAll(this DBObjectBase thisObj)
        {
            Type objType = thisObj.GetType();
            var constructor = Expression.Lambda<Func<object>>(Expression.New(objType)).Compile();

            Type listType = typeof(List<>).MakeGenericType(new[] { objType });
            IList resultList = (IList)Activator.CreateInstance(listType);

            var sqlParam = new DynamicSqlParameter();

            var propInfos = ReflectionFactory.GetMappableProperties(objType);
            var tableName = ReflectionFactory.GetTableAttribute(objType);


            var primaryKey = GetPrimaryKey(thisObj, propInfos).First();

            sqlParam.Add("@primaryKey", primaryKey.Value);
            var query = $"select * from {tableName}";

            try
            {
                using (var dbContext = thisObj.dbContext)
                {
                    var dataTable = await dbContext.ExecuteReaderAsync(query, sqlParam);

                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {                            
                            // Invoke the delegate to create an instance
                            var newObj = constructor();
                            foreach (var prop in propInfos)
                            {

                                var customAttribute = prop.GetCustomAttribute<SqlPropertyAttribute>();
                                if (customAttribute == null) continue;
                                var colName = customAttribute.PropertyName ?? prop.Name;
                                if (dataRow[colName] == DBNull.Value) continue;


                                if (prop.PropertyType == typeof(DateTime?))
                                {
                                    prop.SetValue(newObj, dataRow[colName], null);
                                }
                                else
                                {
                                    var val = dataRow[colName].GetType() == prop.PropertyType ? dataRow[colName] : Convert.ChangeType(dataRow[colName], prop.PropertyType);
                                    prop.SetValue(newObj, val, null);
                                }
                            }//loop properties

                            resultList.Add(newObj);
                        }                       
                    }
                    return resultList;
                }

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

       

        public static async Task<bool> Load(this DBObjectBase thisObj)
        {
            var objType = thisObj.GetType();
            var sqlParam = new DynamicSqlParameter();

            var propInfos = ReflectionFactory.GetMappableProperties(objType);
            var tableName = ReflectionFactory.GetTableAttribute(objType);

           
            var primaryKey = GetPrimaryKey(thisObj, propInfos).First();

            sqlParam.Add("@primaryKey", primaryKey.Value);
            var query = $"select * from {tableName}  where {primaryKey.Key}=@primaryKey";
            Log.Debug(query);
           
            try
            {
                using (var dbContext = thisObj.dbContext)
                {
                    var dataTable = await dbContext.ExecuteReaderAsync(query, sqlParam);

                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            foreach (var prop in propInfos)
                            {

                                var customAttribute = prop.GetCustomAttribute<SqlPropertyAttribute>();
                                if (customAttribute == null) continue;
                                var colName = customAttribute.PropertyName ?? prop.Name;
                                if (dataRow[colName] == DBNull.Value) continue;


                                if (prop.PropertyType == typeof(DateTime?))
                                {
                                    prop.SetValue(thisObj, dataRow[colName], null);
                                }
                                else
                                {
                                    var val = dataRow[colName].GetType() == prop.PropertyType ? dataRow[colName] : Convert.ChangeType(dataRow[colName], prop.PropertyType);
                                    prop.SetValue(thisObj, val, null);
                                }

                            }

                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                  
                }
               
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public static async Task<int> Save(this DBObjectBase thisObj, bool autoUpdate = true)
        {
            var objType = thisObj.GetType();
            var result = 0;
            var sqlParam = new DynamicSqlParameter();

            var propInfos = ReflectionFactory.GetMappableProperties(objType);
            var tableName = ReflectionFactory.GetTableAttribute(objType);
            var primaryKey = GetPrimaryKey(thisObj, propInfos).First();

            sqlParam.Add("@primaryKey", primaryKey.Value);

            var query = $"select count(*) from {tableName}  where {primaryKey.Key}=@primaryKey";

            try
            {
                using (var dbContext = thisObj.dbContext)
                {
                    var count = (int) Convert.ChangeType(await dbContext.ExecuteScalarAsync(query, sqlParam), typeof(int));

                    if (count > 0)
                    {
                        if (autoUpdate)
                        {
                            //Update
                            var updateQuery = dbContext.QueryFactory.Update(thisObj);
                            result = await dbContext.ExecuteNonQueryAsync(updateQuery.Item1, updateQuery.Item2);
                        }
                        //throw new Exception($"{primaryKey.Key} with values {primaryKey.Value} already exists in {tableName}!");
                    }
                    else
                    {
                        //insert
                        var insertQuery = dbContext.QueryFactory.Insert(thisObj);
                        result = await dbContext.ExecuteNonQueryAsync(insertQuery.Item1, insertQuery.Item2);
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

       
    }
}
