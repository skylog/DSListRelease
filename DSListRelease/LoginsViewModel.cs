using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DSList
{
    /// <summary>
    /// Класс представляющий модель представления данных авторизации
    /// </summary>
    public class LoginsViewModel : ModelBase
    {
        /// <summary>
        /// Поле логина
        /// </summary>
        private string _Login;

        /// <summary>
        /// Поле, представляющее коллекцию CollectionView логинов
        /// </summary>
        private CollectionView _Logins;

        /// <summary>
        /// Поле пароля
        /// </summary>
        private string _Password;

        /// <summary>
        /// Поле паролей, представленных в коллекции ColletionView 
        /// </summary>
        private CollectionView _Passwords;
        
        /// <summary>
        /// Свойство, представляющее список удостоверений личности Credentials в List-Credential
        /// </summary>
        private static List<Credential> Credentials { get; set; }

        /// <summary>
        /// Метод формирования коллекции из строкового массива
        /// </summary>
        /// <param name="stringArray">входной строковый массив</param>
        /// <returns>Коллекция CollectionView</returns>
        public static CollectionView StringArrayToCollectionView(string[] stringArray)
        {
            List<BindingString> collection = new List<BindingString>();
            foreach (string str in stringArray)
            {
                collection.Add(new BindingString(str));
            }
            return new CollectionView(collection);
        }

        public NewMainWindow Owner { get; set; }

        /// <summary>
        /// Свойство, представляющее логин
        /// </summary>
        public string Login
        {
            get
            {
                return this._Login;
            }
            
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this._Login = string.Empty;
                }
                else
                {
                    this._Login = value;
                    base.NotifyPropertyChanged("Login");
                    if ((Owner.SelectedTT != null) && (Owner.SelectedIP != null))
                    {
                        //RegionEnum regiontype = (MainWindow.SelectedTT.RegionType != RegionEnum.Общий) ? MainWindow.SelectedTT.RegionType : Customer.GetRegionByCode(MainWindow.SelectedIP.NetbiosName);
                        this.Passwords = StringArrayToCollectionView(NewMainWindow.GetPasswords(value, Owner.SelectedIP.Type/*, regiontype*/));
                    }
                }
            }
        }


        /// <summary>
        /// Свойство, представляющее коллекцию логинов CollectionView
        /// </summary>
        public CollectionView Logins
        {
            get { return this._Logins; }

            set
            {
                this._Logins = value;
                base.NotifyPropertyChanged("Logins");
            }
        }

        /// <summary>
        /// Свойство, представляющее пароль
        /// </summary>
        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                if (this._Password != value)
                {
                    this._Password = value;
                    base.NotifyPropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// Свойство, представляющее коллекцию паролей CollectionView
        /// </summary>
        public CollectionView Passwords
        {
            get
            {
                return this._Passwords;
            }
            set
            {
                this._Passwords = value;
                base.NotifyPropertyChanged("Passwords");
            }
        }
    }
}
