using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Excel = Microsoft.Office.Interop.Excel;   //Microsoft Excel object in  References-> COM tab

namespace QLD
{
    class ExcelProcess
    {
        #region stringPath
        /// <summary>
        /// Đường dẫn tới tệp excel
        /// </summary>
        private string path;
        #endregion
        #region privateVar
        /// <summary>
        /// Số dòng
        /// </summary>
        int rowCount;
        /// <summary>
        /// Số cột
        /// </summary>
        int colCount;
        #endregion
        #region object
        /// <summary>
        /// Đối tượng
        /// </summary>
        private Excel.Application xlApp_Import = null;
        private Excel.Workbook xlWorkbook_Import = null;
        private Excel._Worksheet xlWorksheet_Import = null;
        private Excel.Range xlRange_Import = null;
        #endregion
        #region construct
        /// <summary>
        /// Khởi tạo chương trình đọc tệp excel
        /// </summary>
        /// <param name="path">Đường dẫn tới tệp excel</param>
        public ExcelProcess(string path)
        {
            this.path = path;
            Process();
        }
        #endregion
        #region privateFunction
        /// <summary>
        /// Khởi tạo không gian làm việc
        /// </summary>
        private void Process()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            xlApp_Import = new Excel.Application();
            xlWorkbook_Import = xlApp_Import.Workbooks.Open(path);
            xlWorksheet_Import = xlWorkbook_Import.Sheets[1];
            xlRange_Import = xlWorksheet_Import.UsedRange;
        }
        #endregion
        #region publicFunction
        #region readData
        /// <summary>
        /// Lấy toàn bộ dữ liệu từ tệp Excel
        /// </summary>
        /// <returns>Trả về một danh sách chứa tất cả các dòng trong tệp Excel</returns>
        public List<ThuaDat> ImportAllData()
        {
            List<ThuaDat> output = new List<ThuaDat>();
            List<string> head_row = new List<string>();
            List<object> row = null;


            rowCount = xlRange_Import.Rows.Count;
            colCount = xlRange_Import.Columns.Count;


            for (int i = 1; i <= colCount; i++)
            {
                head_row.Add(Convert.ToString(xlRange_Import.Cells[1, i].Value2));
            }

            for (int i = 2; i <= rowCount; i++)
            {
                row = new List<object>();
                for (int j = 1; j <= colCount; j++)
                {
                    row.Add(Convert.ToString(xlRange_Import.Cells[i, j].Value2));
                }
                output.Add(new ThuaDat(row, head_row));
            }
            return output;
        }
        #endregion

        #endregion
        #region staticFunction
        #region writeData
        /// <summary>
        /// Xuất tệp Excel từ một danh sách dữ liệu
        /// </summary>
        /// <param name="pathExport">Đường dẫn để lưu tệp Excel khi xuất</param>
        /// <param name="data"> Danh sách dữ liệu</param>
        /// <param name="column_name"> Danh sách tên của cột (Tuỳ chọn) - Để trống để lấy tên cột cùng tên với SQL</param>
        public static void ExportData(string pathExport, List<ThuaDat> data, ThuaDat column_name = null)
        {

            //Khởi tạo chương trình xữ lý
            Excel.Application xlApp_Export = new Excel.Application();

            //Khởi tạo các đối tượng cần thiết
            Excel.Workbook xlWorkBook_Export;
            Excel.Worksheet xlWorkSheet_Export;
            Excel.Range aRange_Export;
            object missValue = System.Reflection.Missing.Value;

            //Khởi tạo không gian làm việc
            xlWorkBook_Export = xlApp_Export.Workbooks.Add(missValue);
            xlWorkSheet_Export = (Excel.Worksheet)xlWorkBook_Export.Worksheets[1];


            //Bắt đầu nhập dữ liệu
            //Đặt tên cột
            if (column_name != null)
            {
                for (int i = 0; i < column_name.Columns.Count; i++)
                {
                    xlWorkSheet_Export.Cells[1, i + 1].NumberFormat = "@";
                    xlWorkSheet_Export.Cells[1, i + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    xlWorkSheet_Export.Cells[1, i + 1].Value = column_name.Columns[i];
                }
            }
            else
            {
                for (int i = 0; i < data[0].Columns.Count; i++)
                {
                    xlWorkSheet_Export.Cells[1, i + 1].NumberFormat = "@";
                    xlWorkSheet_Export.Cells[1, i + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    xlWorkSheet_Export.Cells[1, i + 1].Value = data[0].Columns[i];
                }
            }
            //Chèn dữ liệu vào 
            for (int i = 0; i < data.Count; i++)
            {
                xlWorkSheet_Export.Cells[i + 2, 1].NumberFormat = "@";
                xlWorkSheet_Export.Cells[i + 2, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                xlWorkSheet_Export.Cells[i + 2, 1].Value = Convert.ToString(i + 1);
                for (int j = 1; j < data[i].Count(); j++)
                {
                    xlWorkSheet_Export.Cells[i + 2, j + 1].NumberFormat = "@";
                    xlWorkSheet_Export.Cells[i + 2, j + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    xlWorkSheet_Export.Cells[i + 2, j + 1].Value = Convert.ToString(data[i][j]);
                }
            }
            //Autofit - Định dạng kích thước cột
            aRange_Export = xlWorkSheet_Export.UsedRange;
            aRange_Export.Columns.AutoFit();
            //Lưu lại thành tệp cần xuất
            xlWorkBook_Export.SaveAs(pathExport, Excel.XlFileFormat.xlWorkbookDefault, missValue, missValue, missValue, missValue, Excel.XlSaveAsAccessMode.xlExclusive, missValue, missValue, missValue, missValue, missValue);
            xlWorkBook_Export.Close(true, missValue, missValue);
            xlApp_Export.Quit();
            //Dọn dẹp dữ liệu chương trình
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //Giải phóng tài nguyên
            Marshal.ReleaseComObject(xlWorkSheet_Export);
            Marshal.ReleaseComObject(xlWorkBook_Export);
            Marshal.ReleaseComObject(xlApp_Export);
        }
        #endregion
        #endregion
        #region destruct
        /// <summary>
        /// Giải phóng tài nguyên
        /// </summary>
        ~ExcelProcess()
        {
            //clean
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //  rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange_Import);
            Marshal.ReleaseComObject(xlWorksheet_Import);

            try
            {
                //close and release
                xlWorkbook_Import.Close();
                Marshal.ReleaseComObject(xlWorkbook_Import);

                //quit and release
                xlApp_Import.Quit();
                Marshal.ReleaseComObject(xlApp_Import);
            }
            catch
            {
                //force quit
                Environment.Exit(1);
            }
        }
        #endregion
    }
}
