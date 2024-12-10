using Factory.DB.Model;
using Serilog;

namespace Factory.DB.Init
{
    public class InitDB
    {
        public static int Init()
        {
            try
            {
                //Create database - required for prod
                using (var dbContext = new DBContext())
                {
                    _ = dbContext.CreateDatabaseAsync("sqlitedb").Result;
                }
            }
            catch (Exception ex)
            {
                //do nothng    
            }

            try
            {              

                using (var dbContext = new DBContext())
                {

                    var result = new List<Task>();
                    var query = dbContext.QueryFactory.CreateTable(typeof(ModTableXls));
                    result.Add(dbContext.ExecuteNonQueryAsync(query));                 
                   
                    Task.WaitAll(result.ToArray());

                    Log.Debug("Init DB done");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Init " + ex.Message);
                return 0;
            }
        }
    }
}
