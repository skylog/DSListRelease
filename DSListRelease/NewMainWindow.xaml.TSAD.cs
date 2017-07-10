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
using System.Windows.Controls.Ribbon;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Windows.Threading;
using System.Reflection;

namespace DSList
{
    public partial class NewMainWindow
    {
        ObservableCollection<IPAndName> listPcIPAndPcName;

        ObservableCollection<ADUser> listADUsers;
        ObservableCollection<ADUser> searchListADUser;

        ObservableCollection<TSUser> listTSUser;
        ObservableCollection<TSUser> searchListTSUser;

        public IHtmlDocument DocumentADUsersHTML { get; set; }
        BackgroundWorker progressBarWorker;

        #region Поиск в списке AD
        /// <summary>
        /// Поле таймер инициализации поиска при изменении строки поиска AD
        /// </summary>
        private DispatcherTimer SearchADTimer = new DispatcherTimer();

        /// <summary>
        /// Метод инициализации таймера поиска на основе введённых данных в поле searchTextBoxAD
        /// </summary>
        private void InitSearchADTimer()
        {
            this.SearchADTimer.Stop();
            this.SearchADTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            this.SearchADTimer.Start();
        }

        /// <summary>
        /// Метод обработки отработки единичного тика таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchADTimer_Tick(object sender, EventArgs e)
        {
            FillSearchListBoxAD();
        }

        /// <summary>
        /// Метод обработки изменения поля searchTextBoxAD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBoxAD_TextChanged(object sender, TextChangedEventArgs e)
        {
            InitSearchADTimer();
        }

        /// <summary>
        /// Метод заполнения searchListBoxAD на основе строки поиска searchTextBoxAD
        /// </summary>
        private void FillSearchListBoxAD()
        {
            SearchADTimer.Stop();
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!string.IsNullOrEmpty(searchTextBoxAD.Text))
                    {
                        string lowSearch = searchTextBoxAD.Text.ToLower();
                        var resultSearch = from user in listADUsers
                                           where user.AccountName.ToLower().Contains(lowSearch) || user.Family.ToLower().Contains(lowSearch) || user.NameUser.ToLower().Contains(lowSearch) || user.MiddleName.ToLower().Contains(lowSearch) || user.Departament.ToLower().Contains(lowSearch) || user.Position.ToLower().Contains(lowSearch)

                                           select user;
                        searchListBoxAD.ItemsSource = resultSearch;
                    }
                    else
                        searchListBoxAD.ItemsSource = null;

                    Bindings.StatusBarText = searchTextBoxAD.Text;
                }));
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }
        #endregion


        private void btnLoadListHostsInFile_Click(object sender, RoutedEventArgs e)
        {
            LoadListHostsInFile();
        }

        private void btnLoadTSUsers_Click(object sender, RoutedEventArgs e)
        {
            LoadTSUsers();
        }

        private void btnFillIPInTS_Click(object sender, RoutedEventArgs e)
        {
            FillIPInTS();
        }

        private void btnCreateFileUsersAD_click(object sender, RoutedEventArgs e)
        {
            CreateFileUsersAD();
        }

        private void btnLoadListHosts_Click(object sender, RoutedEventArgs e)
        {
            CreateListIPNameFromFile();
        }

        private void btnLoadFileInDataGrid_click(object sender, RoutedEventArgs e)
        {
            LoadFileWithADUsersInDataGrid();
        }

        private void btnRefreshPhoneNumbersInDataGrid_usersAD_Click(object sender, RoutedEventArgs e)
        {
            RefreshMobile();
            //Dispatcher.Invoke(() => 
            //{
            //    new Thread(new ThreadStart(() =>
            //    {

            //    })).Start();
            //});
        }


        private void searchListBoxAD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((e.Source as ListBox).SelectedIndex != -1)
                {
                    var selLBItem = (e.Source as ListBox).SelectedItem as ADUser;
                    dataGridAD.SelectedItem = selLBItem;
                    dataGridAD.ScrollIntoView(dataGridAD.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }

        }

        private void searchTextBoxTS_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!string.IsNullOrEmpty((e.Source as TextBox).Text))
                    {
                        string lowSearch = (e.Source as TextBox).Text.ToLower();
                        var resultSearch = from user in listTSUser
                                           where user.UserName.ToLower().Contains(lowSearch) || user.PCName.ToLower().Contains(lowSearch) || user.ClientIPAddress.ToLower().Contains(lowSearch) || user.RusFamily.ToLower().Contains(lowSearch) || user.RusMiddleName.ToLower().Contains(lowSearch) || user.RusName.ToLower().Contains(lowSearch)

                                           select user;
                        searchListBoxTS.ItemsSource = resultSearch;
                    }
                    else
                        searchListBoxTS.ItemsSource = null;

                    Bindings.StatusBarText = (e.Source as TextBox).Text;
                }));
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }

        }

        private void searchListBoxTS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((e.Source as ListBox).SelectedIndex != -1)
                {
                    var selLBItem = (e.Source as ListBox).SelectedItem as TSUser;
                    dataGridTS.SelectedItem = selLBItem;
                    dataGridTS.ScrollIntoView(dataGridTS.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }

        }


        private void btnSettingMainWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        public void RefreshMobile()
        {
            try
            {
                progressBarStatus.Maximum = listADUsers.Count;
                progressBarStatus.Value = 0;
                progressBarStatus.Visibility = Visibility.Visible;
                Bindings.StatusBarText = "Выполняется приведение телефонов в таблице к общему виду";
                progressBarWorker.DoWork += new DoWorkEventHandler(PhoneChangeWorker_DoWork);
                progressBarWorker.RunWorkerCompleted += PhoneChangeWorker_RunWorkerCompleted;
                progressBarWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Bindings.StatusBarText = ex.Message;
            }
        }

        private void PhoneChangeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Bindings.StatusBarText = "Выполнено приведение телефонов в таблице к общему виду";
            dataGridAD.Items.Refresh();
            progressBarWorker.RunWorkerCompleted -= PhoneChangeWorker_RunWorkerCompleted;
            progressBarWorker.DoWork -= new DoWorkEventHandler(PhoneChangeWorker_DoWork);
            progressBarStatus.Visibility = Visibility.Collapsed;
        }

        #region Реализация ProgressBar
        void PhoneChangeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateProgressBarDelegate updProgress = new UpdateProgressBarDelegate(progressBarStatus.SetValue);
            double value = 0;

            Dispatcher.Invoke(() =>
            {
                foreach (var row in listADUsers)
                {

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        row.MobilePhone = ADUser.FormatPhone(row.MobilePhone);
                    })/*, System.Windows.Threading.DispatcherPriority.Normal*/);
                    Dispatcher.Invoke(updProgress, System.Windows.Threading.DispatcherPriority.Render, new object[] { MetroProgressBar.ValueProperty, ++value });
                    //row.MobilePhone = ADUser.FormatPhone(row.MobilePhone);


                }


            });


        }

        #endregion

        
        /// <summary>
        /// Метод обработки логики при нажатии "Обновить" в "Пользователи TS"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshTS_Click(object sender, RoutedEventArgs e)
        {
            LoadListHostsInFile();  //Сформировать файл с хостами
            LoadTSUsers();          //Сформировать список пользователей TS
            FillIPInTS();           //Заполнить список пользователей TS найденными IP адресами
            CreateFileUsersAD();    //Сформировать список пользователей AD в AD-enabled-users.html
            LoadFileWithADUsersInDataGrid();    //Заполнить dataGrid из AD-enabled-users.html
            RefreshMobile();        //Привести телефоны к стандартному виду
            FillRusFIO();           //Заполнить список пользователей TS ФИО из dataGrid AD
        }

        /// <summary>
        /// Метод обработки логики при нажатии "Обновить" в "Пользователи AD"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshAD_Click(object sender, RoutedEventArgs e)
        {
            CreateFileUsersAD();    //Сформировать список пользователей AD в AD-enabled-users.html
            LoadFileWithADUsersInDataGrid();    //Заполнить dataGrid из AD-enabled-users.html
            RefreshMobile();        //Привести телефоны к стандартному виду
            FillRusFIO();           //Заполнить список пользователей TS ФИО из dataGrid AD
        }

        /// <summary>
        /// Метод открытия ссылок на ресурсы в браузере по умолчанию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferenceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Process process = new Process();
                //process.StartInfo.FileName = (sender as RibbonMenuItem).Tag.ToString();
                ////process.StartInfo.FileName = "cmd.exe|/k whoami";
                //process.Start();
                StartProgramSupportTools((sender as Button).Tag.ToString(), "", true);

            }
            catch (Exception ex)
            {
                Log("Ошибка открытия ресурса в браузере по умолчанию" + ex.Message, true, true, ex.StackTrace);
            }

        }
    }
}
