using ClosedXML.Excel;
using Models;

namespace Factory
{
    public class XlsFactory
    {
        public enum EnumXlsExt
        {
            xls,
            xlsx
        }

        public enum EnumPosition
        {
            before,
            after,
            above,
            below
        }

        public static List<string> GetXlsSheets(string filename)
        {
            List<string> sheetNames = new List<string>();
            try
            {
                
                using (var wb = new XLWorkbook(filename))
                {
                    sheetNames = wb.Worksheets.Select(x=>x.Name).ToList();
                }

                return sheetNames;
            }
            catch (Exception ex) {
                throw new Exception($"Failed to read excel!");
            }
        }

        public static List<XCell> ReadTableCol(string filename, string selectedSheet, string searchText, string columnEndText="")
        {
            var tableColumns = new List<XCell>();
            var found = false;
            using (var wb = new XLWorkbook(filename))
            {
                var ws = wb.Worksheet(selectedSheet);
                var rowCount = ws.LastRowUsed().RowNumber();
                var columnCount = ws.LastColumnUsed().ColumnNumber();
        
                for (int row =1; row<=rowCount;row++)
                {

                    // do something with entry.Value or entry.Key
                    for (int col = 1; col<= columnCount; col++)
                    {
                        var cellVal = ws.Cell(row, col).GetString();                       
                        if (cellVal.Equals(searchText, StringComparison.OrdinalIgnoreCase))
                        {
                            System.Diagnostics.Debug.WriteLine(cellVal);
                            System.Diagnostics.Debug.WriteLine($"{row} : {col}");
                            found = true;
                        }
                                               
                        if (found)
                        {
                            tableColumns.Add(new XCell(cellVal, row, col));
                        }

                        if ((!string.IsNullOrEmpty(columnEndText)) && (cellVal.Equals(columnEndText, StringComparison.OrdinalIgnoreCase))) break;


                    }//end foreach (KeyValuePair<int, SLCell> y in x.Value)

                    if (found) break;
                   
                }//end foreach (KeyValuePair<int, Dictionary<int, SLCell>> x in sl.GetCells())

            }//end using (SLDocument sl = new SLDocument(filename, selectedSheet))

            return tableColumns;
        }

        public static bool WriteTableCell(string filename, string selectedSheet, int row, int column, string text)
        {
            try
            {
                using (var wb = new XLWorkbook(filename))
                {
                    var ws = wb.Worksheet(selectedSheet);
                    ws.Cell(row, column).Value = text;                    
                    wb.Save();

                }//end using (SLDocument sl = new SLDocument(filename, selectedSheet))
                return true;
            }
            catch(Exception ex) {
                throw;
            }            
        }



        /// <summary>
        /// Insert table column 
        /// *** Need closedXml version 0.103 to work *** DO NOT UPDATE TO 0.104
        /// https://github.com/ClosedXML/ClosedXML/issues/2425
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="selectedSheet"></param>
        /// <param name="targetCol"></param>
        /// <param name="numOfCol"></param>
        /// <param name="insertPosition"></param>
        /// <returns></returns>
        public static bool InsertTableCol(string filename, string selectedSheet, int targetCol, int numOfCol, EnumPosition insertPosition)
        {
            try
            {
                using (var wb = new XLWorkbook(filename))
                {
                    var ws = wb.Worksheet(selectedSheet);
                    var target = ws.Column(targetCol);

                    switch (insertPosition)
                    {
                        case EnumPosition.before:
                            target.InsertColumnsBefore(numOfCol);
                            break;
                        default:
                            target.InsertColumnsAfter(numOfCol);
                            break;
                    }                   
                    wb.Save();

                }//end using (SLDocument sl = new SLDocument(filename, selectedSheet))
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Insert Row to table
        /// *** Need closedXml version 0.103 to work *** DO NOT UPDATE TO 0.104
        /// https://github.com/ClosedXML/ClosedXML/issues/2425
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="selectedSheet"></param>
        /// <param name="targetRow"></param>
        /// <param name="numOfRow"></param>
        /// <param name="insertPosition"></param>
        /// <returns></returns>
        public static bool InsertTableRow(string filename, string selectedSheet, int targetRow, int numOfRow, EnumPosition insertPosition)
        {
            try
            {
                using (var wb = new XLWorkbook(filename))
                {
                    var ws = wb.Worksheet(selectedSheet);
                    var target = ws.Row(targetRow);

                    switch (insertPosition)
                    {
                        case EnumPosition.above:
                            target.InsertRowsAbove(targetRow);
                            break;
                        default:
                            target.InsertRowsBelow(targetRow);
                            break;
                    }
                    wb.Save();

                }//end using (SLDocument sl = new SLDocument(filename, selectedSheet))
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
