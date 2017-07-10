using MahApps.Metro.Controls;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
//using System.Drawing;
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
using DSList.OpenSourceControls;

namespace DSList
{
    public partial class Customer : CustomerLogic
    {
        #region Конструкторы


        public Customer(NewMainWindow owner)
        {
            Owner = owner;
            this.Hosts = new List<Host>();
            this.Info = new ObservableCollection<BindingListItem>();
            this.Employees = new List<Employee>();
            //ContextMenuCreate();
            //CustomerContextMenuCreate();
            Provider = new ProvInfo();
            Provider.Info = new ObservableCollection<BindingListItem>();
            //LoadPopup();
            ListViewHostsInCustomer = new ListView();
            ListViewHostsInCustomer.DataContext = this;

            //Binding myBind = new Binding("SelectedIPTest");
            //Binding selIPCust = new Binding("SelectedIPInCustomer");
            //selIPCust.Source = Owner;

            //Binding selIPCust = new Binding("SelectedIPInCustomer");
            //selIPCust.Source = Owner;

            //SetBinding(SelectedIPProperty, selIPCust);

            //Binding commentCust = new Binding("Text");
            //commentCust.Source = Owner.TTPinComment;
            //this.SetBinding(CommentProperty, commentCust);


            //ListViewHostsInCustomer.SetBinding(System.Windows.Controls.ListView.SelectedItemProperty, selIPCust);

            //Binding ipArrayBind = new Binding("IPArray");
            //ipArrayBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //ipArrayBind.Source = this;
            //ipArrayBind.Source = IPArray;
            //ipArrayBind.Mode = BindingMode.OneWay;
            //ListViewHostsInCustomer.SetBinding(ListView.ItemsSourceProperty, ipArrayBind);

            //FillCustomer();
        }





        #endregion


        #region Методы

        

        

        

        /// <summary>
        /// Метод формирования шапки TabItemHeader
        /// </summary>
        /// <returns></returns>
        private FrameworkElement CreateTabHead()
        {
            DockPanel headerContent = new DockPanel();
            StackPanel sp = new StackPanel();

            //TextBlock tbCVZNumber = new TextBlock()
            //{
            //    Text = this.NumberCVZ != 0 ? $"ЦВЗ №{this.NumberCVZ}": Lan_Ip,
            //    FontFamily = new FontFamily("Segoe UI Light"),
            //};
            //sp.Children.Add(tbCVZNumber);

            TextBlock tbSubnet = new TextBlock()
            {
                Text = this.NumberCVZ != 0 ? $"ЦВЗ №{this.NumberCVZ}" : "Подсеть офиса",
                FontFamily = new FontFamily("Segoe UI Light"),
            };
            sp.Children.Add(tbSubnet);

            TextBlock tbLanIP = new TextBlock()
            {
                Text = this.Lan_Ip,
                FontFamily = new FontFamily("Segoe UI Light"),
            };
            sp.Children.Add(tbLanIP);

            headerContent.Children.Add(sp);

            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/deletehs.png"));
            Button hBt = new Button()
            {
                BorderThickness = new Thickness(0),
                Width = 16,
                Height = 16,
                FontFamily = new FontFamily("Segoe UI Light"),
                Background = ib,
                FontSize = 16.0,
                //Content = "×",
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                //Padding = new Thickness(-6,-6,-6,-6),
                Style = new Style(),
                Margin = new Thickness(3),
                ToolTip = "Закрыть вкладку",

            };

            hBt.Click += CloseTabButton_OnClick;
            headerContent.Children.Add(hBt);

            return headerContent;
        }

        /// <summary>
        /// Метод формирования TabContent на основе содержимого Customer
        /// </summary>
        /// <returns></returns>
        private FrameworkElement CreateTabContent()
        {

            Grid gridCustomerContent = new Grid();
            //gridCustomerContent.Background = Brushes.AliceBlue;
            ColumnDefinition coldef = new ColumnDefinition() { Width = new GridLength(400), };
            gridCustomerContent.ColumnDefinitions.Add(coldef);

            coldef = new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Star) };
            gridCustomerContent.ColumnDefinitions.Add(coldef);


            RowDefinition rowdef = new RowDefinition() { Height = new GridLength(50) };
            gridCustomerContent.RowDefinitions.Add(rowdef);

            rowdef = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
            gridCustomerContent.RowDefinitions.Add(rowdef);

            rowdef = new RowDefinition() { Height = new GridLength(100, GridUnitType.Star) };
            gridCustomerContent.RowDefinitions.Add(rowdef);

            DockPanel dockLable = new DockPanel() { /*ContextMenu = customerContextMenu*/ };
            TextBlock subNet = new TextBlock()
            {
                //Text = $"ЦВЗ №{NumberCVZ}. Подсеть {GetSubnetAddress(IPAddress.Parse(Lan_Ip), 256)}/24",
                Text = $"ЦВЗ №{NumberCVZ}",
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Navy,
                FontFamily = new FontFamily("Segoe UI Light"),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            dockLable.Children.Add(subNet);

            Image imageRefreshCustomer = new Image();
            imageRefreshCustomer.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/refresh.png"));
            Button refreshCustomer = new Button()
            {
                Height = 30,
                Width = 30,
                Padding = new Thickness(3),
                Margin = new Thickness(5),
                Content = imageRefreshCustomer,
                ToolTip = "Обновить список хостов",

            };

            refreshCustomer.SetValue(ControlsHelper.CornerRadiusProperty, new CornerRadius(20.0));
            refreshCustomer.Click += async (object sender, RoutedEventArgs args) => { await Owner.PingSelectedTT(); };
            dockLable.Children.Add(refreshCustomer);

            #region Кнопки

            StackPanel labelStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

            Button btnZabbix = new Button()
            {
                Content = "ZABBIX",
                Height = 30,
                Width = 80,
                Margin = new Thickness(5),
                Padding = new Thickness(-7),
                FontSize = 14,
                Foreground = Brushes.White,
                Background = Brushes.Crimson,
                Command = Commands.ZabbixButton
            };

            labelStackPanel.Children.Add(btnZabbix);

            Button btnMikrotikGMS = new Button()
            {
                Content = "MikroTik GMS",
                Height = 30,
                Width = 80,
                Margin = new Thickness(5),
                Padding = new Thickness(-7),
                FontSize = 10,
                Foreground = Brushes.White,
                Background = Brushes.Blue,
                Command = Commands.GmsButton,
            };
            labelStackPanel.Children.Add(btnMikrotikGMS);

            #region btnMikroTik

            Button btnMikrotik = new Button()
            {
                Height = 30,
                Width = 80,
                Margin = new Thickness(5),
                Padding = new Thickness(-7),
                Command = Commands.MikroTikwinboxButton
            };
            StackPanel spMikrotik = new StackPanel() { Orientation = Orientation.Horizontal };
            Image imMikrotik = new Image()
            {
                Width = 20,
                Margin = new Thickness(2),
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/winbox48.png")),
            };
            spMikrotik.Children.Add(imMikrotik);

            TextBlock tbMikrotik = new TextBlock() { Text = "MikroTik" };
            spMikrotik.Children.Add(tbMikrotik);
            btnMikrotik.Content = spMikrotik;

            labelStackPanel.Children.Add(btnMikrotik);

            #endregion

            #region Кнопка Jasper

            Button btnJasper = new Button()
            {
                Content = "Jasper",
                Height = 30,
                Width = 80,
                Margin = new Thickness(5),
                Padding = new Thickness(-7),
                FontSize = 14,
                Foreground = Brushes.White,
                Background = Brushes.Green,
                Command = Commands.JasperButton,
            };
            labelStackPanel.Children.Add(btnJasper);

            #endregion

            #endregion
            dockLable.Children.Add(labelStackPanel);
            //ImageBrush ib = new ImageBrush();
            //ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/zabbix.png"));
            //Button zabbixButton1 = new Button()
            //{
            //    Height = 40,
            //    Width = 120,
            //    Margin = new Thickness(3, 0, 3, 0),
            //    Background = ib
            //};
            //zabbixButton1.Click += ZabbixButton_Click;
            //dockLable.Children.Add(zabbixButton1);

            //Button zabbixButton = new Button()
            //{
            //    Height = 40,
            //    Width = 120,
            //    Margin = new Thickness(3, 0, 3, 0),
            //    Content = "ZABBIX",
            //    FontSize = 25.0,
            //    Padding = new Thickness(-10),
            //    VerticalAlignment = VerticalAlignment.Center,
            //    VerticalContentAlignment = VerticalAlignment.Center,
            //    Background = Brushes.Crimson,
            //    Foreground = Brushes.White

            //};
            //zabbixButton.Click += ZabbixButton_Click;
            //dockLable.Children.Add(zabbixButton);

            //ImageBrush ib = new ImageBrush();
            //ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/MikroTikGMS.png"));
            //Button gmsButton1 = new Button()
            //{
            //    Height = 40,
            //    Width = 120,
            //    Margin = new Thickness(3, 0, 3, 0),
            //    Background = ib
            //};
            //gmsButton1.Click += GmsButton_Click;
            //dockLable.Children.Add(gmsButton1);

            //Button gmsButton = new Button()
            //{
            //    Height = 40,
            //    Width = 120,
            //    Margin = new Thickness(3, 0, 3, 0),
            //    Content = "MikroTik GMS",
            //    FontSize = 14,
            //    FontStyle = FontStyles.Normal,
            //    FontWeight = FontWeights.Bold,
            //    //Padding = new Thickness(-15,-15,-15,-15),
            //    VerticalAlignment = VerticalAlignment.Center,
            //    VerticalContentAlignment = VerticalAlignment.Center,
            //    Background = Brushes.White
            //};
            //gmsButton.Click += GmsButton_Click;
            //dockLable.Children.Add(gmsButton);



            // Выполняется привязка textBlock времени региона к RegionClock, который постоянно меняется
            TextBlock region = new TextBlock()
            {
                DataContext = this,
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Navy,
                FontFamily = new FontFamily("Segoe UI Light"),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            region.SetBinding(TextBlock.TextProperty, "RegionClock");

            dockLable.Children.Add(region);

            gridCustomerContent.Children.Add(dockLable);

            Grid.SetRow(dockLable, 0);
            Grid.SetColumnSpan(dockLable, 2);

            //ListViewHostsInCustomer = new ListView();

            //CollectionView defaultView = (CollectionView)CollectionViewSource.GetDefaultView(this.IPArray);

            //PropertyGroupDescription item = new PropertyGroupDescription("Type");

            // defaultView.GroupDescriptions.Add(item);

            //listCustomer.BindingGroup = new BindingGroup();
            //listCustomer.BindingGroup.SetValue(ListView.BindingGroupProperty, new Binding("Type"));
            // listCustomer.SetValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, true);


            #region Список хостов

            GridView gridHostArray = new GridView();

            GridViewColumn gvc = new GridViewColumn()
            {
                Header = "Наименование",
                Width = 140,
                DisplayMemberBinding = new Binding("Type"),
            };
            gridHostArray.Columns.Add(gvc);

            gvc = new GridViewColumn()
            {
                Header = "IP адрес",
                DisplayMemberBinding = new Binding("IPAddress"),
                Width = 100
            };
            gridHostArray.Columns.Add(gvc);

            gvc = new GridViewColumn()
            {
                Header = "Имя ПК",
                DisplayMemberBinding = new Binding("NetbiosName"),
                Width = 100
            };
            gridHostArray.Columns.Add(gvc);

            gvc = new GridViewColumn()
            {
                Header = "Пинг",
                DisplayMemberBinding = new Binding("Latency"),
                Width = 50
            };
            gridHostArray.Columns.Add(gvc);

            #region Временно закоментировано
            //UserControlListViewHosts tempList = new UserControlListViewHosts();
            //tempList.DataContext = this;
            //ListViewHostsInCustomer = (new UserControlListViewHosts() { DataContext = this }).ListViewIP;

            ////ListViewHostsInCustomer.View = gridHostArray;
            ////ListViewHostsInCustomer.SelectionMode = SelectionMode.Single;
            //ListViewHostsInCustomer.MouseRightButtonUp += ListViewItem_MouseRightButtonUp;
            //ListViewHostsInCustomer.PreviewMouseLeftButtonUp += ListViewHostsInCustomer_PreviewMouseLeftButtonUp;
            //ListViewHostsInCustomer.MouseLeftButtonUp += ListViewHostsInCustomer_MouseLeftButtonUp;
            //ListViewHostsInCustomer.MouseDoubleClick += ListViewHostsInCustomer_MouseDoubleClick;
            //ListViewHostsInCustomer.MouseLeave += ListViewItem_MouseLeave;




            //gridCustomerContent.Children.Add(ListViewHostsInCustomer);
            //Grid.SetRow(ListViewHostsInCustomer, 1);
            //Grid.SetRowSpan(ListViewHostsInCustomer, 2);
            #endregion


            //gridCustomerContent.Children.Add(tempList);
            //Grid.SetRow(tempList, 1);
            //Grid.SetRowSpan(tempList, 2);

            #endregion



            // Формирование Expander
            #region Формирование Expander
            ScrollViewer scr = new ScrollViewer();
            Grid expanderGrid = new Grid();


            StackPanel expanderStackPanel = new StackPanel();
            expanderGrid.Children.Add(expanderStackPanel);

            //RowDefinition exprowdef = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            //expanderGrid.RowDefinitions.Add(exprowdef);

            //exprowdef = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            //expanderGrid.RowDefinitions.Add(exprowdef);

            //exprowdef = new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) };
            //expanderGrid.RowDefinitions.Add(exprowdef);



            ListView newListView = new ListView()
            {
                Margin = new Thickness(5),
                SelectionMode = SelectionMode.Single,
                ItemsSource = Info,
                ToolTip = "Нажатие левой кнопкой копирует в буфер \"Значение\"\nНажатие правой кнопкой копирует в буфер \"Свойство: Значение\"",

            };

            GridView newGridView = new GridView();
            //newGridView.SetValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, true);
            GridViewColumn newCol = new GridViewColumn()
            {
                Header = "Свойство",
                Width = 130,
                DisplayMemberBinding = new Binding("Description"),
            };
            newGridView.Columns.Add(newCol);

            newCol = new GridViewColumn()
            {
                Header = "Значение",
                Width = 500,
                DisplayMemberBinding = new Binding("Value"),
            };
            newGridView.Columns.Add(newCol);
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(newGridView, true);
            newListView.SetValue(VirtualizingPanel.IsVirtualizingWhenGroupingProperty, true);
            newListView.View = newGridView;

            //newListView.MouseLeftButtonUp += ExpanderInfo_MouseLeftButtonUp;
            //newListView.MouseRightButtonUp += ExpanderInfo_MouseRightButtonUp;

            Expander newExpander = new Expander()
            {
                Header = "Информация о потребителе",
                Margin = new Thickness(5, 0, 0, 0),
                IsExpanded = true,
                Content = newListView,

            };
            //newExpander.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, System.Windows.Controls.ScrollBarVisibility.Disabled);
            expanderStackPanel.Children.Add(newExpander);
            //Grid.SetRow(newExpander, 0);

            newGridView = new GridView();
            newCol = new GridViewColumn()
            {
                Header = "Свойство",
                Width = 130,
                DisplayMemberBinding = new Binding("Description"),

            };
            newGridView.Columns.Add(newCol);

            newCol = new GridViewColumn()
            {
                Header = "Значение",
                Width = 500,
                DisplayMemberBinding = new Binding("Value"),
            };
            newGridView.Columns.Add(newCol);

            newListView = new ListView()
            {
                Margin = new Thickness(5),
                SelectionMode = SelectionMode.Single,
                ItemsSource = Provider.Info,
                View = newGridView,
                ToolTip = "Нажатие левой кнопкой копирует в буфер \"Значение\"\nНажатие правой кнопкой копирует в буфер \"Свойство: Значение\""
            };

            //newListView.MouseLeftButtonUp += ExpanderInfo_MouseLeftButtonUp;
            //newListView.MouseRightButtonUp += ExpanderInfo_MouseRightButtonUp;

            newExpander = new Expander()
            {
                Header = "Информация о провайдере",
                Margin = new Thickness(5, 0, 0, 0),
                IsExpanded = true,
                Content = newListView
            };

            Grid infoProvGridExpander = new Grid();
            infoProvGridExpander.Children.Add(newExpander);
            expanderStackPanel.Children.Add(infoProvGridExpander);
            //Grid.SetRow(newExpander, 1);
            gridCustomerContent.Children.Add(scr);

            scr.Content = expanderGrid;
            Grid.SetColumn(scr, 1);
            Grid.SetRow(scr, 1);
            Grid.SetRowSpan(scr, 2);

            #endregion



            return gridCustomerContent;
        }

        





       

        

        

        

        

        

        #endregion


        #region Вынесено в NewMainWindow.xaml.Tabs.cs
        /// <summary>
        /// Метод обработки события нажатия кнопки закрытия вкладки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTabButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Customer curCustomer = ((sender as Button).Parent as DockPanel).Parent as Customer;
            //((((sender as Button).Parent as DockPanel).Parent as TabItem).Parent as TabControl).Items.Remove(curCustomer);
            Owner.OpenedTTRemove(this);
            //this.SetBinding(ListView.SelectedItemProperty, "");
            // this.ListViewHostsInCustomer.ItemsSource = null;
            //MainWindow.Bindings.isipselected = false;
        }

        /// <summary>
        /// Метод формирования нового экземпляра Customer с Header и Content
        /// </summary>
        /// <returns></returns>
        public void CreateCustomerWithContentAndHeader()
        {
            //Content = CreateTabContent();
            //Header = CreateTabHead();

            //Background = Brushes.AliceBlue;
        }

        /// <summary>
        /// Метод формирования контекстного меню для Customer
        /// </summary>
        private void CustomerContextMenuCreate_old()
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            MenuItem menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Код ЦВЗ";
            menuItem.Tag = 1;
            //menuItem.Click += CustomerContextMenu_Click;
            //customerContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Адрес ЦВЗ";
            menuItem.Tag = 2;
            //menuItem.Click += CustomerContextMenu_Click;
            //customerContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Полное имя ЦВЗ";
            menuItem.Tag = 3;
            //menuItem.Click += CustomerContextMenu_Click;
            //customerContextMenu.Items.Add(menuItem);

        }

        /// <summary>
        /// Метод формирования контекстного меню для ListMenuItem
        /// </summary>
        private void ContextMenuCreate()
        {

            Image image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            MenuItem menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать IP адрес";
            menuItem.Tag = 1;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Имя ПК";
            menuItem.Tag = 2;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Имя ПК и IP адрес";
            menuItem.Tag = 3;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать сетевые реквизиты";
            menuItem.Tag = 4;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            Separator separator = new Separator();
            //listViewItemContextMenu.Items.Add(separator);

            image = new Image();
            image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/copy_6524.png"));  // Иконка копирования
            menuItem = new MenuItem();
            menuItem.Icon = image;
            menuItem.Header = "Копировать Название ЦВЗ";
            menuItem.Tag = 5;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            Image image2 = new Image();
            image2.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/imagelistcontrol_683.png"));
            menuItem.Icon = image2;
            menuItem.Header = "Копировать скриншот списка";
            menuItem.Tag = 6;
            //menuItem.Click += MenuItem_Click;
            //listViewItemContextMenu.Items.Add(menuItem);

            separator = new Separator();
            //listViewItemContextMenu.Items.Add(separator);

            #region Формирование подменю со списком типов хостов IPType

            MenuItem menuItemHostType = new MenuItem();
            menuItemHostType.Header = "Задать тип хоста";
            menuItemHostType.Tag = 7;

            MenuItem newElement;
            foreach (var item in Enum.GetValues(typeof(IPType)).Cast<IPType>())
            {
                newElement = new MenuItem();
                //newElement.SetValue(HeaderProperty, item);
                //newElement.SetValue(ContentProperty, item);
                //newElement.Click += (object sender, RoutedEventArgs e) => { Owner.SelectedIP.Type = GetIPType((sender as MenuItem).Header.ToString()); };
                menuItemHostType.Items.Add(newElement);
            }

            #endregion

            //listViewItemContextMenu.Items.Add(menuItemHostType);

        }
        #endregion

    }
}
