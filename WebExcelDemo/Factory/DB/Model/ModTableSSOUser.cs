
namespace Factory.DB.Model
{

   

    [SqlTable("tblxls")]
    public class ModTableXls : DBObjectBase
    {
        
        [SqlProperty("aliasfilename", DataType.TEXT)]
        public string? AliasFileName { get; set; }

        [SqlProperty("FilePath", DataType.TEXT)]
        public string? FilePath { get; set; }


        [SqlProperty("logDate", DataType.DATETIME)]
        public string LogDate { get; set; }

        public ModTableXls():base() {
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }

        public ModTableXls(DBContext dbContext) : base(dbContext)
        {
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
    }
    
}
