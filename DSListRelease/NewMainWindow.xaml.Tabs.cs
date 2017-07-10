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
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using TaskDialogInterop;
using System;
using System.Linq;
using System.Text;
using Renci.SshNet;

namespace DSList
{
    public partial class NewMainWindow
    {
        #region Обработка контекстного меню шапки Tabs
        /// <summary>
        /// Метод обработки события нажатия кнопки закрытия вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTabButton_OnClick(object sender, RoutedEventArgs e)
        {

            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is TabItem)
                {
                    Customer selCust = ((TabItem)dep).Content as Customer;
                    OpenedTTRemove(selCust);

                    return;
                }
            }
        }

        /// <summary>
        /// Метод обработки нажатия элементов контекстного меню TabCustomer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem curMenuItem = sender as MenuItem;
                if (SelectedTT != null)
                    if (curMenuItem.Tag.ToString() == "1")
                        CloseSelectedTab();
                    else if (curMenuItem.Tag.ToString() == "2")
                        CloseOtherTabs();
                    else if (curMenuItem.Tag.ToString() == "3")
                        CloseAllTabs();
            }
            catch (Exception ex)
            {
                Log("Ошибка при нажатии по элементу контекстного меню." + ex.Message, true, true, ex.StackTrace);
            }
        }

        private void CloseSelectedTab()
        {
            this.OpenedTTRemove(this.Tabs.SelectedItem as Customer);
        }

        private void CloseOtherTabs()
        {
            for (int i = OpenedTT.Count - 1; i >= 0; i--)
            {
                if (OpenedTT[i] != Tabs.SelectedItem)
                {
                    this.OpenedTTRemove(OpenedTT[i]);
                }
            }
        }

        private void CloseAllTabs()
        {
            this.OpenedTTClear();
        }

        private void CloseTab(object sender, RoutedEventArgs e)
        {
            this.OpenedTTRemove(CloseTT);
        }

        private bool OpenedTTClear()
        {
            for (int i = OpenedTT.Count - 1; i >= 0; i--)
            {
                if (!this.SaveNotes(OpenedTT[i], true))
                    return false;
                OpenedTT.Remove(OpenedTT[i]);
            }
            return true;
        }

        protected internal void OpenedTTRemove(Customer tt)
        {
            if (this.SaveNotes(tt, true))
            {
                OpenedTT.Remove(tt);
            }
        }
        #endregion

        #region Обработка верхней части Customer(номер ЦВЗ, регион, время)

        private void ButtonPhoneCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(settings.Fields.PhoneIPNumber))
                {
                    Log("Необходимо заполнить номер телефона в настройках", true, false);
                    OpenSettingsWindow(false, true);
                    return;



                }
                Thread runThread = new Thread(new ThreadStart(() =>
                {
                   
                    WebRequest webr = WebRequest.Create($"http://call.dengisrazy.ru/admin/call.php?from={settings.Fields.PhoneIPNumber}&to={SelectedTT.Phone}");
                    HttpWebResponse resp = null;

                    try
                    {
                        resp = (HttpWebResponse)webr.GetResponse();
                    }
                    catch (WebException msg)
                    {
                        resp = (HttpWebResponse)msg.Response;
                        Log(msg.Message);
                    }
                }));
                runThread.Start();
                
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }


        }

        /// <summary>
        /// Метод отработки события нажатия кнопки контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem curMenuItem = sender as MenuItem;

                if (SelectedTT != null)
                {
                    if (curMenuItem.Tag.ToString() == "1")
                    {
                        CopyToClipboard(SelectedTT.NumberCVZ.ToString(), true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "2")
                    {
                        CopyToClipboard($"{ SelectedTT.Province} {SelectedTT.City} {SelectedTT.Address}", true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "3")
                    {
                        CopyToClipboard($"{SelectedTT.ToStringDisplay} {SelectedTT.Province}", true, "");
                    }


                }
            }
            catch (Exception ex)
            {
                Log("Ошибка при нажатии по элементу контекстного меню." + ex.Message, true, true, ex.StackTrace);
            }


        }

        private async void ButtonRefreshHosts_Click(object sender, RoutedEventArgs e)
        {
            await PingSelectedTT();
        }
        #endregion

        #region Обработка ListViewIP

        private void ListViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is ListViewItem)
                {
                    IP selIP = ((ListViewItem)dep).Content as IP;
                    //System.Windows.Forms.MessageBox.Show(selIP.IPAddress);
                    //ContextMenuListViewItem.IsOpen = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Метод отработки события нажатия кнопки контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem curMenuItem = sender as MenuItem;

                if (SelectedIP != null)
                {
                    if (curMenuItem.Tag.ToString() == "1")
                    {
                        CopyToClipboard(SelectedIP.IPAddress, true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "2")
                    {
                        CopyToClipboard(SelectedIP.NetbiosName, true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "3")
                    {
                        CopyToClipboard(SelectedIP.NetbiosName + " " + SelectedIP.IPAddress, true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "4")
                    {
                        CopyToClipboard($"{SelectedIP.NetbiosName} {SelectedIP.IPAddress} {SelectedIP.Owner.SubnetMask}", true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "5")
                    {
                        CopyToClipboard($"{SelectedIP.Owner.ToStringDisplay}", true, "");
                    }
                    else if (curMenuItem.Tag.ToString() == "6")
                    {
                        //CopyUIElementToClipboard((((sender as MenuItem).Parent as ContextMenu).PlacementTarget as ListViewItem).Parent as FrameworkElement);
                        //CopyUIElementToClipboard(ListViewHostsInCustomer);
                        Log($"В буфер скопирован список хостов в виде изображения.", false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка при вызове контекстного меню." + ex.Message, true, true, ex.StackTrace);
            }


        }

        /// <summary>
        /// Метод копирование изображения элемента FrameworkElement в буфер обмена
        /// </summary>
        /// <param name="element">FrameworkElement, который необходимо скопировать в буфер обмена в виде картинки</param>
        public void CopyUIElementToClipboard(FrameworkElement element)
        {
            try
            {
                double actualWidth = element.ActualWidth;
                double actualHeight = element.ActualHeight;
                RenderTargetBitmap image = new RenderTargetBitmap((int)Math.Round(actualWidth), (int)Math.Round(actualHeight), 96.0, 96.0, PixelFormats.Default);
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext context = visual.RenderOpen())
                {
                    VisualBrush brush = new VisualBrush(element);
                    context.DrawRectangle(brush, null, new Rect(new System.Windows.Point(), new System.Windows.Size(actualWidth, actualHeight)));
                }
                image.Render(visual);
                System.Windows.Clipboard.SetImage(image);
                Log("Выполнено копирование элемента FrameworkElement " + element.Name);
            }
            catch (Exception ex)
            {
                Log("Ошибка при копировании элемента FrameworkElement. " + ex.Message, true, true, ex.StackTrace);
            }

        }

        private void IPTypeChange_Click(object sender, RoutedEventArgs e)
        {
            SelectedIP.Type = GetIPType((e.OriginalSource as MenuItem).Header.ToString());

        }

        /// <summary>
        /// Метод получения перечисления IPType на основе строкового значения
        /// </summary>
        /// <param name="stringIPType">строковое значение, соответствующее IPType перечислению</param>
        /// <returns>соответствующее значение перечисления IPType</returns>
        private IPType GetIPType(string stringIPType)
        {
            return (from ipT in Enum.GetValues(typeof(IPType)).Cast<IPType>()
                    where ipT.ToString() == stringIPType
                    select ipT).FirstOrDefault();
        }

        private void ListViewHostsInCustomer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectedCredentials.Login = (from credLogin in NewMainWindow.Credentials
                                             where credLogin.HostType == SelectedIP.Type
                                             select credLogin).FirstOrDefault().Login;
            }
            catch (Exception)
            {
                try
                {
                    SelectedCredentials.Login = (from credLogin in NewMainWindow.Credentials
                                                 where credLogin.HostType == IPType.Общий
                                                 select credLogin).FirstOrDefault().Login;
                }
                catch (Exception)
                {
                    Log("Необходимо заполнить пароли, добавить пароль типа \"Общий\"", true, false);
                    OpenSettingsWindow(true);
                }

            }


        }

        private void ListViewHostsInCustomer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is ListViewItem)
                {
                    IP selIP = ((ListViewItem)dep).Content as IP;

                    if (!NewMainWindow.settings.Fields.PopupMenuDelayed)
                    {
                        if (this.NextPlacementTarget != (dep as ListViewItem))
                        {
                            this.PopupMenu.IsOpen = false;
                        }
                        this.NextPlacementTarget = dep as ListViewItem;
                        this.PopupMenu.AllowsTransparency = false;
                        this.OpenPopupMenu();
                    }
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        //this.AddSelectedIPToMonitoring();
                    }

                }
            }
        }


        private void ListViewHostsInCustomer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (NewMainWindow.settings.Fields.DoubleClickPing)
            {
                var dep = (DependencyObject)e.OriginalSource;
                while (dep != null)
                {
                    dep = VisualTreeHelper.GetParent(dep);
                    if (dep is ListViewItem)
                    {
                        IP selIP = ((ListViewItem)dep).Content as IP;

                        Log($"cmd.exe  /c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} /t & pause ", false, false, "", false);
                        NewMainWindow.ExecuteProgram("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} /t & pause", false, true, "", false, "", "");

                        return;
                    }
                }
            }
        }

        #endregion

        #region Обработка Expander

        private void ExpanderInfo_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((sender as ListView).SelectedItem != null)
                {
                    var placementTarget = (sender as ListView).SelectedItem as BindingListItem;
                    CopyToClipboard(placementTarget.Description + ": " + placementTarget.Value);
                    //Owner.Log("Выполнено копирование в буфер обмена " + placementTarget.Description + ": " + placementTarget.Value, false, false);
                }

            }
            catch (Exception ex)
            {
                Log("Ошибка при нажатии RightMouseButton в поле ExpanderInfo. " + ex.Message, true, true, ex.StackTrace);
            }

        }

        private void ExpanderInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((sender as ListView).SelectedItem != null)
                {
                    var placementTarget = (sender as ListView).SelectedItem as BindingListItem;
                    CopyToClipboard(placementTarget.Value);
                    //Owner.Log("Выполнено копирование в буфер обмена " + placementTarget.Value, false, false);
                }

            }
            catch (Exception ex)
            {
                Log("Ошибка при нажатии LeftMouseButton в поле ExpanderInfo. " + ex.Message, true, true, ex.StackTrace);
            }
        }

        #endregion

        #region ListViewItem APM

        private void btnRefreshHostInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IP curSelIP = SelectedIP;

                Thread runThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        string ipaddress = curSelIP.IPAddress;
                        var login = SelectedCredentials.Login;
                        var password = SelectedCredentials.Password;

                        using (SshClient client = new SshClient(ipaddress, login, password))
                        {
                            client.Connect();
                            StringBuilder commandBuild = new StringBuilder();
                            commandBuild.AppendLine(@"#!/bin/bash");
                            commandBuild.AppendLine($"TempPass={password}");

                            Dispatcher.Invoke(() =>
                            {
                                curSelIP.InfoHost.NetbiosName = client.CreateCommand(commandBuild.ToString() + ("\necho $(hostname)").Replace("\r\n", "\n")).Execute().Replace("\n", "");

                                curSelIP.InfoHost.CPUName = client.CreateCommand(new StringBuilder().Append(commandBuild).AppendLine(Properties.Resources.sshCommand_CPUName).ToString().Replace("\r\n", "\n")).Execute().Replace("\n", "");

                                curSelIP.InfoHost.MBName = client.CreateCommand(commandBuild.ToString() + "\n" + Properties.Resources.sshCommand_MB.Replace("\r\n", "\n")).Execute().Replace("\n", "");

                                curSelIP.InfoHost.RamSize = client.CreateCommand(commandBuild.ToString() + "\n" + Properties.Resources.sshCommand_Memory.Replace("\r\n", "\n")).Execute().Replace("\n", "");

                                curSelIP.InfoHost.HDDSize = client.CreateCommand(commandBuild.ToString() + "\n" + Properties.Resources.sshCommand_HDD_Size.Replace("\r\n", "\n")).Execute().Replace("\n", "");

                            });


                            client.Disconnect();
                        }
                    }
                    catch (ArgumentException exception)
                    {
                        Log("Необходимо выбрать логин и пароль", true, true, exception.StackTrace, false);
                    }
                    catch (Exception exception)
                    {
                        this.Log(exception.Message, true, true, exception.StackTrace, false);
                    }
                }));
                runThread.Start();

            }
            catch (Exception)
            {

            }


        }


        private void DeleteProfileGoogleChrome_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Удаление профиля Google Chrome на хосте {curSelIP.NBNameOrIP()}", "rm -f '/home/user/.config/google-chrome/SingletonLock'", SelectedCredentials.Login, SelectedCredentials.Password, false, true);

            }
        }

        private void DeleteProfileChromium_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Удаление профиля Chromium на хосте {curSelIP.NBNameOrIP()}", "rm -f '/home/user/.config/chromium/SingletonLock'", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void Kill1CWine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Аварийное завершение работы клиента 1C на хосте {curSelIP.NBNameOrIP()}", "pkill -9 wine; pkill -9 1c", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void KillZimbraProcess_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Аварийное завершение работы Zimbra Desktop на хосте {curSelIP.NBNameOrIP()}", "pkill -9 zdclient", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void KillChromeFirefoxProcess_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Аварийное завершение работы браузеров Chrome и Firefox на хосте {curSelIP.NBNameOrIP()}", "pkill -9 chrom ; pkill -9 firefox", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void RefreshProfileZimbra_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Обновление профиля пользователя Zimbra Desktop на хосте {curSelIP.NBNameOrIP()}", "pkill -9 zdclient ; perl /opt/zimbra/zdesktop/linux/user-install.pl", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }



        #endregion

        #region ListViewItem Маршрутизатор

        private void MikrotikIPSecTunelsFlush_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Thread runThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        #region Выполнение Flush

                        MK mikrotikIPSecFlush = new MK(SelectedIP.IPAddress);
                        if (!mikrotikIPSecFlush.Login(SelectedCredentials.Login, SelectedCredentials.Password))
                        {
                            Log("Could not log in");
                            mikrotikIPSecFlush.Close();
                            return;
                        }
                        mikrotikIPSecFlush.Send("/ip/ipsec/installed-sa/flush");
                        mikrotikIPSecFlush.Send("=sa-type=all");
                        mikrotikIPSecFlush.Send(".tag=flush", true);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                }));
                runThread.Start();

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private void UsbPowerReset_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Thread runThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        #region Сброс питания на USB порту MikroTik
                        int seconds = 5;
                        Dispatcher.Invoke(() =>
                        {
                            if (int.TryParse(((Button)e.OriginalSource).Tag.ToString(), out seconds))
                            {

                            }
                            else
                            {
                                seconds = 5;
                            }

                        });


                        MK mikrotikUSBPowerReset = new MK(SelectedIP.IPAddress);
                        if (!mikrotikUSBPowerReset.Login(SelectedCredentials.Login, SelectedCredentials.Password))
                        {
                            Log("Не удается подключиться к MikroTik");
                            mikrotikUSBPowerReset.Close();
                            return;
                        }
                        //system routerboard usb power-reset duration=5
                        mikrotikUSBPowerReset.Send("/system/routerboard/usb/power-reset");


                        mikrotikUSBPowerReset.Send($"=duration={seconds}");
                        mikrotikUSBPowerReset.Send(".tag=rebootUsb", true);
                        //mikrotik.Send(".tag=arp", true);
                        //Host curHost;
                        //StringBuilder result = new StringBuilder();
                        //foreach (string h in mikrotikUSBPowerReset.Read())
                        //{
                        //    result.AppendLine(h);
                        //}
                        //Console.Write(result.ToString());

                        //Console.ReadKey();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                }));
                runThread.Start();

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }

        }

        private void MikrotikIPSecTunelsPrint_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IP curIP = SelectedIP;

                Thread runThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        #region Вывод списка направлений туннеля
                        MK mikrotikIPSecTunels = new MK(curIP.IPAddress);
                        if (!mikrotikIPSecTunels.Login(SelectedCredentials.Login, SelectedCredentials.Password))
                        {
                            Log("Could not log in");
                            mikrotikIPSecTunels.Close();
                            return;
                        }
                        mikrotikIPSecTunels.Send("/ip/ipsec/installed-sa/print");
                        mikrotikIPSecTunels.Send(".tag=listInstalledSAs", true);
                        StringBuilder result = new StringBuilder();
                        //List<string[]> temp = new List<string[]>();
                        foreach (string h in mikrotikIPSecTunels.Read())
                        {
                            var massElements = h.Split('=');
                            if (massElements.Count() > 9)
                            {
                                result.AppendLine($"src-address {massElements[7]} ---> dst-address {massElements[9]}");
                            }
                        }
                        Dispatcher.Invoke(() =>
                        {
                            ((Button)e.OriginalSource).Tag = result;
                        });

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                }));
                runThread.Start();

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            //StringBuilder test = new StringBuilder();
            //test.AppendLine($"src-address 192.168.61.102 ---> dst-address 8.8.8.8");
            //test.AppendLine($"src-address 8.8.8.8 ---> dst-address 192.168.61.102");
            //((Button)e.OriginalSource).Tag = test.ToString();
        }

        private void MikrotikIPSec_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Thread runThread = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        #region Вывод списка каналов

                        MK mikrotikIPSec = new MK(SelectedIP.IPAddress);
                        if (!mikrotikIPSec.Login(SelectedCredentials.Login, SelectedCredentials.Password))
                        {
                            Log("Could not log in");
                            mikrotikIPSec.Close();
                            return;
                        }
                        mikrotikIPSec.Send("/ip/ipsec/policy/print");
                        mikrotikIPSec.Send(".tag=policies", true);
                        StringBuilder result = new StringBuilder();
                        foreach (string h in mikrotikIPSec.Read())
                        {
                            var massElements = h.Split('=');
                            if (massElements.Count() > 9)
                            {
                                foreach (var item in massElements)
                                {
                                    result.AppendLine(item);
                                }

                            }
                        }
                        Dispatcher.Invoke(() =>
                        {
                            ((Button)e.OriginalSource).Tag = result;
                        });

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }

                }));
                runThread.Start();

            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }

        }


        #endregion

    }
}
