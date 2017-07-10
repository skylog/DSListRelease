using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using AngleSharp;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Globalization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using TaskDialogInterop;
using System.Windows.Forms;
using System.Security;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace DSList
{
    public partial class NewMainWindow
    {

        private BackgroundWorker LoadDBBW = new BackgroundWorker();

        private DateTime _DBVersion;
        public DateTime DBVersion
        {
            get { return _DBVersion; }
            set { _DBVersion = value; }
        }

        /// <summary>
        /// Метод загрузки данных из БД SQL сервера(данная техника пока не реализована)
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected internal List<Customer> LoadDBFromSQLServer(string connectionString)
        {
            List<Customer> source = new List<Customer>();
            try
            {

                List<KeyValuePair<string, ProvInfo>> list2 = new List<KeyValuePair<string, ProvInfo>>();
                List<KeyValuePair<string, Employee>> list3 = new List<KeyValuePair<string, Employee>>();
                List<KeyValuePair<string, Host>> list4 = new List<KeyValuePair<string, Host>>();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    if (!connection.Ping())
                    {
                        connection.Open();
                    }

                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM settings limit 0,3000", connection))
                    {
                        if (!connection.Ping())
                        {
                            connection.Open();
                        }
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                Customer item = new Customer(this)
                                {
                                    //Owner = this,
                                    NumberCVZ = int.Parse(reader["cvz_id"].ToString()),
                                    City = reader["city"].ToString(),
                                    Address = reader["address"].ToString(),
                                    Organization = reader["org"].ToString(),
                                    Region = reader["region"].ToString(),
                                    Timezone = reader["timezone"].ToString(),
                                    Status = reader["cvz_state"].ToString(),
                                    Lan_Ip = reader["lan_ip"].ToString(),
                                    ConnectType = reader["conn_type"].ToString(),
                                    WanIP = reader["wan_ip"].ToString(),
                                    WanMask = reader["wan_mask"].ToString(),
                                    WanGW = reader["wan_gw"].ToString(),
                                    WanLogin = reader["wan_login"].ToString(),
                                    WanPass = reader["wan_pass"].ToString(),
                                    ICCID = reader["iccid"].ToString(),
                                    IPSECPass = reader["ipsec_pass"].ToString(),


                                    //Name = reader["Name"].ToString(),
                                    //FormatName = reader["FormatName"].ToString(),
                                    //Grade = reader["Grade"].ToString(),



                                    //WorkTime = reader["WorkTime"].ToString(),

                                    //DistrChannel = reader["DistrChannel"].ToString(),
                                    //Format = reader["Format"].ToString(),
                                    //Province = reader["Province"].ToString(),

                                    //Phone = reader["Phone"].ToString(),
                                    //RD = reader["RD"].ToString(),
                                    //RDPhone = reader["RDPhone"].ToString(),
                                    //MRD = reader["MRD"].ToString(),
                                    //MRDPhone = reader["MRDPhone"].ToString(),
                                    //UM = reader["UM"].ToString(),
                                    //UMPhone = reader["UMPhone"].ToString(),

                                    //SubnetMask = reader["SubnetMask"].ToString(),
                                    //Gateway = reader["Gateway"].ToString(),
                                    //Router = reader["Router"].ToString(),
                                    //KKT = reader["KKT"].ToString(),
                                    //Login = reader["Login"].ToString(),
                                    //Email = reader["Email"].ToString()
                                };
                                source.Add(item);
                            }
                        }
                    }
                    //using (SqlCommand command2 = new SqlCommand("SELECT * FROM ProvInfo", connection))
                    //{
                    //    using (SqlDataReader reader2 = command2.ExecuteReader())
                    //    {
                    //        while (reader2.Read())
                    //        {
                    //            ProvInfo info = new ProvInfo
                    //            {
                    //                ExtIP = reader2["ExtIP"].ToString(),
                    //                ExtSM = reader2["ExtSM"].ToString(),
                    //                ExtGW = reader2["ExtGW"].ToString(),
                    //                ReserveIP = reader2["ReserveIP"].ToString(),
                    //                ProvName = reader2["ProvName"].ToString(),
                    //                ProvPhone = reader2["ProvPhone"].ToString(),
                    //                ContractNum = reader2["ContractNum"].ToString(),
                    //                ChID = reader2["ChID"].ToString(),
                    //                Rate = reader2["Rate"].ToString(),
                    //                Login = reader2["Login"].ToString(),
                    //                Password = reader2["Password"].ToString(),
                    //                ManagerName = reader2["ManagerName"].ToString(),
                    //                ManagerPhone = reader2["ManagerPhone"].ToString(),
                    //                ManagerEmail = reader2["ManagerEmail"].ToString(),
                    //                PhoneLine = reader2["PhoneLine"].ToString(),
                    //                Type = reader2["Type"].ToString(),
                    //                VPIVCI = reader2["VPIVCI"].ToString(),
                    //                Comment = reader2["Comment"].ToString(),
                    //                WifiProv = reader2["WifiProv"].ToString(),
                    //                WifiProfile = reader2["WifiProfile"].ToString()
                    //            };
                    //            list2.Add(new KeyValuePair<string, ProvInfo>(reader2["TTCode"].ToString(), info));
                    //        }
                    //    }
                    //}
                    //using (SqlCommand command3 = new SqlCommand("SELECT * FROM Employee", connection))
                    //{
                    //    using (SqlDataReader reader3 = command3.ExecuteReader())
                    //    {
                    //        while (reader3.Read())
                    //        {
                    //            Employee employee = new Employee
                    //            {
                    //                Code = reader3["EmployeeId"].ToString(),
                    //                Name = reader3["Name"].ToString(),
                    //                Position = reader3["Position"].ToString()
                    //            };
                    //            list3.Add(new KeyValuePair<string, Employee>(reader3["TTCode"].ToString(), employee));
                    //        }
                    //    }
                    //}
                    //using (SqlCommand command4 = new SqlCommand("SELECT * FROM Host", connection))
                    //{
                    //    using (SqlDataReader reader4 = command4.ExecuteReader())
                    //    {
                    //        while (reader4.Read())
                    //        {
                    //            Host host = new Host
                    //            {
                    //                NetbiosName = reader4["NetbiosName"].ToString(),
                    //                HostType = reader4["HostType"].ToString(),
                    //                IPAddress = reader4["IPAddress"].ToString(),
                    //                TOID = reader4["TOID"].ToString(),
                    //                Model = reader4["Model"].ToString(),
                    //                Serial = reader4["Serial"].ToString(),
                    //                Vendor = reader4["Vendor"].ToString(),
                    //                MBName = reader4["MBName"].ToString(),
                    //                MBSerial = reader4["MBSerial"].ToString(),
                    //                CPUName = reader4["CPUName"].ToString(),
                    //                CPUFreq = reader4["CPUFreq"].ToString(),
                    //                HDDName = reader4["HDDName"].ToString(),
                    //                HDDSize = reader4["HDDSize"].ToString(),
                    //                RamSize = reader4["RamSize"].ToString(),
                    //                HDDSmartStatus = reader4["HDDSmartStatus"].ToString(),
                    //                LastHWScan = reader4["LastHWScan"].ToString()
                    //            };
                    //            list4.Add(new KeyValuePair<string, Host>(reader4["TTCode"].ToString(), host));
                    //        }
                    //    }
                    //}
                    //this.DBVersion = DateTime.Parse(LoadSetting(connection, "DatabaseUpdated", null)).ToLocalTime();
                    connection.Close();
                }
                //ILookup<string, ProvInfo> lookup = list2.ToLookup<KeyValuePair<string, ProvInfo>, string, ProvInfo>(x => x.Key, x => x.Value);
                //ILookup<string, Employee> lookup2 = list3.ToLookup<KeyValuePair<string, Employee>, string, Employee>(x => x.Key, x => x.Value);
                //ILookup<string, Host> lookup3 = list4.ToLookup<KeyValuePair<string, Host>, string, Host>(x => x.Key, x => x.Value);
                //foreach (Customer customer2 in source)
                //{
                //    customer2.Providers.AddRange(lookup[customer2.NumberCVZ.ToString()]);
                //    customer2.Employees.AddRange(lookup2[customer2.NumberCVZ.ToString()]);
                //    customer2.Hosts.AddRange(lookup3[customer2.NumberCVZ.ToString()]);
                //}
                AllTT = source.AsEnumerable<Customer>();

            }
            catch (Exception ex)
            {
                //this.Error(ex.Message, "Ошибка при загрузке базы ТТ", ex.StackTrace);
                Log("Ошибка при загрузке базы ТТ. " + ex.Message, true, true, ex.StackTrace);
                return null;
            }

            return source;
        }


        private void LoadDBBW_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Log("Загрузка базы ТТ...", false, false, string.Empty, true);
            try
            {
                this.LoadDBFromSQLServer(settings.Fields.ConnectionString);
                if (this.LoadCredentialsFromSQLServer(settings.Fields.ConnectionString))
                {
                    this.SelectedCredentials.Logins = LoginsViewModel.StringArrayToCollectionView((from s in Credentials select s.Login).Distinct<string>().ToArray<string>());
                    //this.PasswordsMenu = this.EnumPasswordsMenu();
                }
                this.macvendors = System.IO.File.ReadAllLines(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\mac.dat"));
                //if (System.IO.File.Exists(pilot_sbr_file))
                //{
                //    pilot_sbr = System.IO.File.ReadAllLines(pilot_sbr_file);
                //}
                //if (System.IO.File.Exists(pilot_flora_file))
                //{
                //    pilot_flora = System.IO.File.ReadAllLines(pilot_flora_file);
                //}
                //if (System.IO.File.Exists(pilot_bio_file))
                //{
                //    pilot_bio = System.IO.File.ReadAllLines(pilot_bio_file);
                //}
                //if (System.IO.File.Exists(pilot_zlock_file))
                //{
                //    pilot_zlock = System.IO.File.ReadAllLines(pilot_zlock_file);
                //}
                this.Log("Загружена БД от " + this.DBVersion.ToString(), false, true, "", false);
                Bindings.dbversion = this.DBVersion.ToString();
                this.SendLogs();
                //this.InitFilteredTT();
            }
            catch (Exception exception)
            {
                this.Error(exception.Message, "Ошибка при загрузке БД", exception.StackTrace);
            }
        }


        private static string LoadSetting(SqlConnection sqlConnection, string key, string defaultValue = null)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = $"SELECT TOP 1 Value FROM Settings WHERE Setting = '{key}'";
                command.CommandType = CommandType.Text;
                command.Connection = sqlConnection;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        return reader[0].ToString();
                    }
                }
            }
            return defaultValue;
        }


    }
}
