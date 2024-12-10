using Factory.DB.Model;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Collections;

namespace WebExcelDemo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class XlsController : ControllerBase
    {
        public XlsController() { }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return "Test";
        }

        [HttpGet, Route("all")]
        public async Task<ActionResult<string>> GetAllXls()
        {
            var xlsObj = new ModTableXls();
            return JsonConvert.SerializeObject(await GetAll(xlsObj));

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<string>>> GetXlsSheet(int id)
        {
            try
            {
                using (var xlsObj = await GetXlsObj(id))
                {
                    return Factory.XlsFactory.GetXlsSheets(xlsObj.FilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost("search")]
        public async Task<ActionResult<List<XCell>>> SearchXlsSheet([FromForm] int id, [FromForm] string sheet, [FromForm] string searchText, [FromForm] string? colEndText)
        {
            var result = new List<XCell>();
            try
            {
                using (var xlsObj = await GetXlsObj(id))
                {
                    result = Factory.XlsFactory.ReadTableCol(xlsObj.FilePath, sheet, searchText, colEndText);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost("write")]
        public async Task<ActionResult<bool>> WriteCell([FromForm] int id, [FromForm] string sheet, [FromForm] int row, [FromForm] int col, [FromForm] string text)
        {
            var result = false;
            try
            {
                using (var xlsObj = await GetXlsObj(id))
                {
                    result = Factory.XlsFactory.WriteTableCell(xlsObj.FilePath, sheet, row, col, text);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost("column")]
        public async Task<ActionResult<bool>> InsertColumn([FromForm] int id, [FromForm] string sheet, [FromForm] int targetCol, [FromForm] int numOfCol, [FromForm] Factory.XlsFactory.EnumPosition insertPosition)
        {
            var result = false;
            try
            {
                using (var xlsObj = await GetXlsObj(id))
                {
                    result = Factory.XlsFactory.InsertTableCol(xlsObj.FilePath, sheet, targetCol,numOfCol, insertPosition);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPost("row")]
        public async Task<ActionResult<bool>> InsertRow([FromForm] int id, [FromForm] string sheet, [FromForm] int targetRow, [FromForm] int numOfRow, [FromForm] Factory.XlsFactory.EnumPosition insertPosition)
        {
            var result = false;
            try
            {
                using (var xlsObj = await GetXlsObj(id))
                {
                    result = Factory.XlsFactory.InsertTableRow(xlsObj.FilePath, sheet, targetRow, numOfRow, insertPosition);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var xlsObj = await GetXlsObj(id);
            var memory = new MemoryStream();
            using (var stream = new FileStream(xlsObj.FilePath, FileMode.Open, FileAccess.Read))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            var filename = Path.GetFileName(xlsObj.FilePath);
            return File(memory, "application/xlsx", filename);
        }
        
        private async Task<ModTableXls> GetXlsObj(int id)
        {
            var xlsObj = new ModTableXls();
            xlsObj.Id = id;
            await xlsObj.Load();
            return xlsObj;

        }
        private async Task<IList> GetAll<T>(T obj)
        {
            if (obj is not IDBObjectBase)
            {
                throw new ArgumentException(nameof(obj) + " is invalid object type!");
            }
            var dbObj = obj as DBObjectBase;
            return await dbObj.LoadAll();
        }
    }
}
