using System;
using System.Windows.Media;

namespace DSList
{
    public class PinnedTT : ModelBase
    {
        private string _Code = string.Empty;
        private Brush _Color;
        private string _Comment = string.Empty;
        private string _Name = string.Empty;
        private DateTime _NotifyAt = DateTime.MinValue;
        private string _IPAddress;

        public string Code
        {
            get
            {
                return
               this._Code;
            }
            set
            {
                this._Code = value;
                base.NotifyPropertyChanged("Code");
            }
        }

        public string IPAddress
        {
            get
            {
                return
               this._IPAddress;
            }
            set
            {
                this._IPAddress = value;
                base.NotifyPropertyChanged("IPAddress");
            }
        }

        public string Color
        {
            get
            {
                return
               this._Color.ToString();
            }
            set
            {
                this._Color = (Brush)new BrushConverter().ConvertFromString(value);
                base.NotifyPropertyChanged("Color");
            }
        }

        public string Comment
        {
            get
            {
                return
               this._Comment;
            }
            set
            {
                this._Comment = value;
                base.NotifyPropertyChanged("Comment");
            }
        }

        public string Name
        {
            get
            {
                return
               this._Name;
            }
            set
            {
                this._Name = value;
                base.NotifyPropertyChanged("Name");
            }
        }

        public DateTime NotifyAt
        {
            get
            {
                return
               this._NotifyAt;
            }
            set
            {
                this._NotifyAt = value;
                base.NotifyPropertyChanged("NotifyAt");
            }
        }
    }
}
