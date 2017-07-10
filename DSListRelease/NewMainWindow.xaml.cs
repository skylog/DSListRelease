using AngleSharp.Dom.Html;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Microsoft.Shell;
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
using System.Windows.Controls.Ribbon;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Windows.Threading;
using System.Reflection;
//using System.Windows.Forms;

namespace DSList
{
    delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

    /// <summary>
    /// Логика взаимодействия для NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : INotifyPropertyChanged
    {
        #region Реализация интерфейса InotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion




        #region Поля
        public static LogWriter MainLog = LogWriter.Instance;

        public static System.Windows.Forms.NotifyIcon cvzNotifyIcon = new System.Windows.Forms.NotifyIcon();


        internal static string DBPath { get; set; }
        private DispatcherTimer SearchTimer = new DispatcherTimer();
        private DispatcherTimer UpdateTimer = new DispatcherTimer();

        /// <summary>
        /// Поле, содержащее текущую версию сборки
        /// </summary>
        private string version;

        /// <summary>
        ///Поле таблица данных сайта
        /// </summary>
        //TableFromSite siteTable;

        static IP _SelectedIP;

        /// <summary>
        /// Поле класс, поддерживающий методы работы с таблицей сайта
        /// </summary>
        //MethodsTableFromSite mmf = new MethodsTableFromSite();

        /// <summary>
        /// Поле static класса Settings, включает в себя основные настройки программы
        /// </summary>
        public static Settings settings = new Settings();
        private static Customer _SelectedTT;
        //private IP _SelectedIPTest;


        public static readonly DependencyProperty isipselectedProperty = DependencyProperty.RegisterAttached("isipselected", typeof(bool), typeof(NewMainWindow)/*, new PropertyMetadata(false)*/);
        public static readonly DependencyProperty isttselectedProperty = DependencyProperty.Register("isttselected", typeof(bool), typeof(NewMainWindow)/*, new PropertyMetadata(false)*/);



        #endregion

        #region Свойства

        ///// <summary>
        ///// Свойство признака выбранности хоста
        ///// </summary>

        public bool isipselected
        {
            get
            {
                if (SelectedIP != null)
                {
                    return (bool)GetValue(isipselectedProperty);
                }
                else
                {
                    return false;
                }

            }
            set
            {
                if (SelectedIP != null)
                    SetValue(isipselectedProperty, true);
                else
                    SetValue(isipselectedProperty, false);
                NotifyPropertyChanged("isipselected");
            }
        }

        ///// <summary>
        ///// Свойство признака выбранности Customer
        ///// </summary>
        public bool isttselected
        {
            get
            {
                return (bool)GetValue(isttselectedProperty);
            }
            set
            {
                if (SelectedTT != null)
                    SetValue(isttselectedProperty, true);
                else
                    SetValue(isttselectedProperty, false);
            }
        }

        /// <summary>
        /// Выбранный в настоящий момент Customer( ЦВЗ )
        /// </summary>
        public Customer SelectedTT
        {
            get { return _SelectedTT; }
            set
            {
                _SelectedTT = value;
                isttselected = true;
                NotifyPropertyChanged("SelectedTT");
                //NotifyPropertyChanged("isttselected");
            }
        }

        private static bool IsWindows10 { get; set; }

        /// <summary>
        /// Свойство, переменные связывания
        /// </summary>
        public static BindingVariables Bindings { get; set; }

        /// <summary>
        /// Выделенный IP в Customer
        /// </summary>
        public IP SelectedIPInCustomer
        {
            get { return _SelectedIP; }
            set
            {
                _SelectedIP = value;
                isipselected = true;
                NotifyPropertyChanged("SelectedIP");
                NotifyPropertyChanged("isipselected");
                NotifyPropertyChanged("isipselectedProperty");
            }
        }


        public IP SelectedIP
        {
            get
            {
                //if (SelectedTT != null)
                //{
                //    if (SelectedTT.ListViewHostsInCustomer.SelectedIndex != -1)
                //    {
                //        //var i = SelectedTT.ListViewHostsInCustomer.SelectedItem;
                //        return SelectedTT.ListViewHostsInCustomer.SelectedItem as IP;
                //    }

                //    else
                //        return null;
                //}
                //else
                //    return null;
                return _SelectedIP;
            }
        }


        #endregion

        public NewMainWindow()
        {
            try
            {
                Log("<-- Запуск DSList -->");
                InitializeComponent();


                Bindings = new BindingVariables();
                DataContext = Bindings; // Выполняется привязка контекста MainWindows к BindingVariables для корректной работы привязки элементов, прописанных в Xaml

                #region Поиск сотрудников в TS и формирование списка AD


                Bindings.VisibilitySetting = Visibility.Collapsed;
                progressBarWorker = new BackgroundWorker();

                listPcIPAndPcName = new ObservableCollection<IPAndName>();

                listADUsers = new ObservableCollection<ADUser>();
                searchListADUser = new ObservableCollection<ADUser>();

                listTSUser = new ObservableCollection<TSUser>();
                searchListTSUser = new ObservableCollection<TSUser>();

                dataGridTS.ItemsSource = listTSUser;
                dataGridAD.ItemsSource = listADUsers;
                dataGridIPPC.ItemsSource = listPcIPAndPcName;
                dataGridIPPC.HorizontalContentAlignment = HorizontalAlignment.Stretch;


                searchListBoxAD.ItemsSource = searchListADUser;
                searchListBoxTS.ItemsSource = searchListTSUser;

                #endregion

                this.version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Bindings.title = "DSList " + this.version;
                IsWindows10 = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(Environment.SystemDirectory, "Kernel32.dll")).ProductMajorPart >= 10;
                Bindings.IsTSConnecting = Environment.UserDomainName.ToLower().Contains("dengisrazy") ? true : false;
                NewMainWindow.settings.OnSaveOrLoad += settings_OnSaveOrLoad;
                Title = Bindings.title;

                //siteTable = new TableFromSite();

                //TabCtrl.SelectionChanged += TabCtrl_SelectionChanged;
                Tabs.SelectionChanged += Tabs_SelectionChanged;

                //searchListBox.DisplayMemberPath = "ToStringDisplay";    // Представление отображения в списке найденных Customer

                cvzNotifyIcon.Icon = Properties.Resources.vsnetwebservicedynamicdiscovery_8215;
                cvzNotifyIcon.Text = "DSList";
                cvzNotifyIcon.Visible = true;
                cvzNotifyIcon.Click += new EventHandler(this.ni_Click);
                if (DSList.Properties.Settings.Default.Maximized)
                {
                    base.WindowState = WindowState.Maximized;
                }

                this.SearchTimer.Tick += new EventHandler(this.SearchTimer_Tick);

                this.SearchADTimer.Tick += new EventHandler(this.SearchADTimer_Tick);

                //this.UpdateTimer.Tick += new EventHandler(this.UpdateTimer_Tick);

                this.UpdateTimer.Interval = new TimeSpan(0, 0, 15, 0, 0);
                this.UpdateTimer.Start();

                LoadHotkeys();
                LoadUserCredentials();
                SelectedLoginCreate();
                CreateNewCredentials();    // Формируется список логинов и паролей, прописанных в методе CreateCredentials()  
                LoadCredentialsFromSQLServer();


                NewMainWindow.MRU = new ObservableCollection<MRUTT>();
                LoadMRU();
                ListViewMRU.ItemsSource = MRU;
                //ListViewMRU.MouseDoubleClick += (object sender, MouseButtonEventArgs args) => { };
                ListViewMRU.MouseDoubleClick += MRUItem_Click;

                //TabCtrl.ItemsSource = OpenedTT;

                //Tabs.ItemsSource = OpenedTT;


                TabContextMenuCreate();

                LoadPopup();

                RefreshCredentials();
                //TabCtrl.ContextMenu = tabContextMenu;
                #region PinnedWatchTimer
                this.PinnedWatchTimer.Tick += new EventHandler(this.PinnedWatchTimer_Tick);
                this.PinnedWatchTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                this.PinnedWatchTimer.Start();
                #endregion

                //ListViewIP.DataContext = SelectedTT;

                TextBoxSearch.Focus();

            }
            catch (Exception ex)
            {
                Log("Ошибка загрузки основного окна MainWindow. " + ex.Message, true, false, ex.StackTrace);
            }

        }

        private void NewMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Log("NewMainWindow_Loaded");
            try
            {
                try
                {
                    App.splashScreen.LoadComplete();
                    //base.Focus();
                }
                catch (Exception ex)
                {
                    Bindings.StatusBarText = ex.Message;
                }
                NewMainWindow.PinnedTTs = new ObservableCollection<PinnedTT>();
                LoadPinnedTTs();
                ListViewPinned.ItemsSource = NewMainWindow.PinnedTTs;
                NewMainWindow.UpdateCollectionView(ListViewPinned.ItemsSource, "Color");
                CreateSearchListBoxItemTemplate();
                //TabCtrl.DataContext = this;
                //TabCtrl.SetBinding(System.Windows.Controls.TabControl.SelectedItemProperty, new System.Windows.Data.Binding("SelectedTT"));

                Tabs.DataContext = this;
                //Tabs.SetBinding(System.Windows.Controls.TabControl.SelectedItemProperty, new System.Windows.Data.Binding("SelectedTT"));

                System.Windows.Data.Binding isipselBind = new System.Windows.Data.Binding("isipselected");
                isipselBind.Source = Bindings;
                isipselBind.Mode = BindingMode.TwoWay;
                SetBinding(isipselectedProperty, isipselBind);


                System.Windows.Data.Binding isttselBind = new System.Windows.Data.Binding("isttselected");
                isttselBind.Source = Bindings;
                isttselBind.Mode = BindingMode.TwoWay;
                SetBinding(isttselectedProperty, isttselBind);

                string fileHelpHTML = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"html\help.htm");
                frameHelp.Source = new Uri(fileHelpHTML);

                string fileVersionsHTML = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"html\versions.htm");
                frameVersions.Source = new Uri(fileVersionsHTML);

                //Binding listBoxTaskISBinding = new Binding("listBoxTaskItemsSource");
                //listBoxTaskISBinding.Source = Bindings;
                //listBoxTaskISBinding.Mode = BindingMode.TwoWay;
                //ListBoxTask.SetBinding(ListBox.ItemsSourceProperty, listBoxTaskISBinding);

                //System.Windows.Data.Binding commentBind = new System.Windows.Data.Binding("Comment");
                //isipselBind.Source = SelectedTT;
                //isipselBind.Mode = BindingMode.TwoWay;
                //SetBinding(isipselectedProperty, isipselBind);
                //SelectedTT.SetBinding(Customer.CommentProperty, "Comment");

                //Bindings.isipselected = new System.Windows.Data.Binding(isipselectedProperty, isipselBind);


                //SelectedTT.SetBinding(System.Windows.Controls.TabControl.SelectedItemProperty, "SelectedTT");
                ////SelectedTT.DataContext = this;
                ////SelectedTT.SetBinding(Customer.SelectedIPProperty, "SelectedIP");
                //TabCtrl.SetBinding(System.Windows.Controls.TabControl.SelectedItemProperty, )
            }
            catch (Exception ex)
            {
                Log(ex.Message, true, false, ex.StackTrace, false);
            }
            //Log("NewMainWindow_Loaded_End");

        }

        public bool ProcessCommandLineArgs(IList<string> args)
        {
            if (((args != null) && (args.Count != 0)) && (args.Count > 1))
            {
                char[] trimChars = new char[] { '"' };
                this.SearchClipboardText(args[1].Trim(trimChars));
            }
            return true;
        }

        private void SearchClipboardText(string clipboardText)
        {
            if (clipboardText.Length < 30)
            {
                if (clipboardText.Contains<char>('\n'))
                {
                    clipboardText = clipboardText.Remove(clipboardText.IndexOf('\n'));
                }
                this.TextBoxSearch.Text = string.Empty;
                this.TextBoxSearch.Text = clipboardText.Trim();
                this.AutoOpen = true;
            }
        }

        protected internal void FillSearchListBoxAllTT()
        {
            //searchListBox.Items.Clear();
            searchListBox.ItemsSource = AllTT;
        }


        ~NewMainWindow()
        {
            Log("<-- Закрытие DSList -->");
            cvzNotifyIcon.Dispose();
        }

        /// <summary>
        /// Метод обработки изменения выбора вкладок Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isipselected = true;
            if (Tabs.SelectedIndex != -1)
            {
                System.Windows.Data.Binding comTabBind = new System.Windows.Data.Binding("Comment");
                comTabBind.Source = SelectedTT;

                TTPinComment.SetBinding(System.Windows.Controls.TextBox.TextProperty, comTabBind);
            }
            else
                TTPinComment.Text = "";
        }

        /// <summary>
        /// Метод обработки изменения выбора вкладок Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TabCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    isipselected = true;
        //    if (TabCtrl.SelectedIndex != -1)
        //    {
        //        System.Windows.Data.Binding comTabBind = new System.Windows.Data.Binding("Comment");
        //        comTabBind.Source = (sender as System.Windows.Controls.TabControl).SelectedItem as Customer;

        //        TTPinComment.SetBinding(System.Windows.Controls.TextBox.TextProperty, comTabBind);
        //    }
        //    else
        //        TTPinComment.Text = "";
        //    //if ((sender as System.Windows.Controls.TabControl).SelectedIndex != -1)
        //    //{
        //    //    //System.Windows.Forms.MessageBox.Show("Test");
        //    //    //if (((sender as System.Windows.Controls.TabControl).SelectedItem as Customer).ListViewHostsInCustomer.Items != null)
        //    //    //{
        //    //    //    ((sender as System.Windows.Controls.TabControl).SelectedItem as Customer).ListViewHostsInCustomer.SelectedIndex = 0;
        //    //    //}
        //    //}
        //    //if ((sender as System.Windows.Controls.TabControl).SelectedIndex != -1)
        //    //{
        //    //    SelectedTT = (sender as System.Windows.Controls.TabControl).SelectedItem as Customer;
        //    //    Bindings.isttselected = true;
        //    //    if ((from i in PinnedTTs where i.Code == SelectedTT.NumberCVZ.ToString() || i.IPAddress == SelectedTT.Lan_Ip select i).FirstOrDefault()!=null)
        //    //    {
        //    //        //TTPinComment.Text = (from i in PinnedTTs where i.Code == SelectedTT.NumberCVZ.ToString() || i.IPAddress == SelectedTT.Lan_Ip select i).FirstOrDefault().Comment;
        //    //    }
        //    //    else
        //    //    {
        //    //        //TTPinComment.Text = "";
        //    //    }
        //    //    var listViewHostsInCustomer = ((sender as System.Windows.Controls.TabControl).SelectedItem as Customer).ListViewHostsInCustomer;
        //    //    if (listViewHostsInCustomer.SelectedIndex!=-1)
        //    //    {
        //    //        SelectedIP = (((listViewHostsInCustomer.SelectedItem as System.Windows.Controls.ListViewItem).Content) as IP);
        //    //        Bindings.isipselected = true;
        //    //    }
        //    //    else
        //    //    {
        //    //        SelectedIP = null;
        //    //        Bindings.isipselected = false;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    SelectedIP = null;
        //    //    SelectedTT = null;
        //    //    Bindings.isipselected = false;
        //    //    Bindings.isttselected = false;
        //    //    //TTPinComment.Text = "";
        //    //}

        //}


        #region Всплывающая подсказка (низ право)

        /// <summary>
        /// Метод отработки нажатия NotifyIcon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ni_Click(object sender, EventArgs e)
        {
            if ((e as System.Windows.Forms.MouseEventArgs).Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.ActivateMainWindow();
            }
            else
            {
                this.MenuOpen("TrayMenu", sender as FrameworkElement);
            }
        }

        #endregion


        private void MenuOpen(string menuName, FrameworkElement placementTarget = null)
        {
            System.Windows.Controls.ContextMenu menu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem exit = new System.Windows.Controls.MenuItem();
            exit.Header = "Выход";
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/close_16xlg.png"));  // Иконка копирования
            exit.Icon = image;
            exit.Click += Exit;
            menu.Items.Add(exit);
            menu.DataContext = this;

            if (placementTarget != null)
            {
                menu.PlacementTarget = placementTarget;
            }
            menu.IsOpen = true;
            if (SelectedTT != null)
            {
                SelectedTT.CloseCounter = 0;
            }
        }


        private void ActivateMainWindow()
        {
            if ((base.Left > (SystemParameters.VirtualScreenWidth - 20.0)) || (base.Top > (SystemParameters.VirtualScreenHeight - 50.0)))
            {
                base.Left = 50.0;
                base.Top = 50.0;
            }
            base.Visibility = Visibility.Visible;
            base.ShowInTaskbar = true;
            if (base.WindowState == WindowState.Minimized)
            {
                base.WindowState = WindowState.Normal;
            }
            base.Activate();
            this.TextBoxSearch.Focus();
        }

        /// <summary>
        /// Загрузка вендоров mac адресов из файла SupportTools\mac.dat
        /// </summary>
        void LoadMACVendorsDB()
        {
            try
            {
                macvendors = System.IO.File.ReadAllLines(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"SupportTools\mac.dat"));
            }
            catch (Exception ex)
            {
                Log("Ошибка загрузки mac адресов из файла SupportTools\\mac.dat." + ex.Message, true, false, ex.StackTrace);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            cvzNotifyIcon.Dispose();
            base.OnClosed(e);
            Close();
            App.Current.Shutdown();
            Environment.Exit(0);
        }


        /// <summary>
        /// Метод, обрабатывающий нажатие кнопки выхода из программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }




        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.ComboBox).DataContext = this.SelectedCredentials;
            //if (settings.Fields.ChangeLayout)
            //{
            //    InputLanguageManager.SetInputLanguage(sender as System.Windows.Controls.ComboBox, CultureInfo.CreateSpecificCulture("en-US"));
            //}
        }






        /// <summary>
        /// Метод обработки нажатия кнопки помощи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                listBox.SelectedItem = lbiHelp;
            }
            catch (Exception ex)
            {
                Log("Ошибка при открытии справкиру", true, true, ex.StackTrace);
            }


        }

        /// <summary>
        /// Метод, обрабатывающий нажатие кнопки "Журнал"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileOrFolder(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Roaming\DSList\Logs"), "");
        }

        private void OpenFileOrFolder(string folder, string args = "")
        {
            this.Log("Открыть " + folder, false, false, "", false);
            Task.Run(delegate
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        Process.Start(folder);
                    }
                    else
                    {
                        Process process = new Process
                        {
                            StartInfo = {
                                FileName = folder,
                                WorkingDirectory = System.IO.Path.GetDirectoryName(folder)
                            }
                        };
                        if (!string.IsNullOrWhiteSpace(args))
                        {
                            process.StartInfo.Arguments = args;
                        }
                        process.Start();
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message, true, true, ex.StackTrace);
                }
            });
        }





        #region TEST

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }

        /// <summary>
        /// Формирование стандартного списка в тестовой сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void LoadSiteToTable_Click(object sender, RoutedEventArgs e)
        //{
        //    bool createNewCustomer = true;
        //    foreach (Customer tab in TabCtrl.Items)
        //    {
        //        if (tab.Lan_Ip == "192.168.61.254")
        //        {
        //            tab.Focus();
        //            createNewCustomer = false;
        //        }
        //    }
        //    if (createNewCustomer)
        //    {
        //        Customer newCust = new Customer(this) /*{ Owner = this }*/;
        //        newCust.Lan_Ip = "192.168.61.254";
        //        newCust.CreateStandartListIP();
        //        newCust.Owner = this;
        //        newCust.DetectListIPType();
        //        newCust.PingSubnet();
        //        newCust.CreateCustomerWithContentAndHeader();
        //        TabCtrl.Items.Add(newCust);
        //        newCust.Focus();
        //    }

        //}

        #endregion





        /// <summary>
        /// Метод обработки нажатия кнопки "Кексик"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CakeButton_Click(object sender, RoutedEventArgs e)
        {

            if (SelectedCredentials.Password == "WarY5Mtz9")
            {
                if (Bindings.VisibilitySetting == Visibility.Visible)
                {
                    Bindings.VisibilitySetting = Visibility.Collapsed;
                }
                else if (Bindings.VisibilitySetting == Visibility.Collapsed)
                {
                    Bindings.VisibilitySetting = Visibility.Visible;
                }
            }

        }

        /// <summary>
        /// Обработка нажатия на надпись DSList в MainWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DSListMainLabel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileOrFolder(@"\\nas.dengisrazy.ru\it\DSList\Install", "");
        }

        private void NewMainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.CheckAllWindowHotKey(ref e) && !this.CheckCloseTabHotKey(ref e))
            {

            }
        }

        private void SelIPCredentialFill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedCredentials.Login = (from credLogin in NewMainWindow.Credentials
                                             where credLogin.HostType == SelectedIP.Type
                                             select credLogin).FirstOrDefault().Login;
            }
            catch (Exception)
            {
                SelectedCredentials.Login = (from credLogin in NewMainWindow.Credentials
                                             where credLogin.HostType == IPType.Общий
                                             select credLogin).FirstOrDefault().Login;
            }
        }

        private void MikroTikwinboxButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string winbox;
                string arguments;
                Log("Winbox MikroTik " + SelectedTT.Lan_Ip, false, false, "", false);
                arguments = $"{SelectedTT.Lan_Ip} dsadmin WarY5Mtz9 ";

                if (settings.Fields.winboxmikrotik_path_usedefault && File.Exists(settings.Fields.winboxmikrotikpath))
                    winbox = settings.Fields.winboxmikrotikpath;
                else
                    winbox = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\winbox.exe");

                ExecuteProgram(winbox, arguments, false, true, "", false, "", "");
            }
            catch (Exception ex)
            {
                Log("Необходимо выбрать ЦВЗ", true, false, ex.StackTrace);
            }
        }


        private void JasperButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string putty;
                string arguments;
                this.Log("Jasper Putty SSH " + SelectedIP.NBNameOrIP(), false, false, "", false);
                arguments = $"-ssh {SelectedCredentials.Login}@{SelectedIP.IPAddress} 22";
                if (Bindings.IsTSConnecting)
                    arguments = $"-ssh otp@zabbix.dengisrazy.ru 22 -pw 123";
                else
                    arguments = $"-ssh otp@zabbix.vpn.dengisrazy.ru 22 -pw 123";

                if (System.IO.File.Exists(settings.Fields.putty_path))
                    putty = settings.Fields.putty_path;
                else
                    putty = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SupportTools\putty.exe");

                ExecuteProgram(putty, arguments, false, true, "", false, "", "");
            }
            catch (Exception ex)
            {
                Log(ex.Message, true, false, ex.StackTrace);
            }
        }

        /// <summary>
        /// Метод обработки нажатия правой кнопки мыши на ссылке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRef_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CopyToClipboard($"Адрес {(sender as Button).Tag.ToString()}", true, "", true);
        }

        private void CloseItemListTaks_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dep = (DependencyObject)e.OriginalSource;
                while (dep != null)
                {
                    dep = VisualTreeHelper.GetParent(dep);
                    if (dep is ListBoxItem)
                    {
                        // System.Windows.Forms.MessageBox.Show((ListBoxTask.SelectedIndex).ToString());
                        //System.Windows.Forms.MessageBox.Show(((dep)==ListBoxTask.SelectedItem).ToString());
                        ((dep as ListBoxItem).Content as ItemListBoxTask).StopProcess = true;
                        //ListBoxTask.Items.RemoveAt(ListBoxTask.SelectedIndex);
                        Bindings.listBoxTaskItemsSource.RemoveAt(ListBoxTask.SelectedIndex);

                        //ItemListBoxTask selItemListBoxTask = (dep as ListBoxItem).Content as ItemListBoxTask;
                        //ListBoxTask.SelectedValue = selItemListBoxTask;

                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, true, false, ex.StackTrace);
            }


            //foreach (var itemLB in ListBoxTask.Items)
            //{
            //    foreach (var item in FindLogicalChildren<TextBlock>(itemLB as ListBoxItem))
            //    {
            //        if (item==(sender as TextBlock))
            //        {
            //            ListBoxTask.Items.Remove(itemLB);
            //            return;
            //        }
            //    }
            //}

        }

        

       
    }
}
