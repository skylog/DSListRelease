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

namespace DSList
{
    public partial class NewMainWindow
    {
        #region Поля

        private static string Credentials_xmlfile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DSList\Credentials.xml");


        #endregion

        #region Свойства

        /// <summary>
        /// Свойство, представляющее список удостоверений личности Credentials, которые пользователь сам заполняет в SettingsWindow, данной коллекцией заполняется ListViewCredentials
        /// </summary>
        private static ObservableCollection<Credential> UserCredentials { get; set; }

        /// <summary>
        /// Свойство, представляющее список удостоверений личности Credentials в List-Credential
        /// </summary>
        protected internal static List<Credential> Credentials { get; set; }

        /// <summary>
        /// Свойство, представляющее отобранные удостоверения личности, представленные для отображения в GUI
        /// </summary>
        public LoginsViewModel SelectedCredentials { get; set; }

        #endregion

        #region Блок методов, отвечающих за Credentials

        /// <summary>
        /// Метод сохранения удостоверений личности в файл на основе Credentials_xmlfile
        /// </summary>
        public void SaveUserCredentials()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Credential>));
                StreamWriter writer = new StreamWriter(Credentials_xmlfile);
                serializer.Serialize((TextWriter)writer, UserCredentials);
                writer.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Метод загрузки удостоверений личности из файла на основе Credentials_xmlfile
        /// </summary>
        public void LoadUserCredentials()
        {

            try
            {
                if (System.IO.File.Exists(Credentials_xmlfile))
                {
                    using (StreamReader reader = new StreamReader(Credentials_xmlfile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Credential>));
                        UserCredentials = serializer.Deserialize(reader) as ObservableCollection<Credential>;
                    }
                }
                else
                {
                    UserCredentials = new ObservableCollection<Credential>();
                }
            }
            catch (Exception ex)
            {
                //this.Error(exception.Message, "Ошибка при загрузке паролей", exception.StackTrace);
                Log("Ошибка при загрузке паролей. Создана пустая коллекция паролей." + ex.Message, true, true, ex.StackTrace);
                UserCredentials = new ObservableCollection<Credential>();
            }


        }

        public void SelectedLoginCreate()
        {
            this.SelectedCredentials = new LoginsViewModel() { Owner = this, };
        }

        /// <summary>
        /// Временный метод создания удостоверений личности, данный метод является временным, эти данные должны загружаться с сервера и (сохраняться в файл ?)
        /// </summary>
        private void CreateNewCredentials()
        {
            Credentials = new List<Credential>();
            //ObservableCollection<Credential> listCred = new ObservableCollection<Credential>();
            //Credential newCred = new Credential() { Login = "user", Password = "Zpcc21nf", HostType = IPType.АРМ, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "user", Password = "Qgod7EF86", HostType = IPType.АРМ, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "dsadmin", Password = "WarY5Mtz9", HostType = IPType.Маршрутизатор, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "admin", HostType = IPType.ТелефонIP, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "admin", HostType = IPType.Принтер, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "31954", HostType = IPType.ТелефонСервисный, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "adminadmin", HostType = IPType.Принтер, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "user", Password = "Mee8voop", HostType = IPType.Офисный, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "records", Password = "Mee8voop", HostType = IPType.АРМ, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "user", Password = "uFoo8eesh", HostType = IPType.Офисный, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "Zot31954", HostType = IPType.VOIPШлюз, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "Zot31954", HostType = IPType.КамераIP, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "zot31954", HostType = IPType.VOIPШлюз, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "user", Password = "sheaWilm1s", HostType = IPType.АРМ, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "user", Password = "31954", HostType = IPType.КамераIP, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "adminoo", HostType = IPType.Принтер, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "admin", Password = "access", HostType = IPType.Принтер, CustomPassword = false };
            //Credentials.Add(newCred);
            //newCred = new Credential() { Login = "Admin", Password = "Admin", HostType = IPType.Принтер, CustomPassword = false };
            //Credentials.Add(newCred);
            //Credentials = listCred;




        }

        void RefreshCredentials()
        {
            this.LoadHotkeys();
            this.LoadUserCredentials();
            LoadCredentialsFromSQLServer();
            SelectedCredentials.Logins = LoginsViewModel.StringArrayToCollectionView((from i in Credentials select i.Login).Distinct().ToArray<string>());

            SelectedCredentials.Passwords = LoginsViewModel.StringArrayToCollectionView((from i in Credentials select i.Password).Distinct().ToArray<string>());

        }



        /// <summary>
        /// Метод, формирующий массив паролей в массиве string на основе логина и типа хоста
        /// </summary>
        /// <param name="login">Логин в string формате</param>
        /// <param name="iptype">Тип хоста IPType</param>
        /// <returns></returns>
        public static string[] GetPasswords(string login, IPType iptype/*, RegionEnum regiontype*/)
        {
            try
            {

                return (from s in Credentials
                            where (s.Login.Equals(login, StringComparison.OrdinalIgnoreCase) && ((s.HostType == IPType.Общий) || (s.HostType == iptype)) /*&& ((s.Region == RegionEnum.Общий)|| (s.Region == regiontype)*/)
                        select s.Password).Distinct<string>().ToArray<string>();
            }
            catch
            {
                return new string[] { string.Empty };
            }
        }


        private PasswordsMenuViewModel EnumPasswordsMenu()
        {
            PasswordsMenuViewModel model = new PasswordsMenuViewModel
            {
                MenuItems = new ObservableCollection<MenuItemViewModel>()
            };

            MenuItemViewModel item = new MenuItemViewModel("Логины", false, "")
            {
                MenuItems = new ObservableCollection<MenuItemViewModel>()
            };
            foreach (string str in (from s in Credentials select s.Login).Distinct<string>())
            {
                item.MenuItems.Add(new MenuItemViewModel(str, true, ""));
            }

            model.MenuItems.Add(item);


            MenuItemViewModel model3 = new MenuItemViewModel("Общие пароли", false, "")
            {
                MenuItems = new ObservableCollection<MenuItemViewModel>()
            };
            foreach (Credential credential in from p in Credentials
                                                  //where p.Region == RegionEnum.Общий
                                              select p)
            {
                model3.MenuItems.Add(new MenuItemViewModel(credential.Password, true, $"{credential.Login} {credential.HostType}"));
            }
            model.MenuItems.Add(model3);

            for (int i = 1; i < 11; i++)
            {
                RegionEnum RegionType;
                Func<Credential, bool> c;
                if ((i != 10) && (i != 9))
                {
                    RegionType = (RegionEnum)i;
                    MenuItemViewModel model4 = new MenuItemViewModel(RegionType.ToString(), false, "")
                    {
                        MenuItems = new ObservableCollection<MenuItemViewModel>()
                    };
                    foreach (Credential credential2 in Credentials.Where((c = p => p.Region == RegionType)))
                    {
                        model4.MenuItems.Add(new MenuItemViewModel(credential2.Password, true, $"{credential2.Login} {credential2.HostType}"));
                    }
                    model.MenuItems.Add(model4);
                }
            }
            model.MenuItems.Add(new MenuItemViewModel("Отмена", false, ""));
            return model;
        }

        /// <summary>
        /// Временный метод, пока не реализована возможность загрузки из SQL сервера
        /// </summary>
        /// <returns></returns>
        private bool LoadCredentialsFromSQLServer()
        {
            try
            {
                Credentials.Clear();
                Credentials.AddRange(UserCredentials);
                return true;
            }
            catch (Exception ex)
            {
                Log("Ошибка загрузки удостоверений личности. " + ex.Message,true, true, ex.StackTrace);
                return false;
            }
        }

        private bool LoadCredentialsFromSQLServer(string connectionString)
        {
            try
            {
                //Credentials.Clear();
                //Credentials.AddRange(UserCredentials);
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    connection.Open();
                //    string cmdText = Bindings.GrantedAccess.HasFlag(AccessRoles.Grade2) ? "SELECT * FROM Credential ORDER BY SortOrder DESC" : "SELECT * FROM Credential WHERE Login = 'cms'";
                //    using (SqlCommand command = new SqlCommand(cmdText, connection))
                //    {
                //        using (SqlDataReader reader = command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                Credential item = new Credential
                //                {
                //                    Login = reader["Login"].ToString(),
                //                    CryptedPassword = reader["CryptedPassword"].ToString()
                //                };
                //                if (!reader.IsDBNull(reader.GetOrdinal("Region")))
                //                {
                //                    item.Region = (RegionEnum)System.Enum.Parse(typeof(RegionEnum), reader["Region"].ToString());
                //                }
                //                if (!reader.IsDBNull(reader.GetOrdinal("HostType")))
                //                {
                //                    item.HostType = (IPType)System.Enum.Parse(typeof(IPType), reader["HostType"].ToString());
                //                }
                //                Credentials.Add(item);
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                //this.Error(exception.Message,, exception.StackTrace);
                Log("Ошибка при загрузке паролей." + ex.Message, true, false, ex.StackTrace);
                return false;
            }
            return true;
        }


        #endregion

    }
}
