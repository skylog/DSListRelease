using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    public class MRUTT : ModelBase
    {
        private string _Code = string.Empty;
        private string _Name = string.Empty;
        private string _Time = string.Empty;

        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this._Code = value;
                base.NotifyPropertyChanged("Code");
            }
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
                base.NotifyPropertyChanged("Name");
            }
        }

        public string Time
        {
            get
            {
                return this._Time;
            }
            set
            {
                this._Time = value;
                base.NotifyPropertyChanged("Time");
            }
        }
    }
}
