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
using System.Windows.Forms;
using System.IO;

namespace DSList
{
    public partial class NewMainWindow
    {
        #region TEST

        private void TestPlink_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //String command, targetobj;
            //command = ((RoutedCommand)e.Command).Name;
            //targetobj = ((FrameworkElement)sender).Name;
            //StartProgramSupportTools("explorer.exe", $"http://{SelectedIP.Address}", true);
            //MessageBox.Show("The " + command + " command has been invoked on target object " + targetobj);
            //ApplicationCommands.Open

            TestPlink_Method();
        }

        public async void TestPlink_Method()
        {
            await Task.Run(() =>
            {
                try
                {
                    //System.Windows.Forms.MessageBox.Show("Test");
                    StartProgramSupportTools("plink.exe", $"-v -ssh {SelectedIP.IPAddress} -pw {SelectedCredentials.Password} -l {SelectedCredentials.Login} -batch sh test/mc.sh >1.txt", false);
                    // plink.exe -v -ssh 192.168.61.102 -l user -pw 1 -batch sh test/mc.sh
                    //MainWindow.SelectedIP, @"support\plink.exe", $"-ssh {MainWindow.SelectedTT.Gateway}", this.SelectedCredentials.Login, this.SelectedCredentials.Password, true, false, true, null

                }
                catch (Exception ex)
                {
                    Log(ex.Message, true, true, ex.StackTrace);
                }

            });
        }


        private void ClickTest1_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //String command, targetobj;
            //command = ((RoutedCommand)e.Command).Name;
            //targetobj = ((FrameworkElement)sender).Name;
            //StartProgramSupportTools("explorer.exe", $"http://{SelectedIP.Address}", true);
            //MessageBox.Show("The " + command + " command has been invoked on target object " + targetobj);
            //ApplicationCommands.Open

            DownloadXLSXProvidersFile();
        }



        public async void DownloadXLSXProvidersFile()
        {
            await Task.Run(() =>
            {
                try
                {
                    ExcelParsing newExcelFile = new ExcelParsing("1.xlsx");
                    newExcelFile.ParseXLSX();
                    System.Windows.Clipboard.SetDataObject(newExcelFile.DTFromXLSX);
                    //Window newWin = new Window();
                    //System.Windows.Controls.DataGrid DG = new System.Windows.Controls.DataGrid();
                    //DataGridView DGV = new DataGridView();
                    //DGV.DataSource = newExcelFile.DTFromXLSX;
                    //DG.ItemsSource =
                    //newWin.Content = DGV;


                }
                catch (Exception ex)
                {
                    Log(ex.Message, true, true, ex.StackTrace);
                }

            });
        }

        private void ClickTest_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            //System.Windows.Forms.MessageBox.Show($"isipselected = {isipselected} , bindings.isipselected = {Bindings.isipselected}\nisttselected = {isttselected} , bindings.isttselected = {Bindings.isttselected}");

            //if (SelectedIP == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("SelectedIPInCustomer = null");
            //}
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show(SelectedIP.ToString());
            //}

            //if (SelectedIP == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("SelectedIP = null");
            //}
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show(SelectedIP.ToString());
            //}

            //if (TabCtrl.SelectedContent == null)
            //    System.Windows.Forms.MessageBox.Show("SelectedContent null");
            //else
            //    System.Windows.Forms.MessageBox.Show((TabCtrl.SelectedItem as Customer).ToString());

            //try
            //{
            //    System.Windows.Forms.MessageBox.Show(SelectedTT.ListViewHostsInCustomer.SelectedItem.ToString());
            //    //System.Windows.Forms.MessageBox.Show(SelectedTT.ListViewHostsInCustomer.SelectedValue.ToString());

            //    System.Windows.Forms.MessageBox.Show(SelectedTT.SelectedIP.ToString());

            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("SelectedIP - error");
            //}

            //searchListBox.ItemsSource = null;
            ////if (searchListBox.Items.Count!=0)
            ////{
            //searchListBox.Items.Clear();
            ////}

            //searchListBox.ItemsSource = LoadDBFromSQLServer("Data Source=localhost;Port=3306;Initial Catalog=mikrotik;User Id=root;password=fawGV89094047282");
            //String command, targetobj;
            //command = ((RoutedCommand)e.Command).Name;
            //targetobj = ((FrameworkElement)sender).Name;
            //StartProgramSupportTools("explorer.exe", $"http://{SelectedIP.Address}", true);
            //MessageBox.Show("The " + command + " command has been invoked on target object " + targetobj);
            //ApplicationCommands.Open

            //DownloadSiteForNumberCVZ();

        }



        public async void DownloadSiteForNumberCVZ()
        {
            await Task.Run(() =>
            {
                try
                {
                    string path = $"http://gms.dengisrazy.ru/search.php?id={SelectedIP.Owner.NumberCVZ}";
                    System.Windows.Forms.MessageBox.Show(path);
                    TableFromSite dict = new TableFromSite(new Url(path));
                    SelectedIP.Owner.FillCustomerFromDict(dict.CreateDictFromSite());
                    SelectedIP.Owner.PopulateInfo();
                }
                catch (Exception ex)
                {
                    Log(ex.Message, true, true, ex.StackTrace);
                }

            });
        }

        private void ClickTest_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = Bindings.isipselected;
            //bool tp = Bindings.isipselected;
            e.CanExecute = true;
        }

        #endregion

        private void TightVNCViewerWithoutParams_Click(object sender, RoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"TightVNC без параметров ", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\tvnviewer.exe"), null, false, true, "", false, "", "");
            }
        }


        private void RunTightVNC(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"TightVNC ${ SelectedIP.IPAddress} ", false, false, "", false);
                if (!string.IsNullOrWhiteSpace(SelectedCredentials.Password))
                    ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\tvnviewer.exe"), $"{SelectedIP.IPAddress} -password={SelectedCredentials.Password}", false, true, "", false, "", "");
                else
                    ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\tvnviewer.exe"), $"{SelectedIP.IPAddress} ", false, true, "", false, "", "");

            }
        }


        private void RunVNC_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\VNC-Viewer.exe"), $"{SelectedIP.IPAddress} WarnUnencrypted=0", false, true, "", false, "", "");
                //StartProgramSupportTools("VNC-Viewer.exe", $"{SelectedIP.Address} WarnUnencrypted=0");
                CopyToClipboard(SelectedCredentials.Password, false, "Пароль скопирован в буфер обмена");
            }
        }


        private void RunVNCPassFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"VNC { SelectedIP.IPAddress} WarnUnencrypted = 0 с использованием файла шифрованного пароля", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\VNC-Viewer.exe"),
                    $"{SelectedIP.IPAddress} PasswordFile={System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\passvnc.credential")} WarnUnencrypted=0",
                    false, true, "", false, "", "");
            }
        }

        private void ControlsAndFilesTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void PuttySSH_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //if (System.IO.File.Exists(settings.Fields.putty_path))
            //{
            //    StartProgramSupportTools(settings.Fields.putty_path,
            //    $"-ssh {SelectedIP.Address} -l {SelectedCredentials.Login} -pw {SelectedCredentials.Password}", true); //-v ЦВЗ№{SelectedIP.Owner.NumberCVZ} {SelectedIP.Address} -l fawgv -pw 1
            //}
            //else
            //    StartProgramSupportTools("putty.exe",
            //    $"-ssh {SelectedIP.Address} -l {SelectedCredentials.Login} -pw {SelectedCredentials.Password}"); //-v ЦВЗ№{SelectedIP.Owner.NumberCVZ} {SelectedIP.Address} -l fawgv -pw 1

            if (Bindings.isipselected)
            {
                string putty;
                string arguments;
                this.Log("Putty SSH " + SelectedIP.NBNameOrIP(), false, false, "", false);
                if (!string.IsNullOrWhiteSpace(SelectedCredentials.Login))
                {
                    arguments = $"-ssh {SelectedCredentials.Login}@{SelectedIP.IPAddress} 22";
                    if (!string.IsNullOrWhiteSpace(SelectedCredentials.Password))
                    {
                        arguments = arguments + " -pw " + SelectedCredentials.Password;
                    }
                }
                else
                {
                    arguments = SelectedIP.IPAddress;
                }
                if (System.IO.File.Exists(settings.Fields.putty_path))
                    putty = settings.Fields.putty_path;
                else
                    putty = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\putty.exe");

                ExecuteProgram(putty, arguments, false, true, "", false, "", "");
            }
        }

        private void PuttyTelnet_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //StartProgramSupportTools("putty.exe",
            //   $"-telnet {SelectedIP.Address}"); //-v ЦВЗ№{SelectedIP.Owner.NumberCVZ} {SelectedIP.Address} -l fawgv -pw 1

            if (Bindings.isipselected)
            {
                string putty;
                string arguments;
                this.Log("Putty Telnet " + SelectedIP.NBNameOrIP(), false, false, "", false);

                arguments = $"-telnet {SelectedIP.IPAddress}";

                if (System.IO.File.Exists(settings.Fields.putty_path))
                    putty = settings.Fields.putty_path;
                else
                    putty = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\putty.exe");

                ExecuteProgram(putty, arguments, false, true, "", false, "", "");
            }
        }

        private void WinboxMikrotik_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //StartProgramSupportTools("winbox.exe", $"{SelectedIP.Address} {SelectedCredentials.Login} {SelectedCredentials.Password}");
            try
            {
                if (Bindings.isipselected)
                {
                    string winbox;
                    string arguments;
                    Log("Winbox MikroTik " + SelectedIP.NBNameOrIP(), false, false, "", false);
                    if (!string.IsNullOrWhiteSpace(SelectedCredentials.Login))
                    {
                        arguments = $"{SelectedIP.IPAddress} {SelectedCredentials.Login} ";
                        if (!string.IsNullOrWhiteSpace(SelectedCredentials.Password))
                        {
                            arguments = arguments + SelectedCredentials.Password;
                        }
                    }
                    else
                    {
                        arguments = SelectedIP.IPAddress;
                    }

                    if (settings.Fields.winboxmikrotik_path_usedefault && File.Exists(settings.Fields.winboxmikrotikpath))
                        winbox = settings.Fields.winboxmikrotikpath;
                    else
                        winbox = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\winbox.exe");

                    ExecuteProgram(winbox, arguments, false, true, "", false, "", "");
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, true, false, ex.StackTrace);
            }
        }

        private void BrowserHostRun_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //StartProgramSupportTools("explorer.exe", $"http://{SelectedIP.Address}", true);
                Log("Запуск " + SelectedIP.NBNameOrIP() + " в браузере по умолчанию", false, false, "", false);
                ExecuteProgram("explorer.exe", $"http://{SelectedIP.IPAddress}", false, true, "", false, "", "");
            }
        }

        private void FileZilla_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"FileZilla sftp://login:password@{SelectedIP.NBNameOrIP()}:22", false, false, "", false);
                string filezilla = "";
                if (System.IO.File.Exists(settings.Fields.filezilla_path))
                {
                    filezilla = settings.Fields.filezilla_path;
                }
                else
                {
                    string text = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SupportTools");
                    SettingsWindow tempSet = new SettingsWindow();
                    tempSet.Visibility = Visibility.Collapsed;
                    if (tempSet.LocateFile(ref text, "FileZilla*.exe"))
                    {
                        settings.Fields.filezilla_path = text;
                    }
                    settings.SaveSettings();
                    tempSet.Close();
                    filezilla = settings.Fields.filezilla_path;
                }
                if (System.IO.File.Exists(settings.Fields.filezilla_path))
                {
                    //StartProgramSupportTools(settings.Fields.filezilla_path, $"sftp://{SelectedCredentials.Login}:{SelectedCredentials.Password}@{SelectedIP.Address}:22", true); /*-a =% userprofile %\\Desktop*/
                    ExecuteProgram(filezilla, $"sftp://{SelectedCredentials.Login}:{SelectedCredentials.Password}@{SelectedIP.IPAddress}:22", false, true, "", false, "", "");
                }
            }
        }


        private void WinSCP_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"WinSCP sftp://login:password@{SelectedIP.NBNameOrIP()}:22", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\WinSCP.exe"), $"sftp://{SelectedCredentials.Login}:{SelectedCredentials.Password}@{SelectedIP.IPAddress}:22", false, true, "", false, "", "");
            }
        }


        private void PuttyWithoutParams_Click(object sender, RoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                string putty;
                Log("Putty без параметров ", false, false, "", false);

                if (System.IO.File.Exists(settings.Fields.putty_path))
                    putty = settings.Fields.putty_path;
                else
                    putty = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\putty.exe");

                ExecuteProgram(putty, null, false, true, "", false, "", "");
            }
        }

        private void VNCWithoutParams_Click(object sender, RoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log("VNC без параметров ", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\VNC-Viewer.exe"), null, false, true, "", false, "", "");
            }
        }

        private void WinboxMikroTikWithoutParams_Click(object sender, RoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log("Winbox MikroTik без параметров ", false, false, "", false);
                ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\winbox.exe"), null, false, true, "", false, "", "");
            }
        }

        private void FileZillaWithoutParams_Click(object sender, RoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                Log($"FileZilla без параметров", false, false, "", false);
                string filezilla = "";
                if (System.IO.File.Exists(settings.Fields.filezilla_path))
                {
                    filezilla = settings.Fields.filezilla_path;
                }
                else
                {
                    string text = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SupportTools");
                    SettingsWindow tempSet = new SettingsWindow();
                    tempSet.Visibility = Visibility.Collapsed;
                    if (tempSet.LocateFile(ref text, "FileZilla*.exe"))
                    {
                        settings.Fields.filezilla_path = text;
                    }
                    settings.SaveSettings();
                    tempSet.Close();
                    filezilla = settings.Fields.filezilla_path;
                }
                if (System.IO.File.Exists(settings.Fields.filezilla_path))
                {
                    ExecuteProgram(filezilla, null, false, true, "", false, "", "");
                }
            }
        }

        

        
    }
}
