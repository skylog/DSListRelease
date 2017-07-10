using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;

namespace DSList
{
    class ExcelParsing
    {
        private string _Path;

        public DataTable DTFromXLSX { get; set; }
        public string Path
        {
            get { return _Path; }
            set { _Path = value; }
        }
        public ExcelParsing()
        {
            DTFromXLSX = new DataTable();
            Path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"ТАБЛИЦА  ПЛАТЕЖЕЙ  2017.xlsx");
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="fileXLSXName">Имя файла XLSX c провайдерами от Посиделовой, который находится в папке с запускающим файлом</param>
        public ExcelParsing(string fileXLSXName)
        {
            DTFromXLSX = new DataTable();
            Path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, fileXLSXName); 
        }

        public void ParseXLSX()
        {
            var openXls = new Microsoft.Office.Interop.Excel.Application();
            //var openXlsWb = openXls.Workbooks.Open(Path,
            //    Type.Missing, Type.Missing,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0

            var openXlsWb = openXls.Workbooks.Open(Path,Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Excel.Worksheet openXlsWs = (Excel.Worksheet)openXlsWb.Sheets[1];
            var lastCell = openXlsWs.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);

            for (int comNum = 0; comNum < lastCell.Column; comNum++)
            {
                DTFromXLSX.Columns.Add(comNum.ToString());
            }
            DataRow stringData = DTFromXLSX.NewRow();


            for (int i = 1; i < lastCell.Row; i++)
            {
                stringData = DTFromXLSX.NewRow();
                for (int j = 0; j < lastCell.Column; j++)
                {
                    stringData[j] = openXlsWs.Cells[i + 1, j + 1].Text.ToString();
                }
                DTFromXLSX.Rows.Add(stringData);
            }
        }
    }
}
