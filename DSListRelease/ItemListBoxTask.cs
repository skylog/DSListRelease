using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DSList
{
    /// <summary>
    /// Класс, реализующий Item для ListBox ListBoxTask
    /// </summary>
    public class ItemListBoxTask : ModelBase
    {
        private double _CurValueProgressBar = 0;
        private string _IPOrName;
        private string _Description;
        private double _MaxValueProgressBar;
        private bool _StopProcess = false;
        private string _TimeStart = DateTime.Now.ToLongTimeString();
        private string _CVZNumber;
        private bool _IsIndeterminate = false;

        public string TimeStart { get { return _TimeStart; } }


        public bool IsIndeterminate
        {
            get { return _IsIndeterminate; }
            set
            {
                _IsIndeterminate = value;
                NotifyPropertyChanged("IsIndeterminate");
            }
        }
        public bool StopProcess
        {
            get { return _StopProcess; }
            set
            {
                _StopProcess = value;
                NotifyPropertyChanged("StopProcess");
            }
        }


        public string CVZNumber
        {
            get { return _CVZNumber; }
            set
            {
                _CVZNumber = value;
                NotifyPropertyChanged("CVZNumber");
            }
        }
        public string IPOrName
        {
            get { return _IPOrName; }
            set
            {
                _IPOrName = value;
                NotifyPropertyChanged("IPOrName");
            }
        }
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                NotifyPropertyChanged("Description");
            }
        }
        public double MaxValueProgressBar
        {
            get { return _MaxValueProgressBar; }
            set
            {
                _MaxValueProgressBar = value;
                NotifyPropertyChanged("MaxValueProgressBar");
            }
        }
        public double CurValueProgressBar
        {
            get { return _CurValueProgressBar; }
            set
            {
                _CurValueProgressBar = value;
                NotifyPropertyChanged("CurValueProgressBar");
            }
        }

    }
}
