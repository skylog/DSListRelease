using AngleSharp.Dom.Html;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Cassia;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Parser.Html;
using System.IO;
using System.Diagnostics;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.Threading;

namespace DSList
{
    public partial class NewMainWindow
    {
        private void FillRusFIO()
        {
            try
            {
                //Thread.Sleep(30000);
                foreach (var userTS in listTSUser)
                {
                    //var temp = from i in listADUsers
                    //           where i.AccountName.ToLower() == item.UserName.ToLower()
                    //           select (Action)(() =>
                    //           {
                    //               item.RusName = i.NameUser;
                    //               item.RusMiddleName = i.MiddleName;
                    //               item.RusFamily = i.Family;
                    //           });
                    foreach (var userAD in listADUsers)
                    {
                        if (userAD.AccountName == userTS.UserName)
                        {
                            userTS.RusName = userAD.NameUser;
                            userTS.RusMiddleName = userAD.MiddleName;
                            userTS.RusFamily = userAD.Family;
                        }
                    }
                }
                dataGridTS.Items.Refresh();
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }

        }

        private void FillIPInTS()
        {
            try
            {
                foreach (var user in listTSUser)
                {
                    user.ClientIPAddress = (from i in listPcIPAndPcName where i.PcName.ToLower() == user.PCName.ToLower() select i.PcIP).FirstOrDefault();
                }
                dataGridTS.Items.Refresh();
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }

       

        private void LoadTSUsers()
        {

            try
            {
                listTSUser.Clear();
                TerminalServicesManager tsManager = new TerminalServicesManager();
                string[] massHosts = new string[] { "TS1", "TS2", "TS3", "TS4", "TS5", "TS6", "TS7" };

                foreach (var host in massHosts)
                {
                    using (ITerminalServer server = tsManager.GetRemoteServer(host))
                    {
                        server.Open();
                        foreach (ITerminalServicesSession session in server.GetSessions())
                        {
                            NTAccount account = session.UserAccount;
                            if (account != null)
                            {
                                TSUser user = new TSUser()
                                {
                                    DomainName = session.DomainName,
                                    PCName = session.ClientName,
                                    TSName = host,
                                    UserName = session.UserName,
                                    ConnectionState = session.ConnectionState.ToString(),
                                    WindowStationName = session.WindowStationName,
                                    UserAccount = account.ToString(),
                                };
                                Dispatcher.Invoke((Action)(() =>
                                {
                                    listTSUser.Add(user);
                                }));
                            }
                        }
                    }
                }

                foreach (var user in listTSUser)
                {
                    user.ClientIPAddress = (from i in listPcIPAndPcName where i.PcName.ToLower() == user.PCName.ToLower() select i.PcIP).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }

        private void CreateFileUsersAD()
        {
            try
            {
                ProcessStartInfo runCreateHTML = new ProcessStartInfo();
                runCreateHTML.FileName = "cmd.exe";
                string fileNamePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SupportTools");
                string fileName = System.IO.Path.Combine(fileNamePath, "AD-enabled-users.ps1");
                if (!File.Exists(fileName))
                {
                    Bindings.StatusBarText = $"{fileName} не найден";
                }
                string pathSupportTools = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SupportTools");
                //runCreateHTML.Arguments = $"/c cd {pathSupportTools} && powershell -ExecutionPolicy Bypass -file {fileName}";
                runCreateHTML.Arguments = $"/c cd {pathSupportTools} && powershell -ExecutionPolicy Bypass -file AD-enabled-users.ps1";

                //Process.Start(runCreateHTML);
                Process proc = new Process();
                proc.StartInfo = runCreateHTML;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }

        private void LoadFileWithADUsersInDataGrid()
        {
            try
            {
                listADUsers.Clear();
                UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBarStatus.SetValue);
                double value = 0;
                string fileHTMLPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "SupportTools");
                string fileHTML = System.IO.Path.Combine(fileHTMLPath, "AD-enabled-users.html");
                if (!File.Exists(fileHTML))
                {
                    Bindings.StatusBarText = $"{fileHTML} не найден";
                }
                var siteParse = new Url(fileHTML);
                
                var request = WebRequest.Create(siteParse);

                using (var responses = request.GetResponse())
                {
                    using (var streams = responses.GetResponseStream())
                    {
                        using (var readers = new StreamReader(streams))
                        {
                            var parser = new HtmlParser();
                            string source = readers.ReadToEnd();
                            DocumentADUsersHTML = parser.Parse(source);

                            foreach (var table in DocumentADUsersHTML.GetElementsByTagName("Table"))
                            {
                                progressBarStatus.Maximum = table.GetElementsByTagName("TR").Count();
                                progressBarStatus.Value = 0;
                                Bindings.StatusBarText = "Выполняется загрузка записей AD из файла";
                                //progressBarWorker.DoWork += new DoWorkEventHandler(PhoneChangeWorker_DoWork);
                                //progressBarWorker.RunWorkerCompleted += PhoneChangeWorker_RunWorkerCompleted;
                                //progressBarWorker.RunWorkerAsync();
                                #region Формирование словаря с данными
                                foreach (var str in table.GetElementsByTagName("TR"))
                                //for (int i = 0; i < table.GetElementsByTagName("TR").Count(); i++)
                                {
                                    Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Render, new object[] { MetroProgressBar.ValueProperty, ++value });

                                    string[] adUserTR = new string[10];
                                    int curCol = 0;
                                    // Формирование строк
                                    foreach (var curRow in str.GetElementsByTagName("TD"))
                                    {
                                        adUserTR[curCol] = curRow.TextContent;
                                        curCol++;
                                    }

                                    ADUser userAD = new ADUser()
                                    {
                                        WhenCreate = adUserTR[0],
                                        AccountName = adUserTR[1],
                                        Email = adUserTR[2],
                                        //MobilePhone = ADUser.FormatPhone(adUserTR[3]),
                                        MobilePhone = adUserTR[3],
                                        TelephoneNumber = adUserTR[4],
                                        Family = adUserTR[5],
                                        NameUser = adUserTR[6],
                                        MiddleName = adUserTR[7],
                                        Departament = adUserTR[8],
                                        Position = adUserTR[9],
                                    };
                                    listADUsers.Add(userAD);

                                    #endregion
                                }
                            }

                            Bindings.StatusBarText = "Загрузка записей AD из файла выполнена";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }

        public async void LoadListHostsInFile()
        {
            try
            {
                for (int y = 0; y < 3; y++)
                {
                    await Task.Run(async () =>
                    {
                        object x = await LoadTxt(@"172.16.5.0", "21");
                        foreach (IPAndName item in (List<IPAndName>)x)
                        {
                            await Dispatcher.BeginInvoke((Action)(() =>
                            {
                                if ((from i in listPcIPAndPcName
                                     where i.PcIP.ToLower() == item.PcIP.ToLower() && i.PcName.ToLower() == item.PcName.ToLower()
                                     select i).FirstOrDefault() == null)
                                {
                                    listPcIPAndPcName.Add(item);
                                }
                            }));
                        }

                    });
                    await Task.Run(async () =>
                    {
                        object x = await LoadTxt("172.16.11.0", "22");
                        foreach (IPAndName item in (List<IPAndName>)x)
                        {
                            await Dispatcher.BeginInvoke((Action)(() =>
                            {
                                if ((from i in listPcIPAndPcName
                                     where i.PcIP.ToLower() == item.PcIP.ToLower() && i.PcName.ToLower() == item.PcName.ToLower()
                                     select i).FirstOrDefault() == null)
                                {
                                    listPcIPAndPcName.Add(item);
                                }
                            }));
                        }
                    });
                    if (y == 2)
                    {
                        await Dispatcher.InvokeAsync((Action)(() =>
                        {
                            //Thread.Sleep(10000);
                            FillIPInTS();
                        }));

                    }
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }

        }

        public void CreateListIPNameFromFile()
        {
            try
            {
                string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, $"172.16.5.0List.txt");

                List<IPAndName> list = new List<IPAndName>();

                using (StreamReader str = new StreamReader(fileName))
                {
                    while (!str.EndOfStream)
                    {
                        string input = str.ReadLine();
                        string paternIP = @"([0-9]{1,3}[\.]){3}[0-9]{1,3}";
                        Match matchIP = Regex.Match(input, paternIP);
                        if (matchIP != null)
                        {
                            input = input.Replace(" ", "");
                            string[] workgroupAndName = input.Split(new char[] { ':' });
                            IPAndName curIPN = new IPAndName(matchIP.Value, "", workgroupAndName[1], workgroupAndName[4]);
                            list.Add(curIPN);
                        }
                    }
                }
                foreach (IPAndName item in list)
                {
                    if ((from i in listPcIPAndPcName
                         where i.PcIP.ToLower() == item.PcIP.ToLower() && i.PcName.ToLower() == item.PcName.ToLower()
                         select i).FirstOrDefault() == null)
                    {
                        listPcIPAndPcName.Add(item);
                    }
                }


                //object x = await LoadTxt("172.16.11.0", "24");
                fileName = System.IO.Path.Combine(Environment.CurrentDirectory, $"172.16.11.0List.txt");

                list = new List<IPAndName>();

                using (StreamReader str = new StreamReader(fileName))
                {
                    while (!str.EndOfStream)
                    {
                        string input = str.ReadLine();
                        string paternIP = @"([0-9]{1,3}[\.]){3}[0-9]{1,3}";
                        Match matchIP = Regex.Match(input, paternIP);
                        if (matchIP != null)
                        {
                            input = input.Replace(" ", "");
                            string[] workgroupAndName = input.Split(new char[] { ':' });
                            IPAndName curIPN = new IPAndName(matchIP.Value, "", workgroupAndName[1], workgroupAndName[4]);
                            list.Add(curIPN);
                        }
                    }
                }
                foreach (IPAndName item in list)
                {
                    if ((from i in listPcIPAndPcName
                         where i.PcIP.ToLower() == item.PcIP.ToLower() && i.PcName.ToLower() == item.PcName.ToLower()
                         select i).FirstOrDefault() == null)
                    {
                        listPcIPAndPcName.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }



        }

        private async Task<object> LoadTxt(string ipaddr, string subnet)
        {
            return await Task.Run((Func<List<IPAndName>>)(() =>
            {
                List<IPAndName> list = new List<IPAndName>();
                try
                {
                    ProcessStartInfo runNBTScan = new ProcessStartInfo();
                    runNBTScan.FileName = "cmd.exe";
                    string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, $"{ipaddr}List.txt");
                    string nbtscan = System.IO.Path.Combine(Environment.CurrentDirectory, @"SupportTools\nbtscan.exe");
                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    runNBTScan.Arguments = $"/c start /b {nbtscan} -s : {ipaddr}/{subnet} >{fileName}";

                    //Process.Start(runNBTScan);
                    Process proc = new Process();
                    proc.StartInfo = runNBTScan;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();

                    bool startStream = false;
                    while (!startStream)
                    {
                        try
                        {
                            using (StreamReader str1 = new StreamReader(fileName)) { };
                            startStream = true;
                        }
                        catch (Exception)
                        {

                        }
                    }



                    using (StreamReader str = new StreamReader(fileName))
                    {
                        while (!str.EndOfStream)
                        {
                            string input = str.ReadLine();
                            string paternIP = @"([0-9]{1,3}[\.]){3}[0-9]{1,3}";
                            Match matchIP = Regex.Match(input, paternIP);
                            if (matchIP != null)
                            {
                                input = input.Replace(" ", "");
                                string[] workgroupAndName = input.Split(new char[] { ':' });
                                IPAndName curIPN = new IPAndName(matchIP.Value, "", workgroupAndName[1], workgroupAndName[4]);
                                list.Add(curIPN);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Bindings.StatusBarText = ex.Message;
                }

                return list;

            }));



        }
    }
}
