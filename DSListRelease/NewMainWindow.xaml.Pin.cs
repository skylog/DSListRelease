using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using AngleSharp;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using TaskDialogInterop;

namespace DSList
{
    public partial class NewMainWindow
    {
        private static ObservableCollection<PinnedTT> PinnedTTs { get; set; }
        private string PinnedTTs_xmlfile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DSList\PinnedTTs.xml");
        private DispatcherTimer PinnedWatchTimer = new DispatcherTimer();

        private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).DataContext = Bindings;
        }

        /// <summary>
        /// Метод обработки нажатия кнопки закрепления Cusotmer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            this.PinSelectedTT();
        }

        /// <summary>
        /// Метод обработки нажатия кнопки "Enter" в поле комментария
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinComment_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.CheckAllWindowHotKey(ref e) && (e.Key == Key.Return))
            {
                btnPin.Focus();
                //Pin_Click(sender, new RoutedEventArgs());
                //PinSelectedTT();
                //Dispatcher.Invoke((RoutedEventHandler) ((object x, RoutedEventArgs y) => { Pin_Click(x, y); }));
            }
        }

        /// <summary>
        /// Метод обработки нажатия левой кнопкой по закрепленному Customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pinned_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ListViewPinned.SelectedItem != null)
            {
                Customer selected = this.FindTTbyCode((this.ListViewPinned.SelectedItem as PinnedTT).Code, AllTT);
                if ((this.ListViewPinned.SelectedIndex != -1) && (selected != null))
                {
                    selected.Comment = (this.ListViewPinned.SelectedItem as PinnedTT).Comment;
                    try
                    {
                        // Проверка существования данного customer в ранее открытых
                        Customer getCustomer = FindTTbyCode(selected.NumberCVZ.ToString(), OpenedTT);
                        if (getCustomer != null)
                        {
                            foreach (var item in Tabs.Items)
                            {
                                if (item == getCustomer)
                                {
                                    Tabs.SelectedItem = item;
                                }
                            };
                        }
                        else
                        {
                            Customer curCustomer;
                            //if (selected.Content == null)
                            //{
                                curCustomer = CreateCustomerForType(CustomerTypes.TTCVZ, selected);
                            //}
                            //else
                            curCustomer = selected;
                            OpenTTinTab(curCustomer, true);
                            Log($"Открыта {curCustomer.ToStringDisplay}");
                            //curCustomer.Focus();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Ошибка при выводе закрепленной ЦВЗ. " + ex.Message, true, true, ex.StackTrace);
                    }
                }
                else
                {
                    SearchTimer.Stop();



                    // Проверка существования данного customer в ранее открытых
                    var getTab = (from i in OpenedTT where i.Lan_Ip == (ListViewPinned.SelectedItem as PinnedTT).IPAddress select i).FirstOrDefault();
                    if (getTab != null)
                        /*getTab.Focus()*/;
                    else
                    {
                        //IPAddress address;
                        //this.TextBoxSearch.Text = (this.ListViewPinned.SelectedItem as PinnedTT).Code;
                        //this.TextBoxSearch.Focus();
                        //if (IPAddress.TryParse(this.TextBoxSearch.Text, out address))
                        //{
                        //    this.OpenCustomTT();
                        //}
                        Customer curCustomer = CreateCustomerForType(CustomerTypes.SingleIP, new Customer(this) /*{ Owner = this }*/, (this.ListViewPinned.SelectedItem as PinnedTT).IPAddress);
                        OpenTTinTab(curCustomer);
                        //OpenedTT.Add(curCustomer);
                        Log($"Открыт {curCustomer.Lan_Ip}");
                        //curCustomer.Focus();
                    }


                    //IPAddress address;
                    //this.TextBoxSearch.Text = (this.ListViewPinned.SelectedItem as PinnedTT).Code;
                    //this.TextBoxSearch.Focus();
                    //if (IPAddress.TryParse(this.TextBoxSearch.Text, out address))
                    //{
                    //    this.OpenCustomTT();
                    //}
                }
            }
        }

        private void OpenCustomTT()
        {
            //< OpenCustomTT > d__327 stateMachine = new < OpenCustomTT > d__327 {
            //    <> 4__this = this,
            //    <> t__builder = AsyncVoidMethodBuilder.Create(),
            //    <> 1__state = -1
            //      };
            //stateMachine.<> t__builder.Start << OpenCustomTT > d__327 > (ref stateMachine);

            //    private sealed class <OpenCustomTT>d__327 : IAsyncStateMachine
            //{
            //    public int <>1__state;
            //    public MainWindow<>4__this;
            //    private MainWindow.<>c__DisplayClass327_0<>8__1;
            //    public AsyncVoidMethodBuilder<> t__builder;
            //private TaskAwaiter<> u__1;
            //private string <addr>5__4;
            //    private string[] <allowed_preffixes_sm>5__3;
            //    private string[] <allowed_preffixes>5__2;
            //    private Customer<CustomTT>5__5;
            //    private Exception<ex>5__6;

            //    private void MoveNext()
            //{
            //    Exception exception;
            //    int num = this.<> 1__state;
            //    try
            //    {
            //        if (num == 0)
            //        {
            //        }
            //        try
            //        {
            //            TaskAwaiter awaiter;
            //            if (num != 0)
            //            {
            //                this.<> 8__1 = new MainWindow.<> c__DisplayClass327_0();
            //                this.< allowed_preffixes > 5__2 = new string[] { "/24", "/25", "/26", "/27", "/28", "/29", "/30", "/31", "/32" };
            //                this.< allowed_preffixes_sm > 5__3 = new string[] { "0", "128", "192", "224", "238", "246", "250", "252", "253" };
            //                char[] trimChars = new char[] { '/' };
            //                this.<> 8__1.AddressString = this.<> 4__this.TextBoxSearch.Text.Trim().TrimStart(new char[] { 'h', 't', 'p', 's', ':', '/', '\\' }).TrimEnd(trimChars);
            //                if (this.<> 8__1.AddressString.Contains("/"))
            //                    {
            //                    this.<> 8__1.AddressString = this.<> 8__1.AddressString.Remove(this.<> 8__1.AddressString.IndexOf('/'));
            //                }
            //                IPAddress.TryParse(this.<> 8__1.AddressString, out this.<> 8__1.ipaddr);
            //                if (this.<> 8__1.ipaddr == null)
            //                    {
            //                    awaiter = Task.Run(new Action(this.<> 8__1.< OpenCustomTT > b__0)).GetAwaiter();
            //                    if (!awaiter.IsCompleted)
            //                    {
            //                        this.<> 1__state = num = 0;
            //                        this.<> u__1 = awaiter;
            //                        MainWindow.< OpenCustomTT > d__327 stateMachine = this;
            //                        this.<> t__builder.AwaitUnsafeOnCompleted < TaskAwaiter, MainWindow.< OpenCustomTT > d__327 > (ref awaiter, ref stateMachine);
            //                        return;
            //                    }
            //                    goto Label_01F3;
            //                }
            //                goto Label_0205;
            //            }
            //            awaiter = this.<> u__1;
            //            this.<> u__1 = new TaskAwaiter();
            //            this.<> 1__state = num = -1;
            //            Label_01F3:
            //            awaiter.GetResult();
            //            awaiter = new TaskAwaiter();
            //            goto Label_0242;
            //            Label_0205:;
            //            if (this.<> 8__1.AddressString.Count<char>((MainWindow.<> c.<> 9__327_1 ?? (MainWindow.<> c.<> 9__327_1 = new Func<char, bool>(MainWindow.<> c.<> 9.< OpenCustomTT > b__327_1)))) < 3)
            //                {
            //                goto Label_038E;
            //            }
            //            Label_0242:
            //            if (this.<> 8__1.ipaddr == null)
            //                {
            //                throw new Exception("Неверный IP адрес.");
            //            }
            //            this.< addr > 5__4 = this.<> 4__this.GetSubnetAddress(this.<> 8__1.ipaddr, 0xfe - int.Parse(this.< allowed_preffixes_sm > 5__3[8])).ToString();
            //            this.< CustomTT > 5__5 = new Customer();
            //            this.< CustomTT > 5__5.Code = this.< addr > 5__4;
            //            this.< CustomTT > 5__5.SubnetMask = "255.255.255." + this.< allowed_preffixes_sm > 5__3[8];
            //            this.< CustomTT > 5__5.Name = "Подсеть " + this.< addr > 5__4 + this.< allowed_preffixes > 5__2[8];
            //            this.< CustomTT > 5__5.IPAddr = this.< addr > 5__4;
            //            this.<> 4__this.OpenTTinTab(this.< CustomTT > 5__5, false);
            //            this.<> 8__1 = null;
            //            this.< allowed_preffixes > 5__2 = null;
            //            this.< allowed_preffixes_sm > 5__3 = null;
            //            this.< addr > 5__4 = null;
            //            this.< CustomTT > 5__5 = null;
            //        }
            //        catch (Exception exception1)
            //        {
            //            exception = exception1;
            //            this.< ex > 5__6 = exception;
            //            this.<> 4__this.Log(this.< ex > 5__6.Message, true, false, this.< ex > 5__6.StackTrace, false);
            //        }
            //    }
            //    catch (Exception exception2)
            //    {
            //        exception = exception2;
            //        this.<> 1__state = -2;
            //        this.<> t__builder.SetException(exception);
            //        return;
            //    }
            //    Label_038E:
            //    this.<> 1__state = -2;
            //    this.<> t__builder.SetResult();
            //}
        }

        private void Pinned_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ListViewPinned.SelectedIndex != -1)
            {
                this.MenuOpen("ListViewPinnedMenu", sender as FrameworkElement);
            }
        }

        private void ClearPinned_Click(object sender, RoutedEventArgs e)
        {
            PinnedTTs.Clear();
        }

        private void Unpin_Click(object sender, RoutedEventArgs e)
        {
            this.UnpinSelected();
        }

        private void UnpinSelected()
        {
            if (this.ListViewPinned.SelectedIndex != -1)
            {
                PinnedTTs.Remove(this.ListViewPinned.SelectedItem as PinnedTT);
            }
            this.SavePinnedTTs();
        }

        private void UnpinTT(Customer tt, bool save = true)
        {
            for (int i = PinnedTTs.Count - 1; i >= 0; i--)
            {
                if (PinnedTTs[i].Code == tt.NumberCVZ.ToString() || (PinnedTTs[i].IPAddress == tt.Lan_Ip && PinnedTTs[i].Code == "0"))
                {
                    PinnedTTs.RemoveAt(i);
                }
            }
            this.SavePinnedTTs();
        }

        private void SavePinnedTTs()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<PinnedTT>));
                StreamWriter writer = new StreamWriter(this.PinnedTTs_xmlfile);
                serializer.Serialize((TextWriter)writer, PinnedTTs);
                writer.Close();
            }
            catch
            {
            }
        }

        private void LoadPinnedTTs()
        {
            try
            {
                using (StreamReader reader = new StreamReader(this.PinnedTTs_xmlfile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<PinnedTT>));
                    PinnedTTs = serializer.Deserialize(reader) as ObservableCollection<PinnedTT>;
                }
            }
            catch
            {
            }
        }

        private void PinSelectedTT()
        {
            try
            {
                if (Bindings.isttselected)
                {
                    int num = int.Parse((this.NotifyMinutesComboBox.SelectedItem as ComboBoxItem).Tag.ToString());
                    DateTime time = DateTime.Now.AddMinutes((double)num);
                    PinnedTT item = new PinnedTT();
                    if (string.IsNullOrWhiteSpace(SelectedTT.Comment))
                    {
                        SelectedTT.Comment = $"{SelectedTT.Region} {SelectedTT.ToStringDisplay}";
                    }
                    SelectedTT.Comment = $"[{time.ToShortTimeString()}] {SelectedTT.Comment}";
                    this.UnpinTT(SelectedTT, false);
                    //item.Code = SelectedTT.NumberCVZ.ToString();
                    item.Code = SelectedTT.NumberCVZ.ToString() /*!= "0" ? SelectedTT.NumberCVZ.ToString() : SelectedTT.Lan_Ip*/;
                    if (SelectedTT.NumberCVZ.ToString() == "0")
                    {
                        item.IPAddress = SelectedTT.Lan_Ip;
                    }
                    item.Name = $"{SelectedTT.ToStringDisplay} {SelectedTT.Lan_Ip}";
                    item.Comment = SelectedTT.Comment;
                    item.Color = Bindings.brush.ToString();
                    if (num > 0)
                    {
                        item.NotifyAt = time;
                    }
                    PinnedTTs.Add(item);
                    this.PinnedWatchTimer.Stop();
                    PinnedTTs = new ObservableCollection<PinnedTT>(from i in PinnedTTs
                                                                   orderby i.NotifyAt.ToString()
                                                                   select i);
                    this.ListViewPinned.ItemsSource = PinnedTTs;
                    this.PinnedWatchTimer.Start();
                    this.SavePinnedTTs();
                }
            }
            catch (Exception exception)
            {
                this.Error(exception.Message, "Ошибка при закреплении ТТ", exception.StackTrace);
            }
        }

        private void PinnedWatchTimer_Tick(object sender, EventArgs e)
        {
            foreach (PinnedTT dtt in PinnedTTs)
            {
                if ((dtt.NotifyAt != DateTime.MinValue) && (DateTime.Now >= dtt.NotifyAt))
                {
                    this.ActivateMainWindow();
                    dtt.NotifyAt = DateTime.MinValue;
                    ShowBalloonWarning(dtt.Comment, $"{dtt.Code} - {dtt.Name}");
                    TaskDialogOptions options = new TaskDialogOptions
                    {
                        Owner = this,
                        Title = "DSList",
                        Content = dtt.Comment,
                        MainInstruction = $"{dtt.Code} - {dtt.Name}",
                        MainIcon = VistaTaskDialogIcon.Warning
                    };
                    options.CustomButtons = new string[] { "ОК", "Открепить ТТ", "Открыть ТТ", "Отложить" };
                    options.DefaultButtonIndex = 0;
                    options.AllowDialogCancellation = true;
                    TaskDialogResult result = TaskDialog.Show(options);
                    int? customButtonResult = result.CustomButtonResult;
                    int num = 1;
                    if ((customButtonResult.GetValueOrDefault() == num) ? customButtonResult.HasValue : false)
                    {
                        Customer tt = this.FindTTbyCode(dtt.Code, this.AllTT);
                        if (tt == null)
                        {
                            tt = new Customer(this)
                            {
                                NumberCVZ = int.Parse(dtt.Code),
                                //Owner = this
                            };
                        }
                        this.UnpinTT(tt, true);
                        this.PinnedWatchTimer_Tick(sender, e);
                        break;
                    }
                    customButtonResult = result.CustomButtonResult;
                    num = 2;
                    if ((customButtonResult.GetValueOrDefault() == num) ? customButtonResult.HasValue : false)
                    {
                        if (dtt.Code == "0" && dtt.IPAddress != null)
                        {
                            SearchTimer.Stop();
                            // Проверка существования данного customer в ранее открытых
                            var getTab = (from i in OpenedTT where i.Lan_Ip == dtt.IPAddress select i).FirstOrDefault();
                            if (getTab != null)
                                /*getTab.Focus()*/;
                            else
                            {
                                //this.TextBoxSearch.Text = (this.ListViewPinned.SelectedItem as PinnedTT).Code;
                                //this.TextBoxSearch.Focus();
                                Customer curCustomer = CreateCustomerForType(CustomerTypes.SingleIP, new Customer(this) /*{ Owner = this }*/, dtt.IPAddress);
                                OpenTTinTab(curCustomer);
                                //OpenedTT.Add(curCustomer);
                                Log($"Открыт {curCustomer.Lan_Ip}");
                                /*curCustomer.Focus()*/;
                            }
                        }
                        Customer selected = this.FindTTbyCode(dtt.Code, this.AllTT);
                        if (selected != null)
                        {
                            selected.Comment = dtt.Comment;
                            Customer curCustomer = CreateCustomerForType(CustomerTypes.TTCVZ, selected);
                            OpenTTinTab(selected, true);
                            //if (selected.NumberCVZ.ToString()!="0")
                            //{
                            //    Customer curCustomer = CreateCustomerForType(CustomerTypes.TTCVZ, selected);
                            //    this.OpenTTinTab(selected, true);
                            //}
                            //else
                            //{
                            //    Customer curCustomer = CreateCustomerForType(CustomerTypes.SingleIP, selected);
                            //    this.OpenTTinTab(selected, true);
                            //}

                        }
                    }
                    customButtonResult = result.CustomButtonResult;
                    num = 3;
                    if ((customButtonResult.GetValueOrDefault() == num) ? customButtonResult.HasValue : false)
                    {
                        options = new TaskDialogOptions
                        {
                            Owner = this,
                            Title = "DSList",
                            Content = dtt.Comment,
                            MainInstruction = $"Отложить {dtt.Code} - {dtt.Name}"
                        };
                        options.CustomButtons = new string[] { "Отмена" };
                        options.DefaultButtonIndex = 0;
                        options.AllowDialogCancellation = true;
                        List<string> list = new List<string>();
                        for (int i = 1; i < this.NotifyMinutesComboBox.Items.Count; i++)
                        {
                            list.Add((this.NotifyMinutesComboBox.Items[i] as ComboBoxItem).Content.ToString());
                        }
                        options.CommandButtons = list.ToArray();
                        TaskDialogResult result2 = TaskDialog.Show(options);
                        if (!result2.CommandButtonResult.HasValue)
                        {
                            break;
                        }
                        int num2 = int.Parse((this.NotifyMinutesComboBox.Items[result2.CommandButtonResult.Value + 1] as ComboBoxItem).Tag.ToString());
                        DateTime time = DateTime.Now.AddMinutes((double)num2);
                        dtt.NotifyAt = time;
                    }
                }
            }
        }

    }
}
