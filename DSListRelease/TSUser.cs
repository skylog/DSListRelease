using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    class TSUser
    {
        private string _UserName;
        private string _PCName;
        private string _TSName;
        private string _DomainName;
        private string _ClientDisplay;
        private string _ClientIPAddress;
        private string _ConnectionState;
        private string _UserAccount;
        private string _WindowStationName;
        private string _RusName;
        private string _RusFamily;
        private string _RusMiddleName;

        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (value != null)
                    _UserName = value;
                else
                    _UserName = "";
            }
        }
        public string PCName
        {
            get
            {
                return _PCName;
            }
            set
            {
                if (value != null)
                    _PCName = value;
                else
                    _PCName = "";
            }
        }
        public string TSName
        {
            get
            {
                return _TSName;
            }
            set
            {
                if (value != null)
                    _TSName = value;
                else
                    _TSName = "";
            }
        }
        public string DomainName
        {
            get
            {
                return _DomainName;
            }
            set
            {
                if (value != null)
                    _DomainName = value;
                else
                    _DomainName = "";
            }
        }
        public string ClientDisplay
        {
            get
            {
                return _ClientDisplay;
            }
            set
            {
                if (value != null)
                    _ClientDisplay = value;
                else
                    _ClientDisplay = "";
            }
        }
        public string ClientIPAddress
        {
            get
            {
                return _ClientIPAddress;
            }
            set
            {
                if (value != null)
                    _ClientIPAddress = value;
                else
                    _ClientIPAddress = "";
            }
        }
        public string ConnectionState
        {
            get
            {
                return _ConnectionState;
            }
            set
            {
                if (value != null)
                    _ConnectionState = value;
                else
                    _ConnectionState = "";
            }
        }
        public string UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                if (value != null)
                    _UserAccount = value;
                else
                    _UserAccount = "";
            }
        }
        public string WindowStationName
        {
            get
            {
                return _WindowStationName;
            }
            set
            {
                if (value != null)
                    _WindowStationName = value;
                else
                    _WindowStationName = "";
            }
        }

        public string RusName
        {
            get
            {
                return _RusName;
            }
            set
            {
                if (value != null)
                    _RusName = value;
                else
                    _RusName = "";
            }
        }

        public string RusFamily
        {
            get
            {
                return _RusFamily;
            }
            set
            {
                if (value != null)
                    _RusFamily = value;
                else
                    _RusFamily = "";
            }
        }

        public string RusMiddleName
        {
            get
            {
                return _RusMiddleName;
            }
            set
            {
                if (value != null)
                    _RusMiddleName = value;
                else
                    _RusMiddleName = "";
            }
        }

        public override string ToString()
        {
            return $"{UserName} {PCName}";
        }

        public TSUser()
        {
            UserName = "";
            PCName = "";
            TSName = "";
            ClientIPAddress = "";
            RusName = "";
            RusFamily = "";
            RusMiddleName = "";
        }
    }
}
