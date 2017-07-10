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
using System.Collections;

namespace DSList
{
    public partial class NewMainWindow
    {
        ContextMenu tabContextMenu = new ContextMenu();
        public ContextMenu TabContextMenu { get { return tabContextMenu; } set { tabContextMenu = value; } }
        private static Customer CopyTT { get; set; }
        private static Customer CloseTT { get; set; }
        public IEnumerable<Customer> AllTT;
        public IEnumerable<Customer> FilteredTT;
        private bool _AutoOpen = false;

        public bool AutoOpen
        {
            get { return _AutoOpen; }
            set { _AutoOpen = value; }
        }

        private void InitSearchTimer()
        {
            this.SearchTimer.Stop();
            this.SearchTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            this.SearchTimer.Start();
        }

        private Customer FindTTbyCode(string code, IEnumerable<Customer> collection)
        {
            try
            {
                return collection.First<Customer>(p => (p.NumberCVZ.ToString() == code));
            }
            catch
            {
                return null;
            }
        }

        private void SelectedTT_OnCloseCounter(object sender)
        {
            if (OpenedTT.Count > 1)
            {
                this.OpenedTTRemove(sender as Customer);
            }
        }

        private void OpenTTinTab(Customer selected, bool Ping = false)
        {

            Task.Run(async delegate
            {
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
               {
                   try
                   {

                       TaskAwaiter awaiter;
                       Customer ExistTT;
                       if (Ping)
                       {
                           Ping = PingSubnetButton.IsEnabled;
                       }
                       if (NewMainWindow.settings.Fields.OneTab)
                       {
                           OpenedTTClear();
                       }
                       AutoOpen = false;
                       //MainWindow.Bindings.isttselected = true;
                       //if (SelectedTT != selected)
                       //{
                       //    //MainWindow.Bindings.isipselected = false;
                       //    //MainWindow.Bindings.title2 = string.Empty;
                       //}
                       //SelectedTT = selected;
                       TTPinComment.DataContext = SelectedTT;
                       subnet_addr_button.DataContext = SelectedTT;
                       //OnIPDetected += new TTEvent(SelectedTT_OnIPDetected);
                       if (NewMainWindow.settings.Fields.AutoCloseTab)
                       {
                           selected.OnCloseCounter -= new TTEvent(SelectedTT_OnCloseCounter);
                           selected.OnCloseCounter += new TTEvent(SelectedTT_OnCloseCounter);
                           selected.CloseCounter = 0;
                       }
                       NewMainWindow.CopyTT = selected;
                       if (OpenedTT.Contains(selected))
                       {
                           Ping = true;
                           ExistTT = FindTTbyCode(selected.NumberCVZ.ToString(), NewMainWindow.OpenedTT);
                           if (ExistTT != null)
                           {
                               OpenedTTRemove(ExistTT);
                           }
                           NewMainWindow.OpenedTT.Add(selected);
                           Tabs.SelectedIndex = Tabs.Items.IndexOf(selected);
                           //Tabs.ItemsSource = NewMainWindow.OpenedTT;
                           if (selected.CustomerType == CustomerTypes.TTCVZ)
                           {
                               selected.PopulateIPList();
                               selected.PopulateInfo();
                           }
                           Dispatcher.Invoke(async () => { await selected.LoadNotes(); });
                           ExistTT = null;
                       }
                       else
                       {
                           OpenedTT.Add(selected);
                           Tabs.SelectedIndex = Tabs.Items.IndexOf(selected);
                       }
                       MRUTT MRU_temp = new MRUTT();
                       MRU_temp.Code = selected.NumberCVZ.ToString();
                       MRU_temp.Name = selected.ToStringDisplay + " " + selected.Lan_Ip;
                       MRU_temp.Time = $"{DateTime.Now:dd/MM/yyyy hh:mm:ss tt}";
                       //MainWindow.MRU = new ObservableCollection<MRUTT>(MainWindow.MRU.Where<MRUTT>(new Func<MRUTT, bool>()).Take<MRUTT>(11).ToList<MRUTT>());
                       MRU = new ObservableCollection<MRUTT>((from i in MRU select i).Take(111).ToList());
                       //MainWindow.MRU = new ObservableCollection<MRUTT>();

                       NewMainWindow.MRU.Insert(0, MRU_temp);
                       ListViewMRU.ItemsSource = NewMainWindow.MRU;
                       SaveMRU();
                       if (Ping)
                       {
                           awaiter = PingSelectedTT(true).GetAwaiter();
                       }
                   }
                   catch (Exception ex)
                   {
                       Error(ex.Message, "Ошибка", ex.StackTrace);
                   }
               });
            });
        }

        private void InitFilteredTT(bool advancedSearch = false)
        {

            //this.FilteredTT = new List<Customer>();
            if (advancedSearch)
            {
                this.FilteredTT = from i in AllTT
                                  where i.NumberCVZ.ToString().Contains(TextBoxSearch.Text) || i.Address.ToLower().Contains(TextBoxSearch.Text.ToLower()) || i.City.ToLower().Contains(TextBoxSearch.Text.ToLower()) || i.Lan_Ip.Contains(TextBoxSearch.Text)
                                  select i;
            }
            else
                this.FilteredTT = from i in AllTT
                                  where i.NumberCVZ.ToString().Contains(TextBoxSearch.Text)
                                  select i;

            base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
            {
                this.searchListBox.ItemsSource = this.FilteredTT;
                this.TextBoxSearch.Focus();
                if (this.TextBoxSearch.Text.Length > 0)
                {
                    //this.InitSearchTimer();
                }
                else
                {
                    FillSearchListBoxAllTT();
                }
            });
        }



        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            this.FillSearchList();

            //if (!settings.Fields.AlwaysAdvSearch)
            //{
            //    this.FillSearchList();
            //}
            //else
            //{
            //    this.FillSearchListAdvanced();
            //}
        }

        /// <summary>
        /// Метод, обрабатывающий нажатие кнопки Esc (Не доработано)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EscSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSearch();
        }

        void ClearSearch()
        {
            if (TextBoxSearch.Text != null)
                this.TextBoxSearch.Clear();

            this.InitFilteredTT();
        }

        private void FillSearchListAdvanced()
        {
            //< FillSearchListAdvanced > d__182 stateMachine = new < FillSearchListAdvanced > d__182 {
            //    <> 4__this = this,
            //    <> t__builder = AsyncVoidMethodBuilder.Create(),
            //    <> 1__state = -1
            //      };
            //stateMachine.<> t__builder.Start << FillSearchListAdvanced > d__182 > (ref stateMachine);
        }

        private void FillSearchList(bool advSearch = false)
        {
            Task.Run(async delegate
            {
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
                {

                    try
                    {
                        SearchTimer.Stop();
                        var Search = TextBoxSearch.Text.Trim();
                        if (settings.Fields.AlwaysAdvSearch || advSearch)
                        {
                            InitFilteredTT(true);
                        }
                        else
                            InitFilteredTT();
                        searchListBox.ItemsSource = FilteredTT;
                        NewMainWindow.UpdateCollectionView(searchListBox.ItemsSource, NewMainWindow.settings.Fields.SearchGroupByStatus ? "Status" : "RegionType");
                        ShowTextOnStatusBar("Поиск: найдено " + FilteredTT.Count<Customer>() + " торговых точек", false, false);
                        if (searchListBox.Items.Count > 0)
                        {
                            searchListBox.SelectedIndex = 0;
                        }
                        //if ((FilteredTT.Count<Customer>() == 1) && (Search.Length >= 7))
                        //{
                        //    AutoOpen = true;
                        //}
                        if (NewMainWindow.settings.Fields.OneTab)
                        {
                            OpenedTTClear();
                        }
                        //if (AutoOpen && (searchListBox.SelectedIndex != -1))
                        //{
                        //    OpenTTinTab(searchListBox.SelectedItem as Customer, false);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Error(ex.Message, "Ошибка", ex.StackTrace);
                    }
                    progressBarStatus.Visibility = Visibility.Hidden;

                });
            });
        }

        public static void UpdateCollectionView(IEnumerable collection, string groupDescriptionString)
        {
            CollectionView defaultView = (CollectionView)CollectionViewSource.GetDefaultView(collection);
            PropertyGroupDescription item = new PropertyGroupDescription(groupDescriptionString);
            defaultView.GroupDescriptions.Clear();
            defaultView.GroupDescriptions.Add(item);
        }




        /// <summary>
        /// Метод Изменение текста в поисковой строке ЦВЗ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.InitSearchTimer();
        }


        /// <summary>
        /// Метод обработки события нажатия кнопки поиска "Расширенный поиск" (поиск производится не только по номеру ЦВЗ, но и по всем остальным полям)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchExtendedButton_Click(object sender, RoutedEventArgs e)
        {
            //SearchTimer.Stop();
            InitFilteredTT(true);
            //if (siteTable != null)
            //{
            //    if (TextBoxSearch.Text != "")
            //    {
            //        searchListBox.Items.Clear();
            //        //var listCVZ = mmf.FindCVZToListExtended(siteTable.GetTableFromSite(), TextBoxSearch.Text);


            //        //foreach (var customer in listCVZ)
            //        //{
            //        //    searchListBox.Items.Add(customer);
            //        //}
            //    }
            //}
        }

        /// <summary>
        /// Метод обработки события нажатия кнопки поиска "Поиск по IP адресу или имени ПК"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchIPOrName_Click(object sender, RoutedEventArgs e)
        {
            SearchIPOrName();
        }

        private void SearchIPOrName()
        {
            try
            {
                SearchTimer.Stop();
                if (VerifyCorrectIpAddressInString(this.TextBoxSearch.Text))
                {
                    // Проверка существования данного customer в ранее открытых
                    var getTab = (from i in OpenedTT where i.Lan_Ip == TextBoxSearch.Text select i).FirstOrDefault();
                    if (getTab != null)
                        /*getTab.Focus()*/;
                    else
                    {
                        Customer curCustomer = CreateCustomerForType(CustomerTypes.SingleIP, new Customer(this)/* { Owner = this }*/, TextBoxSearch.Text);
                        OpenTTinTab(curCustomer);
                        //OpenedTT.Add(curCustomer);
                        Log($"Открыт {curCustomer.Lan_Ip}");
                        /*curCustomer.Focus()*/;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Необходимо ввести корректный ip адрес: {ex.Message}", true, false, ex.StackTrace);
            }
        }

        /// <summary>
        /// Метод обработки события нажатия кнопки поиска "Открыть подсеть/Найти ПК"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchSubnetButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCustomSubnet();
        }

        /// <summary>
        /// Метод обработки Открыть подсеть/Найти ПК (Num Plus)
        /// </summary>
        private void OpenCustomSubnet()
        {
            try
            {
                SearchTimer.Stop();
                if (VerifyCorrectIpAddressInString(this.TextBoxSearch.Text))
                {
                    // Проверка существования данного customer в ранее открытых
                    var getTab = (from i in OpenedTT where i.Lan_Ip == TextBoxSearch.Text select i).FirstOrDefault();
                    if (getTab != null)
                        /*getTab.Focus()*/;
                    else
                    {
                        Customer curCustomer = CreateCustomerForType(CustomerTypes.SubnetIPArray, new Customer(this) /*{ Owner = this }*/);
                        OpenTTinTab(curCustomer);
                        //curCustomer.Lan_Ip
                        //OpenedTT.Add(curCustomer);
                        Log($"Открыта подсеть {curCustomer.Lan_Ip}");
                        /*curCustomer.Focus()*/;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Необходимо ввести корректный ip адрес: {ex.Message} ", true, false, ex.StackTrace);
            }
        }


        /// <summary>
        /// Обработка события двойного нажатия по элементу в списке найденных ЦВЗ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = (sender as ListBox);
            OnMouseClickSearchListBox(listBox);
        }

        /// <summary>
        /// Метод формирования шаблона для ListBoxItem searchListBox
        /// </summary>
        private void CreateSearchListBoxItemTemplate()
        {
            DataTemplate template = new DataTemplate(typeof(Customer));

            FrameworkElementFactory factoryStack = new FrameworkElementFactory(typeof(StackPanel));
            factoryStack.SetValue(StackPanel.OrientationProperty, System.Windows.Controls.Orientation.Horizontal);
            template.VisualTree = factoryStack;

            #region Формирование кнопки "Открыть" для searchListBox
            FrameworkElementFactory factoryButton = new FrameworkElementFactory(typeof(System.Windows.Controls.Button));
            factoryButton.SetValue(MarginProperty, new Thickness(2));
            factoryButton.SetValue(ForegroundProperty, Brushes.Indigo);
            factoryButton.SetValue(BackgroundProperty, Brushes.LightSteelBlue);
            factoryButton.SetValue(FontFamilyProperty, new FontFamily("Segoe UI Light"));
            factoryButton.SetValue(ContentProperty, "Открыть");
            factoryButton.AddHandler(PreviewMouseDownEvent, new MouseButtonEventHandler(searchListBox_PreviewMouseDownClick));
            factoryStack.AppendChild(factoryButton);
            #endregion

            //factoryStack.AddHandler

            //FrameworkElementFactory factoryRectangle = new FrameworkElementFactory(typeof(Rectangle));
            //factoryRectangle.SetValue(MarginProperty, new Thickness(2));
            //factoryRectangle.SetValue(HeightProperty, 16.0);
            //factoryRectangle.SetValue(WidthProperty, 50.0);
            //factoryRectangle.SetValue(Rectangle.StrokeProperty, SystemColors.WindowTextBrush);
            //factoryRectangle.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            #region Формирование LanIP в searchListBox
            //FrameworkElementFactory factoryLanIP = new FrameworkElementFactory(typeof(TextBlock));
            ////factoryLanIP.SetValue(BorderBrushProperty, Brushes.Black);
            //factoryLanIP.SetValue(MarginProperty, new Thickness(2));
            ////factoryLanIP.SetValue(BorderThicknessProperty, new Thickness(2));
            //factoryLanIP.SetValue(FontSizeProperty, 10.0);
            ////factoryLanIP.SetValue(BackgroundProperty, Brushes.Green);
            ////factoryLanIP.SetValue(PaddingProperty, new Thickness(5));

            //factoryLanIP.SetValue(ForegroundProperty, Brushes.Red);
            //factoryLanIP.SetValue(FontFamilyProperty, new FontFamily("Segoe UI Light"));
            //factoryLanIP.SetValue(TextBlock.TextProperty, new Binding("Lan_Ip"));
            //factoryLanIP.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            ////factoryStack.AppendChild(factoryLanIP);
            ////factoryRectangle.SetValue(ContentProperty, new Binding("Lan_Ip")/*new TextBlock() { Text = "sdf"}*/);

            ////factoryStack.AppendChild(factoryRectangle);

            //factoryStack.AppendChild(factoryLanIP);
            #endregion

            #region Формирование описания ЦВЗ для searchListBox
            FrameworkElementFactory factoryTextBlock = new FrameworkElementFactory(typeof(TextBlock));
            factoryTextBlock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            factoryTextBlock.SetValue(FontFamilyProperty, new FontFamily("Segoe UI Light"));
            factoryTextBlock.SetValue(TextBlock.TextProperty, new System.Windows.Data.Binding("ToStringDisplay"));

            factoryStack.AppendChild(factoryTextBlock);
            #endregion


            //factoryStack.SetValue()

            searchListBox.ItemTemplate = template;

            //searchListBox.Template.

            //testListBox.ItemTemplate = template;
            //testListBox.ItemsSource = AllTT;
        }

        /// <summary>
        /// Обработка события нажатия по кнопке "Открыть" элемента в списке найденных ЦВЗ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchListBox_PreviewMouseDownClick(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is ListBoxItem)
                {

                    Customer selCust = (dep as ListBoxItem).Content as Customer;
                    //var temp = from listItem in testListBox.SelectedValue where (listItem as ListBoxItem).Content == selCust select listItem;
                    searchListBox.SelectedValue = selCust;
                    OnMouseClickSearchListBox(searchListBox);
                }
            }
            //var listBox = (((sender as Button).Parent as StackPanel).TemplatedParent as ListBoxItem).Parent as ListBoxItem;
            /*testListBox.SelectedItem = */

        }

        private void OnMouseClickSearchListBox(object sender)
        {
            if ((sender as ListBox).GetType() == typeof(ListBox))
            {
                try
                {
                    SearchTimer.Stop();
                    if ((sender as ListBox).SelectedIndex != -1)
                    {
                        Customer selected = this.FindTTbyCode((this.searchListBox.SelectedItem as Customer).NumberCVZ.ToString(), AllTT);
                        // Проверка существования данного customer в ранее открытых
                        Customer getCustomer = FindTTbyCode(((sender as ListBox).SelectedItem as Customer).NumberCVZ.ToString(), OpenedTT);
                        if (getCustomer != null)
                        {
                            foreach (var item in Tabs.Items)
                            {
                                if (item==getCustomer)
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
                                curCustomer = CreateCustomerForType(CustomerTypes.TTCVZ, FindTTbyCode(((sender as ListBox).SelectedItem as Customer).NumberCVZ.ToString(), AllTT));
                            //}
                            //else
                                curCustomer = selected;
                            OpenTTinTab(curCustomer);
                            //OpenedTT.Add(curCustomer);
                            //RibbonMenuAuxiliaryPane.Items.Add(curCustomer.ToStringDisplay);
                            Log($"Открыта {curCustomer.ToStringDisplay}");
                            /*curCustomer.Focus()*/;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Ошибка при двойном нажатии по элементу в списке. " + ex.Message, true, true, ex.StackTrace);
                }
            }
            else if ((sender as ListView).GetType() == typeof(ListView))
            {
                try
                {
                    if ((sender as ListView).SelectedIndex != -1)
                    {
                        Customer selected = this.FindTTbyCode(((sender as ListView).SelectedItem as MRUTT).Code, AllTT);
                        // Проверка существования данного customer в ранее открытых
                        Customer getCustomer = FindTTbyCode(((sender as ListView).SelectedItem as MRUTT).Code, OpenedTT);
                        if (getCustomer != null)
                            /*getCustomer.Focus()*/;
                        else
                        {
                            Customer curCustomer;
                            //if (selected.Content == null)
                            //{
                            //    curCustomer = CreateCustomerForType(CustomerTypes.TTCVZ, FindTTbyCode(((sender as ListView).SelectedItem as MRUTT).Code, AllTT));
                            //}
                            //else
                                curCustomer = selected;
                            OpenTTinTab(curCustomer);
                            //OpenedTT.Add(curCustomer);
                            //RibbonMenuAuxiliaryPane.Items.Add(curCustomer.ToStringDisplay);
                            Log($"Открыта {curCustomer.ToStringDisplay}");
                            /*curCustomer.Focus()*/;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Ошибка при двойном нажатии по элементу в списке. " + ex.Message, true, true, ex.StackTrace);
                }
            }

        }

        /// <summary>
        /// Метод формирования Customer по CustomerTypes
        /// </summary>
        /// <param name="customerType">перечисление CustomerTypes</param>
        /// <param name="curCustomer">входной Customer, который обрабатывается</param>
        /// <returns></returns>
        private Customer CreateCustomerForType(CustomerTypes customerType, Customer curCustomer, string ipAddress = "127.0.0.1")
        {
            Customer outCustomer = curCustomer;
            switch (customerType)
            {
                case CustomerTypes.TTCVZ:
                    outCustomer.CreateStandartListIP();
                    outCustomer.DetectListIPType();
                    outCustomer.PingSubnet();
                    outCustomer.FillCustomer();
                    outCustomer.Owner = this;
                    outCustomer.PopulateInfo();
                    //outCustomer.CreateCustomerWithContentAndHeader();
                    //((outCustomer as TabItem).Header as FrameworkElement).ContextMenu = tabContextMenu;

                    return outCustomer;

                case CustomerTypes.SingleIP:
                    outCustomer.Lan_Ip = ipAddress;
                    outCustomer.IPArray = new ObservableCollection<IP>();
                    outCustomer.IPArray.Add(new IP()
                    {
                        Owner = outCustomer,
                        IPAddress = ipAddress,
                        
                    });
                    //outCustomer.NumberCVZ = 0;
                    outCustomer.PingSubnet();
                    outCustomer.Owner = this;
                    //outCustomer.CreateCustomerWithContentAndHeader();
                    //((outCustomer as TabItem).Header as FrameworkElement).ContextMenu = tabContextMenu;
                    return outCustomer;

                case CustomerTypes.SubnetIPArray:
                    outCustomer = curCustomer;
                    outCustomer.Lan_Ip = this.TextBoxSearch.Text;
                    outCustomer.PopulateIPList();
                    outCustomer.PingSubnet();
                    outCustomer.Owner = this;
                    //outCustomer.CreateCustomerWithContentAndHeader();
                    //((outCustomer as TabItem).Header as FrameworkElement).ContextMenu = tabContextMenu;
                    return outCustomer;

                default:
                    return curCustomer;
            }
        }
        

        /// <summary>
        /// Метод Установка фокуса на searchListBox
        /// </summary>
        private void SearchListBoxSetFocus()
        {
            try
            {
                if (this.searchListBox.Items.Count > 0)
                {
                    Keyboard.Focus(this.searchListBox);
                    if (this.searchListBox.SelectedIndex > -1)
                    {
                        ((UIElement)this.searchListBox.ItemContainerGenerator.ContainerFromItem(this.searchListBox.Items[this.searchListBox.SelectedIndex])).Focus();
                    }
                    else
                    {
                        ((UIElement)this.searchListBox.ItemContainerGenerator.ContainerFromItem(this.searchListBox.Items[0])).Focus();
                    }
                }
            }
            catch (Exception exception)
            {
                this.Log(exception.Message, true, false, exception.StackTrace, false);
            }
        }

        #region Old

        /// <summary>
        /// Метод формирования контекстного меню для TabCustomer
        /// </summary>
        private void TabContextMenuCreate()
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/close_16xlg.png"));  // Иконка копирования
            MenuItem menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Закрыть текущую вкладку";
            menuItem.Tag = 1;
            menuItem.Click += TabContextMenu_Click; ;
            tabContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/close_16xlg.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Закрыть все кроме текущей вкладки";
            menuItem.Tag = 2;
            menuItem.Click += TabContextMenu_Click;
            tabContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/close_16xlg.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Закрыть все вкладки";
            menuItem.Tag = 3;
            menuItem.Click += TabContextMenu_Click;
            tabContextMenu.Items.Add(menuItem);

        }

        #endregion
    }
}
