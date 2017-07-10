using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    class IPAndName
    {
        #region Поля
        string pcIP;
        string pcName;
        string group;

        #endregion

        #region Свойства
        public string Group
        {
            get { return group; }
            set { group = value; }
        }
        public string PcIP
        {
            get { return pcIP; }
            set { pcIP = value; }
        }

        public string PcName
        {
            get { return pcName; }
            set { pcName = value; }
        }

        public string Mac { get; set; }
        #endregion
        public IPAndName()
        {

        }

        public IPAndName(string curPcIp, string curGroup, string curPcName, string mac)
        {
            pcIP = curPcIp;
            pcName = curPcName;
            group = curGroup;
            Mac = mac;
        }
    }
}
