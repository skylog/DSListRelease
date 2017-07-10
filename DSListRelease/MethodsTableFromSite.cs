using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using System.Windows.Documents;

namespace DSList
{
    class MethodsTableFromSite
    {
        public List<Customer> GetAllCVZInList(DataTable curTable)
        {
            List<Customer> list = new List<Customer>();

            Customer newCLB;
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                newCLB = new Customer(null);
                newCLB.NumberCVZ = int.Parse(curTable.Rows[i][0].ToString());
                newCLB.City = curTable.Rows[i][1].ToString();
                newCLB.Address = curTable.Rows[i][2].ToString();
                newCLB.Lan_Ip = curTable.Rows[i][3].ToString();
                newCLB.WanIP = curTable.Rows[i][4].ToString();
                newCLB.JasperIP = curTable.Rows[i][5].ToString();
                newCLB.IPSECPass = curTable.Rows[i][6].ToString();


                if (!list.Contains(newCLB))
                {
                    list.Add(newCLB);
                }


            }
            return list;
        }


        /// <summary>
        /// Метод расширенного поиска по таблице DataTable и вывод DataTable со строками, в которых обнаружено совпадение с searchCVZ
        /// </summary>
        /// <param name="curTable">Таблица, в которой веде</param>
        /// <param name="searchCVZ">Переменная string, которую ищем</param>
        /// <returns></returns>
        public DataTable FindCVZExtended(DataTable curTable, string searchCVZ)
        {

            DataTable newTable = curTable.Clone();
            newTable.TableName = "Table_search_result";
            newTable.Clear();
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                foreach (string item in curTable.Rows[i].ItemArray)
                {
                    if (item.ToLower().Contains(searchCVZ.ToLower()))
                    {
                        DataRow newRow = newTable.NewRow();
                        newRow.ItemArray = curTable.Rows[i].ItemArray;
                        if (!newTable.Rows.Contains(newRow[0]))
                        {
                            newTable.Rows.Add(newRow);
                        }
                    }
                }
            }
            return newTable;
        }

        public List<Customer> FindCVZToListSimple(DataTable curTable, string searchCVZ)
        {
            List<Customer> list = new List<Customer>();

            Customer newCLB;
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                if (curTable.Rows[i][0].ToString().Contains(searchCVZ)/*int.Parse(row.ItemArray[0].ToString())==int.Parse(searchCVZ)*/)
                {
                    newCLB = new Customer(null);
                    newCLB.NumberCVZ = int.Parse(curTable.Rows[i][0].ToString());
                    newCLB.City = curTable.Rows[i][1].ToString();
                    newCLB.Address = curTable.Rows[i][2].ToString();
                    newCLB.Lan_Ip = curTable.Rows[i][3].ToString();
                    newCLB.WanIP = curTable.Rows[i][4].ToString();
                    newCLB.JasperIP = curTable.Rows[i][5].ToString();
                    newCLB.IPSECPass = curTable.Rows[i][6].ToString();


                    if (!list.Contains(newCLB))
                    {
                        list.Add(newCLB);
                    }

                }
            }
            return list;
        }
        public List<Customer> FindCVZToListExtended(DataTable curTable, string searchCVZ)
        {
            List<Customer> list = new List<Customer>();

            //DataTable newTable = curTable.Clone();
            //newTable.TableName = "Table_search_result";
            //newTable.Clear();
            Customer newCLB;
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                foreach (string item in curTable.Rows[i].ItemArray)
                {
                    if (item.ToLower().Contains(searchCVZ.ToLower()))
                    {
                        newCLB = new Customer(null);
                        newCLB.NumberCVZ = int.Parse(curTable.Rows[i][0].ToString());
                        newCLB.City = curTable.Rows[i][1].ToString();
                        newCLB.Address = curTable.Rows[i][2].ToString();
                        newCLB.Lan_Ip = curTable.Rows[i][3].ToString();
                        newCLB.WanIP = curTable.Rows[i][4].ToString();
                        newCLB.JasperIP = curTable.Rows[i][5].ToString();
                        newCLB.IPSECPass = curTable.Rows[i][6].ToString();


                        if (!list.Contains(newCLB))
                        {
                            list.Add(newCLB);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Метод поиска по таблице DataTable и вывод DataTable со строками, в которых обнаружено совпадение с searchCVZ
        /// </summary>
        /// <param name="curTable">Таблица, в которой веде</param>
        /// <param name="searchCVZ">Переменная string, которую ищем</param>
        /// <returns></returns>
        public DataTable FindCVZ(DataTable curTable, string searchCVZ)
        {

            DataTable newTable = curTable.Clone();
            newTable.TableName = "Table_search_result";
            newTable.Clear();
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                if ((curTable.Rows[i].ItemArray[0] as string).Contains(searchCVZ))
                {
                    DataRow newRow = newTable.NewRow();
                    newRow.ItemArray = curTable.Rows[i].ItemArray;
                    if (!newTable.Rows.Contains(newRow[0]))
                    {
                        newTable.Rows.Add(newRow);
                    }
                }
            }
            return newTable;
        }




        public Customer GetCVZFromSearchList(string selCustFromSearchList)
        {
            TableFromSite tfs = new TableFromSite();
            DataTable curTable = tfs.GetTableFromSite();
            Customer newCLB;
            for (int i = 0; i < curTable.Rows.Count; i++)
            {
                if (curTable.Rows[i][3].ToString().Contains(selCustFromSearchList)/*int.Parse(row.ItemArray[0].ToString())==int.Parse(searchCVZ)*/)
                {
                    newCLB = new Customer(null);
                    newCLB.NumberCVZ = int.Parse(curTable.Rows[i][0].ToString());
                    newCLB.City = curTable.Rows[i][1].ToString();
                    newCLB.Address = curTable.Rows[i][2].ToString();
                    newCLB.Lan_Ip = curTable.Rows[i][3].ToString();
                    newCLB.WanIP = curTable.Rows[i][4].ToString();
                    newCLB.JasperIP = curTable.Rows[i][5].ToString();
                    newCLB.IPSECPass = curTable.Rows[i][6].ToString();
                    return newCLB;
                }
            }
            return new Customer(null);
        }

    }
}
