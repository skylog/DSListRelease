using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    public class Host : ModelBase
    {
        private string _CPUName;
        private string _HDDSize;
        private string _MBName;
        private string _NetbiosName;
        private string _RamSize;

        public string CPUFreq { get; set; }

        
        public string CPUName
        {
            get
            {
                return this._CPUName;
            }
            set
            {
                this._CPUName = value;
                NotifyPropertyChanged("CPUName");
            }
        }

        public string HDDName { get; set; }

        
        public string HDDSize
        {
            get
            {
                return this._HDDSize;
            }
            set
            {
                this._HDDSize = value;
                NotifyPropertyChanged("HDDSize");
            }
        }

        public string HDDSmartStatus { get; set; }

        public string HostType { get; set; }

        public string IPAddress { get; set; }

        public string LastHWScan { get; set; }

        
        public string MBName
        {
            get
            {
                return this._MBName;
            }
            set
            {
                this._MBName = value;
                NotifyPropertyChanged("MBName");
            }
        }

        public string MBSerial { get; set; }

        public string Model { get; set; }

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

        
        public string RamSize
        {
            get
            {
                return this._RamSize;
            }
            set
            {
                this._RamSize = value;
                NotifyPropertyChanged("RamSize");
            }
        }

        public string Serial { get; set; }

        public string TOID { get; set; }

        public string Vendor { get; set; }

        
    }
}
