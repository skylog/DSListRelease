using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    public class ProvInfo : ModelBase
    {
        private ObservableCollection<BindingListItem> _Info;

        public string ChID { get; set; }

        public string Comment { get; set; }

        public string ContractNum { get; set; }

        public string ExtGW { get; set; }

        public string ExtIP { get; set; }
        

        public string ExtSM { get; set; }

        public ObservableCollection<BindingListItem> Info
        {
            get
            {
                return this._Info;
            }
            set
            {
                this._Info = value;
                base.NotifyPropertyChanged("Info");
            }
        }

        public string ICCID { get; set; }
        public string IPSEC { get; set; }
        
        public string Login { get; set; }

        public string ManagerEmail { get; set; }

        public string ManagerName { get; set; }

        public string ManagerPhone { get; set; }

        public string Password { get; set; }

        public string PhoneLine { get; set; }

        public string ProvName { get; set; }

        public string ProvPhone { get; set; }

        public string Rate { get; set; }

        public string ReserveIP { get; set; }

        public string Type { get; set; }

        public string VPIVCI { get; set; }

        public string WifiProfile { get; set; }

        public string WifiProv { get; set; }

        public string WifiDS { get; set; }
    }
}
