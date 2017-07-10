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
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using TaskDialogInterop;
using System.Windows.Forms;
using System.Security;
using Renci.SshNet;
using Renci.SshNet.Common;
using DSList.Windows;
using MahApps.Metro.Controls;

namespace DSList
{
    public partial class NewMainWindow
    {

        private void SSHCommandThread(IP selectedIP, string command, string commandinresult, string login, string password, bool showWindow = true, bool addToListBoxTask = true)
        {

            Thread runThread = new Thread(new ThreadStart(() =>
            {
                ItemListBoxTask curItemLBT = new ItemListBoxTask() { IPOrName = selectedIP.NBNameOrIP(), Description = $"Выполняется {command}", CVZNumber = $"ЦВЗ№{SelectedTT.NumberCVZ.ToString()}", IsIndeterminate = true };

                //Dispatcher.Invoke(() =>
                //{

                if (addToListBoxTask)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Bindings.listBoxTaskItemsSource.Add(curItemLBT);
                    });
                }
                try
                {
                    this.Log(string.Format($"Выполнение {command} на {selectedIP.NBNameOrIP()}..."), false, false, string.Empty, true);
                    string nbNameOrIP = selectedIP.NBNameOrIP();
                    string ipaddress = selectedIP.IPAddress;


                    using (SshClient client = new SshClient(ipaddress, login, password))
                    {
                        client.Connect();
                        StringBuilder commandBuild = new StringBuilder();
                        commandBuild.AppendLine(@"#!/bin/bash");
                        commandBuild.AppendLine($"TempPass={password}");
                        commandBuild.AppendLine(commandinresult);
                        string commandExec = commandBuild.ToString().Replace("\r\n", "\n");
                        bool uploadFileStoped = false;

                        var runCommand = client.CreateCommand(commandExec);
                        var runClient = runCommand.BeginExecute((x) =>
                        {

                            var temp = curItemLBT.StopProcess;
                            if (curItemLBT.StopProcess)
                            {
                                if (!uploadFileStoped)
                                {
                                    try
                                    {
                                        uploadFileStoped = true;
                                        showWindow = false;
                                        client.Disconnect();
                                        //return $"Прервано выполнение {command}";
                                    }
                                    catch (Exception)
                                    {
                                    }


                                }
                            }

                        });

                        string result = runCommand.EndExecute(runClient);

                        client.Disconnect();
                        if (showWindow)
                        {
                            base.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                SSHReportWindow window = new SSHReportWindow(this)
                                {
                                    Owner = this,
                                    Title = string.Format($"Команда SSH {command} на {nbNameOrIP}"),
                                    Text = { Text = result },
                                    SSHStatusBarText = { Text = string.Format($"Команда SSH {command} на {nbNameOrIP} выполнена.") }
                                };
                                this.Log(window.SSHStatusBarText.Text, false, false, "", false);
                                window.Show();
                            }));
                        }
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

                //});
            }));

            runThread.Start();


            //while (!curItemLBT.StopProcess || (runThread.ThreadState != System.Threading.ThreadState.Stopped) || (runThread.ThreadState != System.Threading.ThreadState.Aborted))
            //{

            //    if (!(runThread.ThreadState == System.Threading.ThreadState.Stopped))
            //    {
            //        runThread.Abort();
            //    }
            //}

        }

        private string SSHCommandThreadGetResult(IP selectedIP, string command, string commandinresult, string login, string password)
        {
            try
            {
                this.Log(string.Format($"Выполнение {command} на {selectedIP.NBNameOrIP()}..."), false, false, string.Empty, true);
                string nbNameOrIP = selectedIP.NBNameOrIP();
                string ipaddress = selectedIP.IPAddress;


                using (SshClient client = new SshClient(ipaddress, login, password))
                {
                    client.Connect();
                    StringBuilder commandBuild = new StringBuilder();
                    commandBuild.AppendLine(@"#!/bin/bash");
                    commandBuild.AppendLine($"TempPass={password}");
                    commandBuild.AppendLine(commandinresult);
                    string commandExec = commandBuild.ToString().Replace("\r\n", "\n");
                    

                    var runCommand = client.CreateCommand(commandExec);
                    
                    string result = runCommand.Execute();
                    
                    client.Disconnect();
                    return result;
                }
            }
            catch (ArgumentException exception)
            {
                Log("Необходимо выбрать логин и пароль", true, true, exception.StackTrace, false);
                return "Ошибка Необходимо выбрать логин и пароль";
            }
            catch (Exception exception)
            {
                this.Log(exception.Message, true, true, exception.StackTrace, false);
                return "Ошибка";
            }

            //});
        }

        /// <summary>
        /// Метод копирования файла на хост
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <param name="pathToFile">путь к файлу</param>
        /// <param name="pathDestination">путь, куда будет скопирован файл</param>
        /// <param name="ipAddress">ip адрес хоста</param>
        /// <param name="login">логин, для подключения к хосту</param>
        /// <param name="password">пароль, для подключения к хосту</param>
        public void SftpCopy(string fileName, string pathToFile, string pathDestination, string ipAddress, string login, string password, bool addToListBoxTask = true)
        {
            try
            {


                bool endRunThread = false;
                var startThread = new Thread(new ThreadStart(() =>
                {
                    //BackgroundWorker progressBarCopyWorker = new BackgroundWorker();
                    PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(ipAddress, 22, login, password);
                    connectionInfo.Timeout = TimeSpan.FromSeconds(15);
                    string fileFullName = System.IO.Path.Combine(pathToFile, fileName);
                    string fileDestinationFullName = System.IO.Path.Combine(pathDestination, fileName);

                    ItemListBoxTask curItemLBT = new ItemListBoxTask() { IPOrName = ipAddress, Description = $"Выполняется копирование: {fileName} в {pathDestination}", CVZNumber = $"ЦВЗ№{SelectedTT.NumberCVZ.ToString()}" };


                    #region Скопировать файл на хост
                    // Проверка доступности файла 
                    if (File.Exists(fileFullName))
                    {
                        //Dispatcher.Invoke(() =>
                        //{

                        if (addToListBoxTask)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Bindings.listBoxTaskItemsSource.Add(curItemLBT);
                            });
                            //ListBoxTask.Items.Add(curItemLBT);
                        }
                        using (FileStream stream = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
                        {
                            curItemLBT.MaxValueProgressBar = stream.Length;
                            //progressBarStatus.Maximum = stream.Length;
                        }
                        //progressBarStatus.Value = 0;
                        //progressBarStatus.Visibility = Visibility.Visible;
                        //Bindings.StatusBarText = $"Выполняется копирование файла {fileName}";
                        bool uploadFileStoped = false;
                        UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBarStatus.SetValue);
                        //UpdateProgressBarDelegate updProgressItemTask = new UpdateProgressBarDelegate(progressBarStatus.SetValue);
                        using (var sftp = new SftpClient(connectionInfo))
                        {
                            try
                            {
                                sftp.Connect();

                                using (FileStream stream = new FileStream(fileFullName, FileMode.Open, FileAccess.Read))
                                {
                                    var startUpload = sftp.BeginUploadFile(stream, fileDestinationFullName, (asyncCallback) =>
                                    {
                                        //sftp.EndUploadFile(startUpload);
                                        endRunThread = true;

                                    }, null, (progressBarStatusCallBack) =>
                                    {

                                        var temp = curItemLBT.StopProcess;
                                        if (curItemLBT.StopProcess)
                                        {
                                            if (!uploadFileStoped)
                                            {
                                                try
                                                {
                                                    uploadFileStoped = true;
                                                    sftp.Disconnect();
                                                    endRunThread = true;
                                                }
                                                catch (Exception)
                                                {
                                                }


                                            }
                                        }
                                        Dispatcher.Invoke((Action)(() => { curItemLBT.CurValueProgressBar = (double)progressBarStatusCallBack; }));
                                        Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Render, new object[] { MetroProgressBar.ValueProperty, (double)progressBarStatusCallBack });
                                    });


                                    while (!startUpload.IsCompleted)
                                    {
                                        if (startUpload.IsCompleted)
                                        {
                                            stream.Close();
                                            stream.Dispose();
                                            endRunThread = true;
                                        }
                                    }

                                    Log(!uploadFileStoped ? $"Выполнено копирование {fileDestinationFullName} на хост {ipAddress}" : $"Прервано копирование {fileDestinationFullName} на хост {ipAddress}", false, true);
                                }


                            }
                            //catch(excep)
                            catch (Exception ex)
                            {
                                Log(!uploadFileStoped ? ex.Message : $"Прервано копирование {fileDestinationFullName} на хост {ipAddress}", false, true);
                                //Log(ex.Message, true, false, ex.StackTrace);
                            }
                            finally
                            {
                                try
                                {
                                    sftp.Disconnect();
                                    sftp.Dispose();
                                }
                                catch (Exception)
                                {

                                }


                            }
                        }
                    }
                    else
                        Log($"{fileFullName} не доступен", true, true);

                    #endregion
                    //}

                }));
                startThread.Start();

                //startThread.Suspend();

                //new Thread(new ThreadStart(() => 
                //{
                //    while (!endRunThread)
                //    {
                //        if (endRunThread)
                //        {
                //            int endD = 1;
                //        }
                //    }
                //})).Start();

            }
            catch (Exception ex)
            {
                Log(ex.Message, true, true, ex.StackTrace);
            }
        }

        private void TestScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SftpCopy("123.mkv", @"E:\futuron", @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password, true);
            }
        }

        private void ClearMotionHostPC_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Чистка Motion по хосту {curSelIP.NBNameOrIP()}", Properties.Resources.sshCommand_ClearMotion, SelectedCredentials.Login, SelectedCredentials.Password);

            }
        }

        private void ZabbixAgentUpdate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, $"Обновление Zabbix Agent по хосту {curSelIP.NBNameOrIP()}", Properties.Resources.sshCommand_ZabbixAgentUpdate, SelectedCredentials.Login, SelectedCredentials.Password);

            }
        }

        private void SmartCtl_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                StringBuilder commandSmartCtl = new StringBuilder();
                //commandSmartCtl.AppendLine("echo $TempPass|sudo -S su");
                commandSmartCtl.AppendLine("echo $TempPass|sudo -S apt-get -y install smartmontools");
                commandSmartCtl.AppendLine("echo $TempPass|sudo -S fdisk -l");
                commandSmartCtl.AppendLine("echo $TempPass|sudo -S smartctl --smart=on /dev/sda");
                commandSmartCtl.AppendLine("echo $TempPass|sudo -S smartctl -a /dev/sda");
                SSHCommandThread(curSelIP, $"S.M.A.R.T по хосту {curSelIP.NBNameOrIP()}", commandSmartCtl.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);

            }
        }

        private void DS_Benchmark_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;

                #region Скопировать DS_Benchmark.tar.gz на хост
                string pathTofileDS_Bencmark = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts");
                SftpCopy("DS_Benchmark.tar.gz", pathTofileDS_Bencmark, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);

                string pathTofilebench = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts");
                SftpCopy("bench.sh", pathTofilebench, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
                #endregion

                #region Выполнение скрипта тестирования DS_Benchmark

                SSHCommandThread(curSelIP, $"Тестирование производительности хоста {curSelIP.NBNameOrIP()} с помощью DS_Benchmark", @"echo $TempPass|sudo -S sh /home/user/bench.sh", SelectedCredentials.Login, SelectedCredentials.Password);

                #endregion
            }
        }

        private void Calendar1CDll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                CopyToClipboard("du /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll");
                SSHCommandThread(curSelIP, "Проверка наличия библиотеки /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll", "du /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll", SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void Calendar1CDlldownload_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                StringBuilder commandZimbraInstall = new StringBuilder();
                commandZimbraInstall.AppendLine(@"cp -a /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll.old");
                commandZimbraInstall.AppendLine(@"wget -N -t 5 -w 10 -O /home/user/.wine/drive_c/windows/system32/uiautomationcore.dll http://service.dengisrazy.ru:8080/scripts/uiautomationcore.dll");
                CopyToClipboard(commandZimbraInstall.ToString());
                SSHCommandThread(curSelIP, "Cохраняет существующую библиотеку под новым именем и скачивает новую библиотеку uiautomationcore.dll ", commandZimbraInstall.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void DоntOpenPDF1С_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                string pathTo = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts");
                SftpCopy("pdf1.txt", pathTo, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
                SftpCopy("pdf2.txt", pathTo, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
                StringBuilder commandDоntOpenPDF1С = new StringBuilder();
                commandDоntOpenPDF1С.AppendLine(@"regedit /home/user/pdf1.txt");
                commandDоntOpenPDF1С.AppendLine(@"regedit /home/user/pdf2.txt");
                SSHCommandThread(curSelIP, "Устранить проблему с открытием скана в 1С", commandDоntOpenPDF1С.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void Ink1CWine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(curSelIP.IPAddress, 22, SelectedCredentials.Login, SelectedCredentials.Password);
                connectionInfo.Timeout = TimeSpan.FromSeconds(30);

                #region Скопировать ibases.v8i и скрипт на хост
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        sftp.Connect();
                        string fileDS_Bencmark = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts\ibases.v8i");
                        using (FileStream stream = new FileStream(fileDS_Bencmark, FileMode.Open, FileAccess.Read))
                        {
                            sftp.UploadFile(stream, @"/home/user/ibases.v8i", true);
                            stream.Close();
                            stream.Dispose();
                        }

                        string filebench = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts\copyInk.sh");
                        using (FileStream stream = new FileStream(filebench, FileMode.Open, FileAccess.Read))
                        {
                            sftp.UploadFile(stream, @"/home/user/copyInk.sh", true);
                            stream.Close();
                            stream.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message, true, false, ex.StackTrace);
                    }
                    finally
                    {
                        sftp.Disconnect();
                        sftp.Dispose();
                    }
                }
                #endregion

                #region Выполнение скрипта тестирования DS_Benchmark

                SSHCommandThread(curSelIP, $"Копирование ярлыка со списком БД на хост {curSelIP.NBNameOrIP()}", @"echo $TempPass|sudo -S sh /home/user/copyInk.sh", SelectedCredentials.Login, SelectedCredentials.Password);

                #endregion
            }
        }

        private void LinphoneInstInk_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                string pathTo = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\LinuxScripts");
                SftpCopy("linphone_inst.sh", pathTo, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
                SftpCopy("linphone_inst_lnk.sh", pathTo, @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
                StringBuilder commandLinphoneInstInk = new StringBuilder();
                commandLinphoneInstInk.AppendLine("echo $TempPass|sudo -S bash linphone_inst.sh");
                commandLinphoneInstInk.AppendLine("echo $TempPass|sudo -S bash linphone_inst_lnk.sh");
                SSHCommandThread(curSelIP, "Установить linphone и вынести ярлыки на рабочий стол", commandLinphoneInstInk.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void AddAliasInBash_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                StringBuilder commandSmartCtl = new StringBuilder();

                //commandSmartCtl.AppendLine("echo $TempPass|sudo -S smartctl -a /dev/sda");
                SSHCommandThread(curSelIP, $"Добавить Alias в /etc/bash.bashrc {curSelIP.NBNameOrIP()}", Properties.Resources.sshCommand_AddAlias1C_reinstall, SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }



        private void RestartHostPC_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curIP = SelectedIP;
                new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(curIP.IPAddress, 22, SelectedCredentials.Login, SelectedCredentials.Password);
                        connectionInfo.Timeout = TimeSpan.FromSeconds(10);
                        using (SshClient client = new SshClient(connectionInfo))
                        {
                            client.Connect();
                            if (client.IsConnected)
                            {
                                try
                                {
                                    client.RunCommand($"echo {SelectedCredentials.Password}|sudo -S reboot");
                                }
                                catch (Exception)
                                {
                                    Log($"Выполнена перезагрузка хоста {curIP.IPAddress}");
                                }
                            }
                            client.Disconnect();
                        }
                    }
                    catch (ArgumentException exception)
                    {
                        Log("Необходимо выбрать логин и пароль", true, true, exception.StackTrace, false);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message, true, false, ex.StackTrace);
                    }
                })).Start();
            }
        }

        private void Scripts_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }






        private void HostPCInfo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //new Thread(new ThreadStart(HostPCInfoThread)).Start();
                //Dispatcher.InvokeAsync(() => { HostPCInfoThread(); });
                IP curSelIP = SelectedIP;
                SSHCommandThread(curSelIP, "Получить информацию о системе", Properties.Resources.sshCommand_SystemInfo2, SelectedCredentials.Login, SelectedCredentials.Password);

            }
        }

        private void WineComplect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                //SftpCopy("zimbra_desktop_update.tar.gz", @"\\nas.dengisrazy.ru\it\DSList\Актуальные скрипты Linux\Обновление почтового клиента Zimbra до версии 7.2.8", @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void ZimbraComplect_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                SftpCopy("zimbra_desktop_update.tar.gz", @"\\nas.dengisrazy.ru\it\DSList\Актуальные скрипты Linux\Обновление почтового клиента Zimbra до версии 7.2.8", @"/home/user/", curSelIP.IPAddress, SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void WineComplectScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //Log("Winbox MikroTik без параметров ", false, false, "", false);
                //ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\winbox.exe"), null, false, true, "", false, "", "");

                //StartProgramSupportTools("putty.exe",
                //    $"-ssh {SelectedIP.Address} -l {SelectedCredentials.Login} -pw {SelectedCredentials.Password}"); //-v ЦВЗ№{SelectedIP.Owner.NumberCVZ} {SelectedIP.Address} -l fawgv -pw 1

            }
        }

        private void ZimbraComplectScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                StringBuilder commandZimbraInstall = new StringBuilder();
                commandZimbraInstall.AppendLine("tar xzvf /home/user/zimbra_desktop_update.tar.gz");
                commandZimbraInstall.AppendLine("cd zimbra_desktop");
                commandZimbraInstall.AppendLine("echo $TempPass|sudo -S perl install.pl");
                commandZimbraInstall.AppendLine("perl /opt/zimbra/zdesktop/linux/user-install.pl");
                SSHCommandThread(curSelIP, $"Установка Zimbra 7.2.8 на хосте {curSelIP.NBNameOrIP()}", commandZimbraInstall.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }

        private void LockScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                //Log("Winbox MikroTik без параметров ", false, false, "", false);
                //ExecuteProgram(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\winbox.exe"), null, false, true, "", false, "", "");

                //StartProgramSupportTools("putty.exe",
                //    $"-ssh {SelectedIP.Address} -l {SelectedCredentials.Login} -pw {SelectedCredentials.Password}"); //-v ЦВЗ№{SelectedIP.Owner.NumberCVZ} {SelectedIP.Address} -l fawgv -pw 1
            }
        }

        private void UnLockScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Bindings.isipselected)
            {
                IP curSelIP = SelectedIP;
                StringBuilder commandUnlock = new StringBuilder();
                commandUnlock.AppendLine("echo $TempPass|sudo -S iptables -F && echo $TempPass|sudo -S iptables -X && echo $TempPass|sudo -S iptables -L");
                SSHCommandThread(curSelIP, $"Разблокировать интернет на хосте {curSelIP.NBNameOrIP()}", commandUnlock.ToString(), SelectedCredentials.Login, SelectedCredentials.Password);
            }
        }
    }
}
