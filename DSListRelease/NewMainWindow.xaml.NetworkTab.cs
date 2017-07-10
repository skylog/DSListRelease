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
using TaskDialogInterop;

namespace DSList
{
    public partial class NewMainWindow
    {
        private void DetectSubnetIP_Click(object sender, RoutedEventArgs e)
        {
            EnterSubnetIP(SelectedTT);
        }

        private async void EnterSubnetIP(Customer selectedTT)
        {
            EnterIPWindow newEIPW = new EnterIPWindow();
            newEIPW.Owner = this;
            newEIPW.Title = $"{selectedTT.ToStringDisplay}";
            newEIPW.TextBoxIP.Text = (selectedTT.Lan_Ip == "0.0.0.0") ? string.Empty : selectedTT.Lan_Ip;
            newEIPW.TextBoxSM.Text = (selectedTT.SubnetMask == string.Empty) ? "255.255.255.224" : selectedTT.SubnetMask;
            newEIPW.TextBoxIP.Focus();
            newEIPW.TextBoxIP.SelectAll();
            newEIPW.ShowDialog();
            bool? dialogResult = newEIPW.DialogResult;
            bool flag2 = true;
            if ((dialogResult.GetValueOrDefault() == flag2) ? dialogResult.HasValue : false)
            {
                selectedTT.Lan_Ip = newEIPW.TextBoxIP.Text;
                selectedTT.SubnetMask = newEIPW.TextBoxSM.Text;
                selectedTT.PopulateIPList();
                await PingSelectedTT(true);
            }
        }

        protected internal async Task PingSelectedTT(bool always = false)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
            {
                if (Bindings.isttselected && (PingSubnetButton.IsEnabled || always))
                {
                    if (SelectedTT != null)
                        SelectedTT.PingSubnet();
                };
                
            });
        }


        private void PingAll_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async delegate
            {
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) async delegate { await PingSelectedTT(false); });
            });
        }


        /// <summary>
        /// Поле, содержащее производителей MAC адресов
        /// </summary>
        string[] macvendors = new string[0];

        /// <summary>
        /// Метод вывода ARP таблицы текущего ПК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ARPWin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log("Таблица ARP локального ПК ", false, false, "", false);
                ExecuteProgram("cmd", $" /k arp -a -v", false, true, "", false, "", "");
                //StartProgramSupportTools("cmd.exe ", $" /k arp -a -v", true);
            }

        }

        /// <summary>
        /// Метод вызова WinMTR(программа для диагностики сети на потери при передаче пакетов)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMTR_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"WinMTR.exe {SelectedIP.IPAddress} ", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\WinMTR.exe"), SelectedIP.IPAddress, false, true, "", false, "", "");
                //StartProgramSupportTools("WinMTR.exe", SelectedIP.Address, false);
            }

        }

        /// <summary>
        /// Метод вызова определения MAC адреса выбранного хоста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MacInfo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (macvendors.Length == 0)
            {
                LoadMACVendorsDB();
            }
            try
            {
                Task.Run(() => this.mac_info_thread(SelectedIP));
            }
            catch (Exception ex)
            {
                Log(ex.Message, true, false, ex.StackTrace);
            }
        }

        private void mac_info_thread(IP selectedIP)
        {
            string mac = GetMac(selectedIP.IPAddress);

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => CopyToClipboard(mac, true, "")));

            string mac_msg = null;
            for (int i = 0; i < this.macvendors.Length; i += 2)
            {
                if (mac.StartsWith(this.macvendors[i]))
                {
                    mac_msg = "MAC адрес: " + mac + "\nПроизводитель: " + this.macvendors[i + 1];
                    break;
                }
            }
            if (mac_msg != null)
            {
                base.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    TaskDialogOptions options = new TaskDialogOptions
                    {
                        Owner = this,
                        Title = "DSList",
                        MainInstruction = selectedIP.NBNameOrIP(),
                        Content = mac_msg,
                        MainIcon = VistaTaskDialogIcon.Information
                    };
                    options.CustomButtons = new string[] { "ОК" };
                    options.AllowDialogCancellation = true;
                    TaskDialog.Show(options);
                }));
            }
            else
            {
                Log($"Не удалось определить mac адрес для {selectedIP.NBNameOrIP()}", false, false);
            }
        }

        private string GetMac(string ip)
        {
            string path = Environment.GetEnvironmentVariable("SystemRoot") + @"\" + (Environment.Is64BitOperatingSystem ? "sysnative" : "System32") + @"\nbtstat.exe";
            if (!System.IO.File.Exists(path))
            {
                path = "nbtstat.exe";
            }
            string str2 = string.Empty;
            try
            {
                Process process = new Process
                {
                    StartInfo = {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        FileName = path,
                        Arguments = "-a " + ip
                    }
                };
                process.Start();
                str2 = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                char[] separator = new char[] { '\n' };
                foreach (string str3 in str2.Split(separator))
                {
                    string[] strArray2 = new string[] { "(MAC) = ", "MAC Address = ", "Адрес платы (MAC) = " };
                    foreach (string str4 in strArray2)
                    {
                        if (str3.Contains(str4))
                        {
                            Log("Получен MAC-адрес " + str3.Remove(0, str3.IndexOf(str4) + str4.Length), false, false);
                            return str3.Remove(0, str3.IndexOf(str4) + str4.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Ошибка при выполнении GetMac. " + ex.Message, true, false, ex.StackTrace);
                return ex.Message;
            }
            return string.Empty;
        }


        private void PingExtended_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //string arguments = "";
                //if (SelectedIP.Owner != null)
                //    arguments = $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & ping {SelectedIP.Address} -l {Properties.Settings.Default.ping_bytes} /t & pause";
                //else
                //    arguments = $"/c title {SelectedIP.Address} & ping {SelectedIP.Address} -l {Properties.Settings.Default.ping_bytes} /t & pause";

                Log($"cmd.exe  /c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} -l {Properties.Settings.Default.ping_bytes} /t & pause ", false, false, "", false);
                ExecuteProgram("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} -l {Properties.Settings.Default.ping_bytes} /t & pause", false, true, "", false, "", "");
                //StartProgramSupportTools("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & ping {SelectedIP.Address} -l {Properties.Settings.Default.ping_bytes} /t & pause", true);
            }
        }
        private void PingControl_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //string arguments = "";
                //if (SelectedIP.Owner != null)
                //    arguments = $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & ping {SelectedIP.Address} /t & pause";
                //else
                //    arguments = $"/c title {SelectedIP.Address} & ping {SelectedIP.Address} /t & pause";

                Log($"cmd.exe  /c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} /t & pause ", false, false, "", false);
                ExecuteProgram("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & ping {SelectedIP.IPAddress} /t & pause", false, true, "", false, "", "");
                //StartProgramSupportTools("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & ping {SelectedIP.Address} /t & pause", true);
            }
        }

        private void PathPing_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //string arguments = "";
                //if (SelectedIP.Owner != null)
                //    arguments = $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & PathPing {SelectedIP.Address} & pause";
                //else
                //    arguments = $"/c title {SelectedIP.Address} & PathPing {SelectedIP.Address} & pause";

                Log($"cmd.exe  /c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & PathPing {SelectedIP.IPAddress} & pause ", false, false, "", false);
                ExecuteProgram("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & PathPing {SelectedIP.IPAddress} & pause", false, true, "", false, "", "");
                //StartProgramSupportTools("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & PathPing {SelectedIP.Address} & pause", true);
            }
        }

        private void Tracert_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //string arguments = "";
                //if (SelectedIP.Owner != null)
                //    arguments = $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & Tracert {SelectedIP.Address} & pause";
                //else
                //    arguments = $"/c title {SelectedIP.Address} & Tracert {SelectedIP.Address} & pause";

                Log($"cmd.exe  /c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & Tracert {SelectedIP.IPAddress} & pause ", false, false, "", false);
                ExecuteProgram("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.IPAddress} & Tracert {SelectedIP.IPAddress} & pause", false, true, "", false, "", "");
                //StartProgramSupportTools("cmd.exe", $"/c title {SelectedIP.Owner.ToStringDisplay} {SelectedIP.Address} & Tracert {SelectedIP.Address} & pause", true);
            }

        }

        private void NetworkTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}

