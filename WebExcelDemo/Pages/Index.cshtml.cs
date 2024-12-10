using Factory.DB.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using static Factory.XlsFactory;

namespace WebExcelDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public Dictionary<string, int> xlsList { get; set; }


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            xlsList = new Dictionary<string, int>();
            var xlsLog = new Factory.DB.Model.ModTableXls();
            var ilistResult = await xlsLog.LoadAll();
            foreach (ModTableXls item in ilistResult)
            {
                xlsList.Add(item.AliasFileName, item.Id);
            }
        }

        public void OnPost(IFormFile xlsFile)
        {
            string fileName = string.Empty;
            var errInvalidXlsMsg = "Invalid excel file format!";

            var dir = AppDomain.CurrentDomain.BaseDirectory + "temp\\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                var ext = Path.GetExtension(xlsFile.FileName);

                bool validExt = Enum.GetNames(typeof(EnumXlsExt)).Any(f => f == ext.Substring(1));
                if (!validExt) throw new Exception(errInvalidXlsMsg);

                fileName = Path.Combine(dir,Path.GetRandomFileName() + ext);

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    xlsFile.CopyToAsync(stream).Wait();
                }

                var sheets = Factory.XlsFactory.GetXlsSheets(fileName);

                var xlsLog = new Factory.DB.Model.ModTableXls()
                {
                    AliasFileName  = xlsFile.FileName,
                    FilePath = fileName,
                };
                xlsLog.Save().Wait();

                Response.Redirect("/");
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);
                Response.Redirect($"/?error={ex.Message}");
            }
        }
    }
}
