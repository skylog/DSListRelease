using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DSList
{
    public class CustomerBase: INotifyPropertyChanged
    {

        #region Поля
        private NewMainWindow owner;
        private int _NumberCVZ = 0;
        private string _Address;
        private string _WanIP;
        private string _JasperIP;
        private string _IPSECPass;
        private ObservableCollection<BindingListItem> _Info;
        private List<Host> _Hosts;
        private ObservableCollection<IP> _IPArray;
        private string _SubnetMask = string.Empty;
        private string _Timezone /*= "+00:00"*/;
        private List<ProvInfo> _Providers;
        private string _Region;
        private string _RegionClock;


        protected internal DispatcherTimer UpdateTimer = new DispatcherTimer();
        protected internal BackgroundWorker UpdateBW = new BackgroundWorker();

        private string _FormatName = string.Empty;


        public int CloseCounter = 0;
        public CustomerTypes CustomerType = CustomerTypes.TTCVZ;
        protected internal static string[] Days = new string[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        public string Gateway = string.Empty;

        //protected internal ContextMenu listViewItemContextMenu = new ContextMenu();    // Контекстное меню list Customer
        //protected internal ContextMenu customerContextMenu = new ContextMenu();    // Контекстное меню list Customer

        protected internal DispatcherTimer timer;
        private string _Login;
        private string _Email;
        private string _Status;
        private string _WorkTime;
        private string _Province;
        private string _City;
        private string _Phone;
        private string _RD;
        private string _RDPhone;
        private string _MRD;
        private string _MRDPhone;
        private Brush _TitleColor;
        private string _Organization;
        private ProvInfo _Provider;
        private List<Employee> _Employees;
        private string _WanLogin;
        private string _Lan_Ip;
        private string _WanPass;
        private string _WanGW;
        private string _ICCID;
        private string _ConnectType;
        private string _WanMask;
        private ListView _ListViewHostsInCustomer;

        public event TTEvent OnCloseCounter;

        //public event TTEvent OnIPDetected;

        //public event TTEvent OnIPNotDetected;

        //public static readonly DependencyProperty SelectedIPProperty = DependencyProperty.Register("SelectedIP", typeof(IP), typeof(Customer), new PropertyMetadata(null));
        //public static readonly DependencyProperty CommentProperty = DependencyProperty.Register("Comment", typeof(string), typeof(Customer), new PropertyMetadata(""));
        private string _Comment;

        public string Comment
        {
            get
            {
                //if ((string)GetValue(CommentProperty) != null)
                //    return (string)GetValue(CommentProperty);
                //else
                //    return "";
                //return this._Comment;
                if (_Comment != null)
                    return _Comment;
                else
                    return "";
                //return this._Comment;


            }
            set
            {
                this._Comment = value;
                NotifyPropertyChanged("Comment");
                //SetValue(CommentProperty, value);
            }
        }

        #endregion

        #region Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion



        #region Свойства

        /// <summary>
        /// Свойство, включающее NewMainWindow в Customer
        /// </summary>
        public NewMainWindow Owner
        {
            get { return owner; }
            set { owner = value; }
        }


        /// <summary>
        /// Свойство, представляющее сотрудников
        /// </summary>
        public List<Employee> Employees
        {
            get
            {
                return this._Employees;
            }
            set
            {
                this._Employees = value;
                NotifyPropertyChanged("Employees");
            }
        }

        /// <summary>
        /// Свойство, возвращающее строковое представление для отображения в listSearch (список найденных ЦВЗ)
        /// </summary>
        public string ToStringDisplay
        {
            get { return $"ЦВЗ №{NumberCVZ} {Address} {City}"; }
        }

        public Brush TitleColor
        {
            get
            {
                return this._TitleColor;
            }
            set
            {
                this._TitleColor = value;
                NotifyPropertyChanged("TitleColor");
            }
        }

        public string Organization
        {
            get
            {
                return this._Organization;
            }
            set
            {
                this._Organization = value;
                NotifyPropertyChanged("Organization");
            }
        }

        public string MRD
        {
            get
            {
                return this._MRD;
            }
            set
            {
                this._MRD = value;
                NotifyPropertyChanged("OM");
            }
        }

        public string MRDPhone
        {
            get
            {
                return this._MRDPhone;
            }
            set
            {
                this._MRDPhone = value;
                NotifyPropertyChanged("OMPhone");
            }
        }

        public string RD
        {
            get
            {
                return this._RD;
            }
            set
            {
                this._RD = value;
                NotifyPropertyChanged("RD");
            }
        }

        public ListView ListViewHostsInCustomer
        {
            get
            {
                return this._ListViewHostsInCustomer;
            }
            set
            {
                this._ListViewHostsInCustomer = value;
                NotifyPropertyChanged("ListViewHostsInCustomer");
            }
        }

        public string RDPhone
        {
            get
            {
                return this._RDPhone;
            }
            set
            {
                this._RDPhone = value;
                NotifyPropertyChanged("RTTPhone");
            }
        }

        public string Phone
        {
            get
            {
                return this._Phone;
            }
            set
            {
                this._Phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        public string City
        {
            get
            {
                return this._City;
            }
            set
            {
                this._City = value;
                NotifyPropertyChanged("City");
            }
        }
        /// <summary>
        /// Регион (область), в котором находится населённый пункт
        /// </summary>
        public string Province
        {
            get
            {
                return this._Province;
            }
            set
            {
                this._Province = value;
                NotifyPropertyChanged("Province");
            }
        }

        public string WorkTime
        {
            get
            {
                return this._WorkTime;
            }
            set
            {
                this._WorkTime = value;
                NotifyPropertyChanged("WorkTime");
            }
        }


        public string Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("StatusImage");
                NotifyPropertyChanged("StatusInt");
            }
        }

        public string StatusIcon
        {
            get
            {
                switch (this.Status.ToLower())
                {
                    case "открыта":
                        return @"Images\ACTIVATED_NAME.png";

                    case "закрыта":
                        return @"Images\DEACTIVATED_NAME.png";

                    case "готовитсякоткрытию":
                        return @"Images\ACTIVATION_READY_NAME.png";
                }
                return @"Images\PURGED_NAME.png";
            }
        }

        public int StatusInt
        {
            get
            {
                switch (this.Status.ToLower())
                {
                    case "открыта":
                        return 1;

                    case "закрыта":
                        return 2;

                    case "готовитсякоткрытию":
                        return 0;
                }
                return 3;
            }
        }

        public string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
                NotifyPropertyChanged("Email");
            }
        }

        public string Login
        {
            get
            {
                return this._Login;
            }
            set
            {
                this._Login = value;
                NotifyPropertyChanged("Login");
            }
        }

        public string WanLogin
        {
            get
            {
                return this._WanLogin;
            }
            set
            {
                this._WanLogin = value;
                NotifyPropertyChanged("WanLogin");
            }
        }

        public string WanPass
        {
            get
            {
                return this._WanPass;
            }
            set
            {
                this._WanPass = value;
                NotifyPropertyChanged("WanPass");
            }
        }

        public string WanGW
        {
            get
            {
                return this._WanGW;
            }
            set
            {
                this._WanGW = value;
                NotifyPropertyChanged("WanGW");
            }
        }

        public string WanMask
        {
            get
            {
                return this._WanMask;
            }
            set
            {
                this._WanMask = value;
                NotifyPropertyChanged("WanMask");
            }
        }

        public string ICCID
        {
            get
            {
                return this._ICCID;
            }
            set
            {
                this._ICCID = value;
                NotifyPropertyChanged("ICCID");
            }
        }

        public string ConnectType
        {
            get
            {
                return this._ConnectType;
            }
            set
            {
                this._ConnectType = value;
                NotifyPropertyChanged("ConnectType");
            }
        }

        public string FormatName
        {
            get
            {
                return this._FormatName;
            }
            set
            {
                this._FormatName = value;
                NotifyPropertyChanged("FormatName");
            }
        }

        public ObservableCollection<BindingListItem> Info
        {
            get
            {
                return this._Info;
            }
            set
            {
                this._Info = value;
                NotifyPropertyChanged("Info");
            }
        }
        public string SubnetMask
        {
            get
            {
                return this._SubnetMask;
            }
            set
            {
                this._SubnetMask = value;
                NotifyPropertyChanged("SubnetMask");
            }
        }

        public string Timezone
        {
            get
            {
                return this._Timezone;
            }
            set
            {
                //char[] ch = value.ToCharArray();
                //int timeint = int.Parse(ch[2].ToString()) - 3;
                ////string time = ch[2].ToString();
                //this._Timezone = timeint.ToString();
                this._Timezone = value;
                NotifyPropertyChanged("Timezone");
            }
        }

        public ObservableCollection<IP> IPArray
        {
            get
            {
                return this._IPArray;
            }
            set
            {
                this._IPArray = value;
                NotifyPropertyChanged("IPArray");
            }
        }

        public List<Host> Hosts
        {
            get
            {
                return this._Hosts;
            }
            set
            {
                this._Hosts = value;
                NotifyPropertyChanged("Hosts");
            }
        }

        public int NumberCVZ
        {
            get
            {
                return this._NumberCVZ; }
            set
            {
                this._NumberCVZ = value;
                NotifyPropertyChanged("NumberCVZ");
            }
        }

        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                NotifyPropertyChanged("Address");
            }
        }
        public string Lan_Ip
        {
            get { return _Lan_Ip; }
            set
            {
                _Lan_Ip = value;
                NotifyPropertyChanged("Lan_Ip");
            }
        }
        public string WanIP
        {
            get { return _WanIP; }
            set
            {
                _WanIP = value;
                NotifyPropertyChanged("WanIP");
            }
        }

        public string JasperIP
        {
            get { return _JasperIP; }
            set
            {
                _JasperIP = value;
                NotifyPropertyChanged("JasperIP");
            }

        }
        public string IPSECPass
        {
            get { return _IPSECPass; }
            set
            {
                _IPSECPass = value;
                NotifyPropertyChanged("IPSECPass");
            }

        }



        /// <summary>
        /// Свойство, представляющее текущего провайдера типа ProvInfo
        /// </summary>
        public ProvInfo Provider
        {
            get
            {
                return this._Provider;
            }
            set
            {
                this._Provider = value;
                NotifyPropertyChanged("Provider");
            }
        }

        public List<ProvInfo> Providers
        {
            get
            {
                return this._Providers;
            }
            set
            {
                this._Providers = value;
                NotifyPropertyChanged("Providers");
            }
        }

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

        public string RegionClock
        {
            get
            {
                return this._RegionClock;
            }
            set
            {
                this._RegionClock = value;
                NotifyPropertyChanged("RegionClock");
            }
        }


        public string Router { get; set; }

        public override string ToString()
        {
            return $" ЦВЗ №{NumberCVZ.ToString()} {City} {Address}";
        }
        #endregion

    }
}
