using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DSList
{
    public class CustomerLogic : CustomerBase
    {

        /// <summary>
        /// Метод проверки открытия ЦВЗ (Доработать)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private static bool CheckOpened(string item, DateTime time)
        {
            item = item.Replace("Ежедневно", "Пн-Вс");
            char[] separator = new char[] { ',' };
            foreach (string str in item.Split(separator))
            {
                bool flag = true;
                bool flag2 = false;
                char[] chArray2 = new char[] { ' ' };
                string[] strArray2 = str.Trim().Split(chArray2);
                if ((strArray2.Length > 1) && strArray2[1].Contains<char>('('))
                {
                    char[] trimChars = new char[] { '(' };
                    char[] chArray4 = new char[] { ')' };
                    char[] chArray5 = new char[] { '-' };
                    string[] strArray3 = strArray2[1].TrimStart(trimChars).TrimEnd(chArray4).Split(chArray5);
                    flag = time.TimeOfDay.IsBetween(TimeSpan.Parse(strArray3[0]), TimeSpan.Parse(strArray3[1]));
                }
                if (strArray2[0].Contains<char>('-'))
                {
                    char[] chArray6 = new char[] { '-' };
                    string[] strArray4 = strArray2[0].Split(chArray6);
                    flag2 = (Array.IndexOf<string>(Days, strArray4[0]) <= Array.IndexOf<string>(Days, time.ToString("ddd"))) && (Array.IndexOf<string>(Days, strArray4[1]) >= Array.IndexOf<string>(Days, time.ToString("ddd")));
                }
                else
                {
                    flag2 = time.ToString("ddd") == strArray2[0];
                }
                if (flag & flag2)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Метод заполнения свойств основными значениями
        /// </summary>
        public void FillCustomer()
        {
            if (!string.IsNullOrEmpty(NumberCVZ.ToString()))
            {
                char[] numb = new char[3] { '0', '0', '0' };
                char[] temp = NumberCVZ.ToString().ToCharArray();
                for (int i = 0; i < temp.Count(); i++)
                {
                    numb[numb.Count() - 1 - i] = temp[temp.Count() - 1 - i];
                }
                string numbReg = "";
                foreach (var item in numb)
                {
                    numbReg += item;
                }
                Email = $"1{numbReg}@dengisrazy.ru";
                Phone = $"1{numbReg}";
                Provider.IPSEC = IPSECPass;
            }
        }


        /// <summary>
        /// Метод заполнения Customer значениями из словаря, который формируется из странички html
        /// </summary>
        /// <param name="dictionary"></param>
        public void FillCustomerFromDict(Dictionary<string, string> dictionary)
        {
            foreach (var item in dictionary)
            {
                if (item.Key == "Город")
                {
                    City = item.Value;
                }
                if (item.Key == "Адрес")
                {
                    Address = item.Value;
                }
                if (item.Key == "Организация")
                {
                    Organization = item.Value;
                }
                if (item.Key == "Регион")
                {
                    Province = item.Value;
                }
                if (item.Key == "Часовой пояс")
                {
                    Timezone = item.Value;
                }
                if (item.Key == "IP MikroTik-а")
                {
                    Lan_Ip = item.Value;
                }
                if (item.Key == "Тип соединения")
                {
                    Provider.Type = item.Value;
                }
                if (item.Key == "WAN IP")
                {
                    WanIP = item.Value;
                    Provider.ExtIP = item.Value;
                }
                if (item.Key == "WAN маска сети")
                {
                    Provider.ExtSM = item.Value;
                }
                if (item.Key == "WAN шлюз")
                {
                    Provider.ExtGW = item.Value;
                }
                if (item.Key == "WAN пользователь")
                {
                    Provider.Login = item.Value;
                }
                if (item.Key == "WAN пароль")
                {
                    Provider.Password = item.Value;
                }
                if (item.Key == "ICCID")
                {
                    Provider.ICCID = item.Value;
                }
                if (item.Key == "Reserve IP")
                {
                    Provider.ReserveIP = item.Value;
                }
                if (item.Key == "Пароль от IPSEC")
                {
                    Provider.IPSEC = item.Value;
                }
            }
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!this.UpdateBW.IsBusy)
            {
                this.UpdateBW.RunWorkerAsync();
            }
        }

        public void AddStringToBindingList(ObservableCollection<BindingListItem> bindinglist, string description, string value, InfoType grouping = 0)
        {
            if (!string.IsNullOrEmpty(value))
            {
                BindingListItem item = new BindingListItem
                {
                    Grouping = grouping,
                    Description = description,
                    Value = value
                };
                bindinglist.Add(item);
            }
        }


        public void PopulateInfo()
        {
            this.RegionClock = this.Region;
            if (this.timer == null)
            {
                this.timer = new DispatcherTimer(DispatcherPriority.Background);
                this.timer.Interval = TimeSpan.FromSeconds(1.0);
                this.timer.IsEnabled = true;
                this.timer.Tick += new EventHandler(this.timer_Tick);
            }
            this.Info.Clear();
            this.AddStringToBindingList(this.Info, "ЦВЗ №", this.NumberCVZ.ToString(), InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Формат имени", this.FormatName, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "E-mail", this.Email, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Подсеть", this.Lan_Ip, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Статус", this.Status, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Рабочее время", this.WorkTime, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Адрес", this.Address, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Регион", this.Region, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Область", this.Province, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Город", this.City, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Маска подсети", this.SubnetMask, InfoType.Основные);
            this.AddStringToBindingList(this.Info, "Телефон", this.Phone, InfoType.Контакты);
            this.AddStringToBindingList(this.Info, "РД", this.RD, InfoType.Контакты);
            this.AddStringToBindingList(this.Info, "Телефон РД", FormatPhone(this.RDPhone), InfoType.Контакты);
            this.AddStringToBindingList(this.Info, "МРД", this.MRD, InfoType.Контакты);
            this.AddStringToBindingList(this.Info, "Телефон МРД", FormatPhone(this.MRDPhone), InfoType.Контакты);
            this.AddStringToBindingList(this.Info, "Организация", this.Organization, InfoType.Основные);

            //this.AddStringToBindingList(this.Info, "ККТ", this.KKT, InfoType.Основные);
            CollectionView defaultView = (CollectionView)CollectionViewSource.GetDefaultView(this.Info);
            PropertyGroupDescription item = new PropertyGroupDescription("Grouping");
            defaultView.GroupDescriptions.Clear();
            defaultView.GroupDescriptions.Add(item);
            //foreach (ProvInfo info in this.Providers)
            //{

            Provider.Info.Clear();
            //Provider.Info = new ObservableCollection<BindingListItem>();
            this.AddStringToBindingList(Provider.Info, "Название", Provider.ProvName, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "WAN IP", WanIP, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Маска подсети", SubnetMask, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "WAN GateWay", WanGW, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "WAN Mask", WanMask, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Резерв", Provider.ReserveIP, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Телефон техподдержки", Provider.ProvPhone, InfoType.Контакты);
            this.AddStringToBindingList(Provider.Info, "Номер счета", Provider.ContractNum, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "ID канала", Provider.ChID, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Тариф", Provider.Rate, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Логин", WanLogin, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Пароль", WanPass, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Менеджер", Provider.ManagerName, InfoType.Контакты);
            this.AddStringToBindingList(Provider.Info, "Телефон менеджера", Provider.ManagerPhone, InfoType.Контакты);
            this.AddStringToBindingList(Provider.Info, "Почта менеджера", Provider.ManagerEmail, InfoType.Контакты);
            this.AddStringToBindingList(Provider.Info, "Тел. линия", Provider.PhoneLine, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Тип подключения", ConnectType, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "VPI/VCI", Provider.VPIVCI, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Reserve IP", Provider.ReserveIP, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Пароль от IPSEC", IPSECPass, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "ICCID", ICCID, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Комментарий", Provider.Comment, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "Профиль Wi-Fi", Provider.WifiProfile, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "AP провайдера", Provider.WifiProv, InfoType.Основные);
            this.AddStringToBindingList(Provider.Info, "AP Связного", Provider.WifiDS, InfoType.Основные);
            //CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(Provider.Info);
            //view2.GroupDescriptions.Clear();
            //view2.GroupDescriptions.Add(item);
            //}
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            int num = NewMainWindow.settings.Fields.CloseTabTime * 60;
            if (NewMainWindow.settings.Fields.AutoCloseTab && (this.CloseCounter < (num + 1)))
            {
                this.CloseCounter++;
                if (this.CloseCounter == num)
                {
                    //this.OnCloseCounter?.Invoke(this);
                    this.CloseCounter = 0;
                }
            }
            this.UpdateTime();
        }



        private void UpdateTime()
        {
            if (string.IsNullOrEmpty(this.Timezone))
            {
                this.Timezone = "+00:00";
            }
            int num = TimezoneToInt(Timezone);
            //int num = 0;
            string str = (num == 0) ? (" (МСК) ") : ($" (МСК+{num}) ");
            this.RegionClock = this.Region + str + DateTime.UtcNow.AddHours((double)(num + 3)).ToShortTimeString();
            this.TitleColor = (string.IsNullOrWhiteSpace(this.WorkTime) || CheckOpened(this.WorkTime, DateTime.UtcNow.AddHours((double)(num + 3)))) ? SystemColors.HotTrackBrush : Brushes.DarkRed;
        }

        private int TimezoneToInt(string timezone)
        {
            try
            {
                char[] ch = timezone.ToCharArray();
                int timeint = int.Parse(ch[2].ToString()) - 3;
                //string time = ch[2].ToString();
                return timeint;
            }
            catch (Exception)
            {
                return 0;
            }

        }


        

        private int CalculateHostCount(string SubnetMask, int defaultCount = 1)
        {
            if ((SubnetMask == "255.255.255.255") || (SubnetMask == "255.255.255.254"))
            {
                return 1;
            }
            try
            {
                byte[] addressBytes = IPAddress.Parse(SubnetMask).GetAddressBytes();
                int num2 = (((0x100 - addressBytes[0]) * (0x100 - addressBytes[1])) * (0x100 - addressBytes[2])) * (0xfe - addressBytes[3]);
                if (num2 > 0x100)
                {
                    num2 = 0x100;
                    NewMainWindow.ShowBalloonWarning("Количество хостов уменьшено до 256.", "Количество хостов превышено");
                }
                return num2;
            }
            catch
            {
                return defaultCount;
            }
        }


        /// <summary>
        /// Асинхронный метод заполнения IPArray и на его основе выполняется заполнение NetbiosName и Latency
        /// </summary>
        public async void DetectSubnetIP()
        {
            await Task.Run(() =>
            {
                PopulateIPList();
                PingSubnet();
            });
        }


        /// <summary>
        /// Метод изменяет IP адрес на значение delta
        /// </summary>
        /// <param name="RangeFirst">первоначальный IP адрес</param>
        /// <param name="delta">целое число, на которое меняем IP адрес</param>
        /// <returns>возвращает измененный IP адрес</returns>
        private IPAddress ExpandIpRange(IPAddress RangeFirst, int delta)
        {
            uint num = (uint)this.CompressByteArray(RangeFirst.GetAddressBytes());
            return new IPAddress(this.DecompressByteArray(((int)num) + delta));
        }

        public IPAddress GetSubnetAddress(IPAddress ipAddr, int subnet_hosts = 0x20)
        {
            uint ip = (uint)(((CompressByteArray(ipAddr.GetAddressBytes())) / (long)subnet_hosts) * subnet_hosts);
            return new IPAddress(this.DecompressByteArray((int)ip));
        }

        /// <summary>
        /// Метод перевода byte[] в int
        /// </summary>
        /// <param name="bytes">ip адрес в формате byte[]</param>
        /// <returns>возвращает int число, соответствующее входящему ip в виде byte[]</returns>
        private int CompressByteArray(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                throw new FormatException("Wrong IPv4 address");
            }
            int num = 0;
            byte num2 = 0;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                num += bytes[i] << num2;
                num2 = (byte)(num2 + 8);
            }
            return num;
        }



        /// <summary>
        /// Метод перевода ip адреса из int в byte[] формат
        /// </summary>
        /// <param name="ip">входящий ip в int формате</param>
        /// <returns>возвращает ip адрес в byte[] формате</returns>
        private byte[] DecompressByteArray(int ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Заполнение IP листа IPArray
        /// </summary>
        public void PopulateIPList()
        {
            IPAddress address;
            if (string.IsNullOrEmpty(this.SubnetMask))
            {
                this.SubnetMask = "255.255.255.0";
            }
            int num = this.CalculateHostCount(this.SubnetMask, 254);
            try
            {
                string[] curSubnetTemp = Lan_Ip.Split('.');
                curSubnetTemp[3] = "0";
                string curSubnet = "";
                bool firstNumb = true;
                foreach (var str in curSubnetTemp)
                {
                    if (firstNumb)
                    {
                        curSubnet += str;
                        firstNumb = false;
                    }
                    else
                        curSubnet += "." + str;
                }
                address = IPAddress.Parse(curSubnet);
                //address = IPAddress.Parse(NetMikrotik);

            }
            catch
            {
                this.DetectSubnetIP();
                return;
            }

            IPArray = new ObservableCollection<IP>();

            //if (this.Providers != null)
            //{
            //    for (int j = 0; j < this.Providers.Count; j++)
            //    {
            //        try
            //        {
            //            if ((this.Providers[j].ProvName == null) || !this.Providers[j].ProvName.Contains("Wi-Fi"))
            //            {
            //                if ((j > 0))
            //                {
            //                    break;
            //                }
            //                if (!string.IsNullOrEmpty(this.Providers[j].ExtGW))
            //                {
            //                    IP ip = new IP
            //                    {
            //                        Owner = this,
            //                        IPAddress = this.Providers[j].ExtGW,
            //                        Region = this.Region,
            //                        Description = "Шлюз провайдера"
            //                    };
            //                    if (j > 0)
            //                    {
            //                        ip.Description = ip.Description + " #" + j.ToString();
            //                    }
            //                    ip.Type = IPType.Маршрутизатор;
            //                    this.IPArray.Add(ip);
            //                }
            //                if (!string.IsNullOrEmpty(this.Providers[j].ExtIP))
            //                {
            //                    IP ip2 = new IP
            //                    {
            //                        Owner = this,
            //                        IPAddress = this.Providers[j].ExtIP,
            //                        Region = this.Region,
            //                        Description = "Внешний адрес"
            //                    };
            //                    if (j > 0)
            //                    {
            //                        ip2.Description = ip2.Description + " #" + j.ToString();
            //                    }
            //                    ip2.Type = IPType.Маршрутизатор;
            //                    this.IPArray.Add(ip2);
            //                }
            //            }
            //        }
            //        catch
            //        {
            //        }
            //    }
            //}
            string[] strArray = new string[0xfe];   //0xfe - число 254
                                                    //string[] strArray2 = new string[0x7f];  //0x7f - число 127
                                                    //try
                                                    //{
                                                    //    strArray = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"SupportTools\im.dat");
                                                    //}
                                                    //catch
                                                    //{
                                                    //    for (int k = 0; k < strArray.Length; k++)
                                                    //    {
                                                    //        strArray[k] = "unknown";
                                                    //    }
                                                    //}
                                                    //try
                                                    //{
                                                    //    strArray2 = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"support\top.dat");
                                                    //}
                                                    //catch
                                                    //{
                                                    //    for (int m = 0; m < strArray2.Length; m++)
                                                    //    {
                                                    //        strArray2[m] = "unknown";
                                                    //    }
                                                    //}
            int[] source = new int[] { 12, 13, 14 };
            for (int i = 1; i <= num; i++)  // цикл с проходом по общему количеству хостов
            {
                IP ip3 = new IP();
                try
                {
                    ip3.Owner = this;
                    ip3.Region = this.Region;

                    //if (this.SubnetMask.EndsWith(".128"))
                    //{
                    //    ip3.Description = strArray2[i - 1];
                    //    ip3.Type = GetIMIPType(ip3.Description);
                    //}
                    //else
                    //{
                    //    if ((this.SubnetMask.EndsWith(".253") || this.SubnetMask.EndsWith(".254")) || this.SubnetMask.EndsWith(".255"))
                    //    {
                    //        ip3.Address = this.ExpandIpRange(address, i - 1).ToString();
                    //        if (ip3.Address.StartsWith("10.91"))
                    //        {
                    //            ip3.Description = "УТ";
                    //            ip3.Type = IPType.УТ;
                    //        }
                    //        else
                    //        {
                    //            ip3.Description = "АРМ";
                    //            ip3.Type = IPType.АРМ;
                    //        }
                    //        this.IPArray.Add(ip3);
                    //        return;
                    //    }
                    switch (i)
                    {
                        case 0xfe:
                            ip3.Description = "Маршрутизатор";
                            ip3.Type = IPType.Маршрутизатор;
                            if (!string.IsNullOrWhiteSpace(this.Router))
                            {
                                ip3.Tooltip = this.Router;
                            }
                            break;

                        case 101:
                            ip3.Description = "АРМ УТТ";
                            ip3.Type = IPType.АРМ;
                            break;

                        case 102:
                            ip3.Description = "АРМ ПТТ";
                            ip3.Type = IPType.АРМ;
                            break;

                        case 103:
                            ip3.Description = "АРМ 3";
                            ip3.Type = IPType.АРМ;
                            break;

                        case 0xfa:
                            ip3.Description = "Принтер";
                            ip3.Type = IPType.Принтер;
                            break;

                        case 0xfb:
                            ip3.Description = "Камера IP";
                            ip3.Type = IPType.КамераIP;
                            break;

                        case 0xf0:
                            ip3.Description = "Телефон IP";
                            ip3.Type = IPType.ТелефонIP;
                            break;

                        case 0xf1:
                            ip3.Description = "Телефон Сервисный";
                            ip3.Type = IPType.ТелефонСервисный;
                            break;
                        default:
                            ip3.Description = "Прочее";
                            ip3.Type = IPType.Прочее;
                            break;
                    }

                    //}
                }
                catch
                {
                }
                ip3.IPAddress = this.ExpandIpRange(address, i).ToString();
                if ((ip3.Type == IPType.Маршрутизатор) && string.IsNullOrWhiteSpace(this.Gateway))
                {
                    this.Gateway = ip3.IPAddress;
                }
                this.IPArray.Add(ip3);
            }
        }

        /// <summary>
        /// Метод заполнения IPArray значениями на основе принадлежности IP к типу оборудования и родителя Customer
        /// </summary>
        public void DetectListIPType()
        {

            for (int i = 0; i < IPArray.Count; i++)  // цикл с проходом по общему количеству хостов
            {

                IP ip3 = IPArray[i];
                try
                {
                    ip3.Owner = this;
                    if (ip3.Type == IPType.Маршрутизатор)
                    {

                    }
                    else
                    {

                        //ip3.Region = this.Region;
                        switch (Convert.ToInt32(ip3.IPAddress.Remove(0, ip3.IPAddress.Length - 3)))
                        {
                            case 0xfe:
                                ip3.Description = "Маршрутизатор";
                                ip3.Type = IPType.Маршрутизатор;
                                if (!string.IsNullOrWhiteSpace(this.Router))
                                {
                                    ip3.Tooltip = this.Router;
                                }
                                break;

                            case 101:
                                ip3.Description = "АРМ УТТ";
                                ip3.Type = IPType.АРМ;
                                break;

                            case 102:
                                ip3.Description = "АРМ ПТТ";
                                ip3.Type = IPType.АРМ;
                                break;

                            case 103:
                                ip3.Description = "АРМ 3";
                                ip3.Type = IPType.АРМ;
                                break;

                            case 0xfa:
                                ip3.Description = "Основной";
                                ip3.Type = IPType.Принтер;
                                break;

                            case 0xfb:
                                ip3.Description = "Основной";
                                ip3.Type = IPType.КамераIP;
                                break;

                            case 0xf0:
                                ip3.Description = "Основной";
                                ip3.Type = IPType.ТелефонIP;
                                break;

                            case 0xf1:
                                ip3.Description = "Основной";
                                ip3.Type = IPType.ТелефонСервисный;
                                break;
                            default:
                                ip3.Description = "Прочее";
                                ip3.Type = IPType.Прочее;
                                break;
                        }
                    }
                }
                catch
                {
                }

                if ((ip3.Type == IPType.Маршрутизатор) && string.IsNullOrWhiteSpace(this.Gateway))
                {
                    this.Gateway = ip3.IPAddress;
                }

            }
        }


        /// <summary>
        /// Метод заполнения массива IPArray значениями NetbiosName и Latency
        /// </summary>
        public void PingSubnet()
        {
            this.CloseCounter = 0;
            if (this.Lan_Ip != "0.0.0.0")
            {
                Task.Run(delegate
                {
                    foreach (IP ip in this.IPArray)
                    {
                        if (NewMainWindow.settings.Fields.PingDelay)
                        {
                            Thread.Sleep(15);
                        }
                        ip.Ping(true);
                    }
                });
            }
        }

        /// <summary>
        /// Метод форматирования номера телефона
        /// </summary>
        /// <param name="phone">телефонный номер в строковом представлении</param>
        /// <returns>возвращает отформатированное представление телефонного номера</returns>
        public static string FormatPhone(string phone)
        {

            try
            {
                if (((phone[0] == '+') && (phone[1] == '7')) && (phone.Length == 12))
                {
                    string[] textArray1 = new string[] { "+7 (", phone.Substring(2, 3), ") ", phone.Substring(5, 3), " ", phone.Substring(8, 2), " ", phone.Substring(10, 2) };
                    phone = string.Concat(textArray1);
                }
                if ((phone[0] == '8') && (phone.Length == 11))
                {
                    string[] textArray2 = new string[] { "+7 (", phone.Substring(1, 3), ") ", phone.Substring(4, 3), " ", phone.Substring(7, 2), " ", phone.Substring(9, 2) };
                    phone = string.Concat(textArray2);
                }
                if ((phone[0] == '9') && (phone.Length == 10))
                {
                    string[] textArray3 = new string[] { "+7 (", phone.Substring(0, 3), ") ", phone.Substring(3, 3), " ", phone.Substring(6, 2), " ", phone.Substring(8, 2) };
                    phone = string.Concat(textArray3);
                }
            }
            catch
            {
            }
            return phone;

        }


        /// <summary>
        /// Метод заполнения свойства IPArray стандартными ip адресами ЦВЗ
        /// </summary>
        public void CreateStandartListIP()
        {
            IPArray = new ObservableCollection<IP>();
            string armIP = Lan_Ip.Remove(Lan_Ip.Length - 4);
            IPArray.Add(new IP() { IPAddress = $"{this.WanIP}", Type = IPType.Маршрутизатор });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.101" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.102" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.103" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.240" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.241" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.250" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.251" });
            IPArray.Add(new IP() { IPAddress = $"{armIP}.254" });
        }


    }
}
