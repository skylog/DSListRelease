using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Security;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;

namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для Access1.xaml  Класс предоставления доступа
    /// </summary>
    public partial class Access : Window, IComponentConnector
    {
        protected internal bool blockRunDSList = true;
        /// <summary>
        /// String значения ключа шифрования (основной ключ шифрования, применяемый в программе)
        /// </summary>
        public static string sv_password = Crypt.FromBase64("c3ZfcGFzc3dvcmQ=");

        public Access()
        {
            this.InitializeComponent();
            //MainWindow.DBPath = @"\\maxus.lan\msk\A\IT\All\Централизация\!Единая ТП ТТ\ttlist";
            //MainWindow.helpfile = Path.Combine(MainWindow.DBPath, "help.zip");
            //MainWindow.pilot_sbr_file = Path.Combine(MainWindow.DBPath, "pilot_sbr.txt");
            //MainWindow.pilot_flora_file = Path.Combine(MainWindow.DBPath, "pilot_flora.txt");
            //MainWindow.pilot_bio_file = Path.Combine(MainWindow.DBPath, "pilot_bio.txt");
            //MainWindow.pilot_zlock_file = Path.Combine(MainWindow.DBPath, "pilot_zlock.txt");
            //MainWindow.versions_file = Path.Combine(MainWindow.DBPath, "versions.txt");
            
            App.splashScreen.AddMessage("Инициализация...");
            NewMainWindow window = new NewMainWindow();
            Application.Current.MainWindow = window;
            try
            {
                //PrincipalContext pc = new PrincipalContext(ContextType.Domain, Environment.UserDomainName);
                App.splashScreen.AddMessage("Авторизация.");
                //if (CheckUserInGroup(pc, "SG.IT.ttlist.AccessMinimal"))
                //{
                //    MainWindow.Bindings.GrantedAccess |= AccessRoles.Minimal;
                //}
                App.splashScreen.AddMessage("Авторизация..");
                //if (CheckUserInGroup(pc, "SG.IT.ttlist.AccessGrade2"))
                //{
                //    MainWindow.Bindings.GrantedAccess |= AccessRoles.Grade2;
                //}
                App.splashScreen.AddMessage("Авторизация...");
                //if (CheckUserInGroup(pc, "SG.IT.ttlist.AccessFull"))
                //{
                //    MainWindow.Bindings.GrantedAccess |= AccessRoles.Full;
                //}
                App.splashScreen.AddMessage("Авторизация....");
                //if (CheckUserInGroup(pc, "SG.IT.ttlist.AccessOfficeSupport"))
                //{
                //    MainWindow.Bindings.GrantedAccess |= AccessRoles.Office;
                //}
                App.splashScreen.AddMessage("Авторизация.....");
                //if (CheckUserInGroup(pc, "SG.IT.OPP.Master-Line"))
                //{
                //    MainWindow.Bindings.GrantedAccess |= AccessRoles.Ext;
                //}
            }
            catch (Exception exception)
            {
                App.splashScreen.ShowError(exception.Message, "", false);
            }
            //if (!MainWindow.Bindings.GrantedAccess.HasFlag(AccessRoles.Minimal))
            //{
            //    App.splashScreen.ShowError("Доступ запрещен для пользователя " + Environment.UserName, "", true);
            //    Environment.Exit(1);
            //}
            

            App.splashScreen.AddMessage("Загрузка списка ЦВЗ из БД...");
            //window.LoadDBFromSQLServer("Data Source=localhost;Port=3306;Initial Catalog=mikrotik;User Id=root;password=fawGV89094047282");
            //window.LoadDBFromSQLServer(NewMainWindow.settings.Fields.ConnectionString);
            window.LoadDBFromSQLServer("Data Source=gms.dengisrazy.ru;Port=3306;Initial Catalog=mikrotik;User Id=otp;password=Wct3XH39hxbC;Encrypt=false;");
            window.FillSearchListBoxAllTT();
            App.splashScreen.AddMessage("Запуск...");
            //window.SelectedLoginCreate();
            window.Show();
            base.Close();
        }

        private static bool CheckUserInGroup(PrincipalContext pc, string Group)
        {
            try
            {
                GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, Group);
                if (group == null)
                {
                    throw new ArgumentNullException(Group, "Группа безопасности не найдена.");
                }
                return UserPrincipal.FindByIdentity(pc, Environment.UserName).IsMemberOf(group);
            }
            catch (Exception exception)
            {
                App.splashScreen.ShowError(exception.Message, "", false);
                return false;
            }
        }
    }
}


