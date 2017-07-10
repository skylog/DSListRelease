using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    public class CustomerAuxiliaryPaneItem
    {
        private int? _NumberCVZ;

        public CustomerAuxiliaryPaneItem()
        {

        }

        public CustomerAuxiliaryPaneItem(NewMainWindow owner)
        {
            Owner = owner;
        }

        public NewMainWindow Owner { get; set; }
        public int? NumberCVZ
        {
            get { return _NumberCVZ; }
            set { _NumberCVZ = value; }
        }
        public Customer AuxCustomer { get; set; }

        public IPAddress IPAddress { get; set; }

        public string Text { get; set; }

        //public Customer GetCustomer()
        //{
        //    MethodsTableFromSite mmf = new MethodsTableFromSite();
            
        //    Customer curCustomer = mmf.GetCVZFromSearchList(AuxCustomer.NetMikrotik);

        //    curCustomer.CreateStandartListIP();
        //    curCustomer.DetectListIPType();
        //    curCustomer.PingSubnet();
        //    curCustomer.FillCustomer();

        //    curCustomer.Owner = Owner;
        //    curCustomer.PopulateInfo();
        //    curCustomer.CreateCustomerWithContentAndHeader();
        //    //curCustomer.PopulateInfo();
        //    //ContextMenuCreate();
        //    //curCustomer.ContextMenu = customerListContextMenu;
        //    TabCtrl.Items.Add(curCustomer);
        //    RibbonMenuAuxiliaryPane.Items.Add(curCustomer.ToStringDisplay);
        //    Log($"Открыта {curCustomer.ToStringDisplay}");
        //    curCustomer.Focus();
        //}

        public override string ToString()
        {
            if (_NumberCVZ != null)
            {
                return Text;
            }
            else
            {
                return IPAddress.ToString();
            }

        }
    }
}
