using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    class ADUser
    {
        private string _WhenCreate;
        private string _AccountName;
        private string _Email;
        private string _MobilePhone;
        private string _TelephoneNumber;
        private string _Family;
        private string _NameUser;
        private string _MiddleName;
        private string _Departament;
        private string _Position;

        public string WhenCreate
        {
            get
            {
                return _WhenCreate;
            }
            set
            {
                if (value != null)
                    _WhenCreate = value;
                else
                    _WhenCreate = "";
            }
        }

        public string AccountName
        {
            get
            {
                return _AccountName;
            }
            set
            {
                if (value != null)
                    _AccountName = value;
                else
                    _AccountName = "";
            }
        }
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                if (value != null)
                    _Email = value;
                else
                    _Email = "";
            }
        }
        public string MobilePhone
        {
            get
            {
                return _MobilePhone;
            }
            set
            {
                if (value != null)
                    _MobilePhone = value;
                else
                    _MobilePhone = "";
            }
        }
        public string TelephoneNumber
        {
            get
            {
                return _TelephoneNumber;
            }
            set
            {
                if (value != null)
                    _TelephoneNumber = value;
                else
                    _TelephoneNumber = "";
            }
        }
        public string Family
        {
            get
            {
                return _Family;
            }
            set
            {
                if (value != null)
                    _Family = value;
                else
                    _Family = "";
            }
        }
        public string NameUser
        {
            get
            {
                return _NameUser;
            }
            set
            {
                if (value != null)
                    _NameUser = value;
                else
                    _NameUser = "";
            }
        }
        public string MiddleName
        {
            get
            {
                return _MiddleName;
            }
            set
            {
                if (value != null)
                    _MiddleName = value;
                else
                    _MiddleName = "";
            }
        }
        public string Departament
        {
            get
            {
                return _Departament;
            }
            set
            {
                if (value != null)
                    _Departament = value;
                else
                    _Departament = "";
            }
        }
        public string Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (value != null)
                    _Position = value;
                else
                    _Position = "";
            }
        }

        public ADUser()
        {
            //WhenCreate = "";
            //AccountName = "";
            //Email = "";
            //MobilePhone = "";
            //TelephoneNumber = "";
            //Family = "";
            //NameUser = "";
            //MiddleName = "";
            //Departament = "";
            //Position = "";
        }

        /// <summary>
        /// Метод форматирования номера телефона
        /// </summary>
        /// <param name="phone">телефонный номер в строковом представлении</param>
        /// <returns>возвращает отформатированное представление телефонного номера</returns>
        public static string FormatPhone(string phone)
        {
            StringBuilder newphone = new StringBuilder(phone);
            try
            {

                newphone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                //phone = phone.Replace(" ", "");
                //phone = phone.Replace("(", "");
                //phone = phone.Replace(")", "");
                //phone = phone.Replace("-", "");

                if (((newphone[0] == '+') && (newphone[1] == '7')) && (newphone.Length == 12))
                {
                    //+7 (909) 404 72 82
                    //StringBuilder[] textArray11 = new StringBuilder[] { "+7 (", phone.Substring(2, 3), ") ", phone.Substring(5, 3), " ", phone.Substring(8, 2), " ", phone.Substring(10, 2) };
                    StringBuilder textarray11 = new StringBuilder();
                    //phone = newphone.ToString();
                    newphone = textarray11.Append("+7 (").Append(newphone.ToString().Substring(2, 3)).Append(") ").Append(newphone.ToString().Substring(5, 3)).Append(" ").Append(newphone.ToString().Substring(8, 2)).Append(" ").Append(newphone.ToString().Substring(10, 2));



                    //newphone =  newphone.Append(" (", 2,0);
                    //textarray11.Append(newphone.AppendFormat())
                    //newphone.Append(") ", 7, 1);
                    //newphone.Append(" ", 12, 1);
                    //newphone.Append(" ",15,1);
                    //string[] textArray1 = new string[] { "+7 (", phone.Substring(2, 3), ") ", phone.Substring(5, 3), " ", phone.Substring(8, 2), " ", phone.Substring(10, 2) };
                    //phone = string.Concat(textArray1);
                    //newphone = textarray11;
                }
                if ((newphone[0] == '8') && (newphone.Length == 11))
                {
                    StringBuilder textarray12 = new StringBuilder();
                    //phone = newphone.ToString();
                    newphone = textarray12.Append("+7 (").Append(newphone.ToString().Substring(1, 3)).Append(") ").Append(newphone.ToString().Substring(4, 3)).Append(" ").Append(newphone.ToString().Substring(7, 2)).Append(" ").Append(newphone.ToString().Substring(9, 2));
                    //phone = string.Concat(textArray2);
                }
                if ((newphone[0] == '9') && (newphone.Length == 10))
                {
                    StringBuilder textarray13 = new StringBuilder();
                    //phone = newphone.ToString();
                    newphone = textarray13.Append("+7 (").Append(newphone.ToString().Substring(0, 3)).Append(") ").Append(newphone.ToString().Substring(3, 3)).Append(" ").Append(newphone.ToString().Substring(6, 2)).Append(" ").Append(newphone.ToString().Substring(8, 2));
                    //phone = string.Concat(textArray3);
                }
            }
            catch
            {
            }
            return newphone.ToString();

        }

        public override string ToString()
        {
            return $"{Family} {NameUser} {MiddleName} {AccountName}";
        }
    }
}
