
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DSList
{
    public class IP : INotifyPropertyChanged
    {
        #region Реализация INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Поля

        ContextMenu cmIP = new ContextMenu();
        private string _IPAddress;
        private string _Description;
        private string _Latency;
        private string _NetbiosName;
        private CustomerLogic _Owner;
        private string _Region;
        private string _Tooltip;
        private IPType _Type;
        private Host _InfoHost = new Host();

        #endregion

        #region Свойства

        /// <summary>
        /// Свойство IP адрес
        /// </summary>
        public string IPAddress
        {
            get
            {
                return this._IPAddress;
            }
            set
            {
                this._IPAddress = value;
                NotifyPropertyChanged("IPAddress");
            }
        }

        /// <summary>
        /// Свойство описания IP хоста
        /// </summary>
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
                NotifyPropertyChanged("Description");
            }
        }
        
        /// <summary>
        /// Свойство задержка Ping
        /// </summary>
        public string Latency
        {
            get
            {
                return this._Latency;
            }
            set
            {
                this._Latency = value;
                NotifyPropertyChanged("Latency");
            }
        }

        /// <summary>
        /// Свойство Имя хоста
        /// </summary>
        public string NetbiosName
        {
            get
            {
                return this._NetbiosName;
            }
            set
            {
                this._NetbiosName = value;
                NotifyPropertyChanged("NetbiosName");
            }
        }

        /// <summary>
        /// Свойство Информация по комплектации хоста
        /// </summary>
        public Host InfoHost
        {
            get
            {
                return this._InfoHost;
            }
            set
            {
                this._InfoHost = value;
                NotifyPropertyChanged("InfoHost");
            }
        }

        /// <summary>
        /// Свойство хозяина класса
        /// </summary>
        public CustomerLogic Owner
        {
            get
            {
                return this._Owner;
            }
            set
            {
                this._Owner = value;
                NotifyPropertyChanged("Owner");
            }
        }

        /// <summary>
        /// Свойство регион, к которому принадлежит хост
        /// </summary>
        public string Region
        {
            get
            {
                return this._Region;
            }
            set
            {
                this._Region = value;
                NotifyPropertyChanged("Region");
            }
        }

        /// <summary>
        /// Свойство подсказка для данного IP хоста
        /// </summary>
        public string Tooltip
        {
            get
            {
                return this._Tooltip;
            }
            set
            {
                this._Tooltip = value;
                NotifyPropertyChanged("Tooltip");
            }
        }

        /// <summary>
        /// Свойство тип хоста IPType
        /// </summary>
        public IPType Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
                NotifyPropertyChanged("Type");
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Класс IP хост
        /// </summary>
        public IP() { }

        /// <summary>
        /// Класс IP хост
        /// </summary>
        /// <param name="ipType">тип IP (IPType)</param>
        /// <param name="address">IP адрес</param>
        /// <param name="netbiosName">имя хоста</param>
        public IP(IPType ipType, string address, string netbiosName)
        {
            Type = ipType;
            IPAddress = address;
            NetbiosName = netbiosName;
        }
        #endregion

        #region Методы

        /// <summary>
        /// Метод определения типа хоста по имени
        /// </summary>
        private void DetectTypeByName()
        {
            if (this.NetbiosName.Length >= 10)
            {
                char c = this.NetbiosName.ToLower()[9];
                if ((char.IsDigit(c) || (c == 'f')) && (this.Type != IPType.АРМ))
                {
                    this.Description = "АРМ (авто)";
                    this.Type = IPType.АРМ;
                }
                else if ((c == 'r') && (this.Type != IPType.Маршрутизатор))
                {
                    this.Description = "Маршрутизатор (авто)";
                    this.Type = IPType.Маршрутизатор;
                }
                else if ((c == 'p') && (this.Type != IPType.Принтер))
                {
                    this.Description = "Принтер (авто)";
                    this.Type = IPType.Принтер;
                }
                else if ((c == 's') && (this.Type != IPType.Видеорегистратор))
                {
                    this.Description = "Видеорегистратор";
                    this.Type = IPType.Видеорегистратор;
                }
            }
        }

        /// <summary>
        /// Метод получения типа хоста
        /// </summary>
        /// <param name="name">имя хоста</param>
        /// <returns></returns>
        public static IPType GetIPType(string name)
        {
            try
            {
                char c = name.ToLower()[9];
                if (char.IsDigit(c) || (c == 'f'))
                {
                    return IPType.АРМ;
                }
                if (c == 'r')
                {
                    return IPType.Маршрутизатор;
                }
                if (c == 'p')
                {
                    return IPType.Принтер;
                }
                if (c == 's')
                {
                    return IPType.Видеорегистратор;
                }
            }
            catch
            {
            }
            return IPType.АРМ;
        }

        /// <summary>
        /// Метод возвращает имя хоста или его IP
        /// </summary>
        /// <returns>имя хоста или его IP в string</returns>
        public string NBNameOrIP()
        {
            if (!string.IsNullOrEmpty(this.NetbiosName) && (this.NetbiosName != "..."))
            {
                return this.NetbiosName;
            }
            return this.IPAddress;
        }

        /// <summary>
        /// Метод ping хоста
        /// </summary>
        /// <param name="resolveHostname">разрешить имя хоста</param>
        public void Ping(bool resolveHostname = true)
        {
            if (this.IPAddress != "0.0.0.0")
            {
                if (resolveHostname)
                {
                    new Thread(new ThreadStart(this.ResolveNameThread)).Start();
                }
                new Thread(new ThreadStart(this.PingThread)).Start();
            }
        }

        /// <summary>
        /// Метод выполнения пинга
        /// </summary>
        /// <returns></returns>
        private bool PingOperation()
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            PingReply reply = null;
            try
            {
                reply = ping.Send(this.IPAddress);
                if (reply.Status != IPStatus.TimedOut)
                {
                    this.Latency = reply.RoundtripTime.ToString();
                }
                else
                {
                    this.Latency = "-";
                }
                if (this.Latency == "0")
                {
                    this.Latency = "-";
                }
            }
            catch
            {
                this.Latency = "-";
            }
            return (this.Latency != "-");
        }

        /// <summary>
        /// Метод выполнения Ping в потоке
        /// </summary>
        private void PingThread()
        {
            this.ProcessPing(true, true);
        }

        /// <summary>
        /// Метод выполнения Ping
        /// </summary>
        /// <param name="retry">признак выполнения повторения</param>
        /// <param name="showProcess">признак отображения процесса</param>
        /// <returns></returns>
        public bool ProcessPing(bool retry = true, bool showProcess = true)
        {
            if (showProcess)
            {
                this.Latency = "...";
                NotifyPropertyChanged("Latency");
            }
            bool flag = this.PingOperation();
            if ((retry && !flag) && !string.IsNullOrWhiteSpace(this.NetbiosName))
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    flag = this.PingOperation();
                    if (flag)
                    {
                        break;
                    }
                }
            }
            NotifyPropertyChanged("Latency");
            return flag;
        }

        /// <summary>
        /// Метод  
        /// </summary>
        public void ResolveNameThread()
        {
            string str = (this.NetbiosName == null) ? "" : this.NetbiosName;
            this.NetbiosName = "...";
            NotifyPropertyChanged("NetbiosName");
            try
            {
                this.NetbiosName = Dns.GetHostEntry(this.IPAddress).HostName.Replace(".maxus.lan", "");
                //if (MainWindow.settings.Fields.DetectTypeByName && ((this.NetbiosName.Length == 11) || (this.NetbiosName.Length == 12)))
                //{
                //    if ((this.NetbiosName[8] == '_') && char.IsDigit(this.NetbiosName[10]))
                //    {
                //        this.DetectTypeByName();
                //        this.UpdateHostInfo(null);
                //        NotifyPropertyChanged("Description");
                //        NotifyPropertyChanged("Tooltip");
                //    }
                //    if (this.NetbiosName[2] == '-')
                //    {
                //        this.Description = "Офисный ПК";
                //        this.Type = IPType.Офисный;
                //        NotifyPropertyChanged("Description");
                //    }
                //}
            }
            catch
            {
                //Host selectedHost = this.Owner.Hosts.FirstOrDefault<Host>(h => h.IPAddress == this.Address);
                //if (selectedHost != null)
                //{
                //    this.NetbiosName = selectedHost.NetbiosName;
                //    this.DetectTypeByName();
                //    this.UpdateHostInfo(selectedHost);
                //}
                //else
                //{
                this.NetbiosName = str;
                //}
            }
            NotifyPropertyChanged("NetbiosName");
        }

        /// <summary>
        /// Обновление информации о хосте
        /// </summary>
        /// <param name="selectedHost"></param>
        private void UpdateHostInfo(Host selectedHost = null)
        {
            Host host = (selectedHost == null) ? this.Owner.Hosts.FirstOrDefault<Host>(h => h.NetbiosName.Equals(this.NetbiosName, StringComparison.OrdinalIgnoreCase)) : selectedHost;
            if (host != null)
            {
                this.Tooltip = string.Empty;
                //if (!string.IsNullOrWhiteSpace(host.Vendor))
                //{
                //    this.Tooltip = host.Vendor + " ";
                //}
                //if (!string.IsNullOrWhiteSpace(host.Model))
                //{
                //    this.Tooltip = this.Tooltip + host.Model;
                //}
                //if (!string.IsNullOrWhiteSpace(host.RamSize))
                //{
                //    this.Tooltip = this.Tooltip + "\r\nОЗУ: " + host.RamSize + " МБ";
                //}
                //if ((this.Type == IPType.ТО) || (this.Type == IPType.УТ))
                //{
                //    this.Description = this.Description + " " + host.TOID;
                //    this.TOID = host.TOID;
                //    if (!string.IsNullOrWhiteSpace(host.MBName))
                //    {
                //        this.Tooltip = this.Tooltip + "\r\nМ/П: " + host.MBName;
                //    }
                //}
                //this.Tooltip = this.Tooltip + "\r\nДанные от " + host.LastHWScan;
            }
        }

        public override string ToString() =>
            this.IPAddress;

        #endregion
    }
}

