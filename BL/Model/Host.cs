using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model
{
    public class Host // : ModelBase
    {
        private string cpuName;
        private string hddSize;
        private string mbName;
        private string netBiosName;
        private string ramSize;

        public string CPUFreq { get; set; }


        public string CPUName
        {
            get
            {
                return this.cpuName;
            }
            set
            {
                this.cpuName = value;
                //NotifyPropertyChanged("cpuName");
            }
        }

        public string HDDName { get; set; }


        public string HDDSize
        {
            get
            {
                return this.hddSize;
            }
            set
            {
                this.hddSize = value;
                //NotifyPropertyChanged("hddSize");
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
                return this.mbName;
            }
            set
            {
                this.mbName = value;
                //NotifyPropertyChanged("mbName");
            }
        }

        public string MBSerial { get; set; }

        public string Model { get; set; }

        public string NetbiosName
        {
            get
            {
                return this.netBiosName;
            }
            set
            {
                this.netBiosName = value;
                //NotifyPropertyChanged("netbiosName");
            }
        }


        public string RamSize
        {
            get
            {
                return this.ramSize;
            }
            set
            {
                this.ramSize = value;
                //NotifyPropertyChanged("ramSize");
            }
        }

        public string Serial { get; set; }

        public string TOID { get; set; }

        public string Vendor { get; set; }


    }
}
