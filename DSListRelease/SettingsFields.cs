using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    /// <summary>
    /// Класс полей настроек
    /// </summary>
    public class SettingsFields
    {
        public bool AdditionalProvIP = false;
        /// <summary>
        /// Поле настроек поиска в расширенном режиме
        /// </summary>
        public bool AlwaysAdvSearch = false;
        /// <summary>
        /// Поле настроек автоматического закрытие закладок
        /// </summary>
        public bool AutoCloseTab = true;
        public bool autoupdate = false;
        public bool BigPopupToolbar = false;
        public bool ChangeLayout = false;
        /// <summary>
        /// Поле настроек времени (в минутах), через которое вкладка закрывается
        /// </summary>
        public int CloseTabTime = 15;
        //public string ConnectionString = "Data Source=gms.dengisrazy.ru;Port=3306;Initial Catalog=mikrotik;User Id=otp;password=Wct3XH39hxbC;Encrypt=false;";
        public string ConnectionString = "";
        public bool DetectTypeByName = true;
        /// <summary>
        /// Поле настроек Ping при двойном нажатии по хосту
        /// </summary>
        public bool DoubleClickPing = false;
        /// <summary>
        /// Поле настроек путь к FileZilla
        /// </summary>
        public string filezilla_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"\Desktop\FileZillaPortable\FileZillaPortable.exe");
        public bool DWDirect = true;
        /// <summary>
        /// Поле настроек скрытия недоступных в настоящий момент хостов
        /// </summary>
        public bool HideOffline = true;
        /// <summary>
        /// Поле настроек сворачивания программы при закрытии
        /// </summary>
        public bool MinimizeOnClose = false;
        public string nsc_lastfolder = string.Empty;
        /// <summary>
        /// Поле настроек признак открытия только одной вкладки Customer в TabControl
        /// </summary>
        public bool OneTab = false;
        /// <summary>
        /// Поле настроек задержка (для медленных компьютеров)
        /// </summary>
        public bool PingDelay = false;
        public bool PopupMenuDelayed = false;
        /// <summary>
        /// Поле настроек путь к Putty
        /// </summary>
        public string putty_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"\Desktop\PuTTy\PuTTy.exe");
        public bool restart_confirm = false;
        public bool SearchGroupByStatus = false;
        public bool ShowClosed = true;
        /// <summary>
        /// Поле настроек путь к winbox.exe MikroTik
        /// </summary>
        public string winboxmikrotikpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"\Desktop\winbox.exe");
        public bool winboxmikrotik_path_usedefault = false;
        public int ToastShowTime = 10;
        public string version = "2.0.0.0";
        public string PhoneIPNumber;
    }
}
