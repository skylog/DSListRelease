using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace DSList
{
    public class BindingVariables : ModelBase
    {
        private ObservableCollection<ItemListBoxTask> _listBoxTaskItemsSource = new ObservableCollection<ItemListBoxTask>();
        public ObservableCollection<ItemListBoxTask> listBoxTaskItemsSource
        {
            get
            {
                return _listBoxTaskItemsSource;
            }
            set
            {
                this._listBoxTaskItemsSource = value;
                NotifyPropertyChanged("_listBoxTaskItemsSource");
            }
        }


        private Brush _brush = Brushes.Black;
        private bool _isIpSelected = false;
        private string _SearchText;
        private Visibility _Visibility;


        public string SearchText
        {
            get
            {
                return this._SearchText;
            }
            set
            {
                this._SearchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }

        public bool IsTSConnecting
        {
            get
            {
                return this._IsTSConnecting;
            }
            set
            {
                this._IsTSConnecting = value;
                NotifyPropertyChanged("IsTSConnecting");
            }
        }

        public Visibility VisibilitySetting
        {
            get
            {
                return this._Visibility;
            }
            set
            {
                this._Visibility = value;
                NotifyPropertyChanged("VisibilitySetting");
            }
        }

        //public string StatusLabelText
        //{
        //    get
        //    {
        //        return this._StatusLabelText;
        //    }
        //    set
        //    {
        //        this._StatusLabelText = value;
        //        NotifyPropertyChanged("StatusLabelText");
        //    }
        //}

        public Brush brush
        {
            get
            {
                return
               this._brush;

            }
            set
            {
                this._brush = value;
                base.NotifyPropertyChanged("brush");
            }
        }

        /// <summary>
        /// Свойство признака выбранности хоста
        /// </summary>
        public bool isipselected
        {
            get { return _isIpSelected; }
            set { _isIpSelected = value; NotifyPropertyChanged("isipselected"); }
        }


        private string _StatusBarText;
        internal string title;
        private bool _isttselected;
        internal string dbversion;
        private string _title2;
        private bool _IsTSConnecting;

        /// <summary>
        /// Свойство текста StatusBar основного окна.
        /// </summary>
        public string StatusBarText
        {
            get { return _StatusBarText; }
            set
            {
                _StatusBarText = value;
                NotifyPropertyChanged("StatusBarText");
            }
        }

        public bool isttselected
        {
            get
            {
                return this._isttselected;
            }
            set
            {
                this._isttselected = value;
                base.NotifyPropertyChanged("isttselected");
            }
        }

        public string title2
        {
            get
            {
                return this._title2;
            }
            set
            {
                this._title2 = value;
                base.NotifyPropertyChanged("title2");
            }
        }
    }
}
