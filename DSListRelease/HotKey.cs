using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSList
{
    public class Hotkey : ModelBase
    {
        private string _Command;
        private bool _Control = false;
        private Key _HotKey;
        private bool _Shift = false;

        public string Command
        {
            get
            { return _Command; }
            set
            {
                _Command = value;
                NotifyPropertyChanged("Command");
            }
        }

        public bool Control
        {
            get
            {
                return this._Control;
            }
            set
            {
                this._Control = value;
                base.NotifyPropertyChanged("Control");
                base.NotifyPropertyChanged("HotKeyString");
            }
        }

        public Key HotKey
        {
            get
            {
                return this._HotKey;
            }
            set
            {
                this._HotKey = value;
                base.NotifyPropertyChanged("HotKey");
                base.NotifyPropertyChanged("HotKeyString");
            }
        }

        public string HotKeyString
        {
            get
            {
                string str = string.Empty;
                if (this.Control)
                {
                    str = str + "Ctrl+";
                }
                if (this.Shift)
                {
                    str = str + "Shift+";
                }
                return (str + this.HotKey.ToString());
            }
        }

        public bool Shift
        {
            get
            {
                return
               this._Shift;
            }
            set
            {
                this._Shift = value;
                base.NotifyPropertyChanged("Shift");
                base.NotifyPropertyChanged("HotKeyString");
            }
        }
    }
}
