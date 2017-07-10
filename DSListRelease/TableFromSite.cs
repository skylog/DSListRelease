using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;

namespace DSList
{
    /// <summary>
    /// Класс формирования DataTable из данных сайта
    /// </summary>
    public class TableFromSite
    {
        #region Поля

        Url siteParse;
        DataTable tableCVZ;
        string classNameTable;
        IHtmlDocument document;

        #endregion
        
        #region Свойства

        /// <summary>
        /// Сайт, разобранный на структуры
        /// </summary>
        public IHtmlDocument DocumentHtml { get { return document; } }

        /// <summary>
        ///Свойство, адрес сайта, который будет парсится
        /// </summary>
        public Url SiteParse
        {
            get { return siteParse; }
            set { siteParse = value; }
        }

        /// <summary>
        /// Значение аттрибута таблицы class, для определения таблицы сайта, которую необходимо переводить в тип DataTable
        /// </summary>
        /// /// <example>данные с сайта: table class="view_table" указывается view_table</example>
        public string ClassNameTable
        {
            get { return classNameTable; }
            set { classNameTable = value; }
        }

        #endregion


        /// <summary>
        /// Класс формирует таблицу типа DataTable из таблицы сайта
        /// </summary>
        public TableFromSite()
        {
            siteParse = new Url(Path.Combine(Application.StartupPath, "site.html"));
            ClassNameTable = "view_table";
            CreateDocumentFromSite();
        }

        /// <summary>
        /// Класс формирует таблицу типа DataTable из таблицы сайта
        /// </summary>
        /// <param name="siteParse">Адрес сайта для преобразования в таблицу</param>
        public TableFromSite(Url curSiteParse)
        {
            siteParse = curSiteParse;
            ClassNameTable = "view_table";
            CreateDocumentFromSite();
        }

        /// <summary>
        /// Класс формирует таблицу типа DataTable из таблицы сайта
        /// </summary>
        /// <param name="siteParse">Адрес сайта для преобразования в таблицу</param>
        /// <param name="classNameTable">Значение аттрибута таблицы class, для определения таблицы сайта, которую необходимо переводить в тип DataTable</param>
        /// <example>table class="view_table"</example>
        public TableFromSite(Url curSiteParse, string curClassNameTable)
        {
            siteParse = curSiteParse;
            ClassNameTable = curClassNameTable;
            CreateDocumentFromSite();
        }

        public Dictionary<string, string> CreateDictFromSite()
        {
            Dictionary<string, string> CustomerMikroTikDict = new Dictionary<string, string>();
            foreach (var table in document.GetElementsByTagName("Table"))
            {
                foreach (var attr in table.Attributes)
                {
                    if (attr.Value == classNameTable)

                        foreach (var node in table.GetElementsByTagName("TBODY"))
                            #region Формирование словаря с данными
                            foreach (var str in node.GetElementsByTagName("TR"))
                            {
                                string[] dictFieldValue = new string[2];
                                int curColRow = 0;
                                //if (str.Attributes.Count() > 0)
                                //{
                                // Формирование строк
                                foreach (var curRowItem in str.GetElementsByTagName("TD"))
                                {
                                    dictFieldValue[curColRow] = curRowItem.TextContent;
                                    curColRow++;
                                }
                                

                                if (dictFieldValue[0]!=null&&dictFieldValue[1]!=null)
                                    CustomerMikroTikDict.Add(dictFieldValue[0], dictFieldValue[1]);
                                //}
                            }
                    #endregion
                }
            }
            return CustomerMikroTikDict;
        }

        /// <summary>
        /// Наполняет document содержимым из сайта
        /// </summary>
        void CreateDocumentFromSite()
        {
            var request = WebRequest.Create(siteParse);

            using (var responses = request.GetResponse())
            {
                using (var streams = responses.GetResponseStream())
                {
                    using (var readers = new StreamReader(streams))
                    {
                        var parser = new HtmlParser();
                        string source = readers.ReadToEnd();
                        document = parser.Parse(source);
                    }
                }
            }
        }

        /// <summary>
        /// Метод формирования таблицы
        /// </summary>
        /// <returns>Возвращает таблицу DataTable</returns>
        public DataTable GetTableFromSite()
        {
            foreach (var table in document.GetElementsByTagName("Table"))
            {
                foreach (var attr in table.Attributes)
                {
                    if (attr.Value == classNameTable)
                    {
                        foreach (var node in table.GetElementsByTagName("TBODY"))
                        {
                            tableCVZ = new DataTable();
                            tableCVZ.TableName = "Table_with_ttcvz";
                            DataColumn newCol;
                            DataRow newRow;

                            #region Формирование таблицы с данными
                            foreach (var str in node.GetElementsByTagName("TR"))
                            {
                                newRow = tableCVZ.NewRow();
                                int curColRow = 0;

                                if (str.Attributes.Count() == 0)
                                {
                                    // Формирование колонок
                                    foreach (var curCol in str.GetElementsByTagName("TH"))
                                    {
                                        newCol = new DataColumn();
                                        newCol.ColumnName = curCol.TextContent;
                                        newCol.DataType = typeof(string);
                                        tableCVZ.Columns.Add(newCol);
                                    }
                                }

                                if (str.Attributes.Count() > 0)
                                {
                                    // Формирование строк
                                    foreach (var curRowItem in str.GetElementsByTagName("TD"))
                                    {
                                        newRow[curColRow] = curRowItem.TextContent;
                                        curColRow++;
                                    }
                                    tableCVZ.Rows.Add(newRow);
                                }
                            }
                            DataColumn[] primaryColumns = new DataColumn[1];
                            primaryColumns[0] = tableCVZ.Columns[0];
                            tableCVZ.PrimaryKey = primaryColumns;

                            #endregion
                        }
                    }
                }
            }
            return tableCVZ;
        }
    }
}
