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
        
        private void ButtonsPanel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Метод открытия Zabbix в браузере
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZabbixButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //MessageBox.Show(Environment.Version.ToString());
                if (Environment.UserDomainName.ToLower().Contains("dengisrazy"))
                    NewMainWindow.ExecuteProgram($"http://zabbix.dengisrazy.ru/search.php?sid=402de2905824eba9&form_refresh=3&search={SelectedTT.Lan_Ip}");
                else
                    NewMainWindow.ExecuteProgram($"http://zabbix.vpn.dengisrazy.ru/search.php?sid=402de2905824eba9&form_refresh=3&search={SelectedTT.Lan_Ip}");
            }
            catch (Exception)
            {
                Bindings.StatusBarText = "Необходимо выбрать ЦВЗ";
            }

        }


        private void GmsButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                NewMainWindow.ExecuteProgram($"http://gms.dengisrazy.ru/search.php?id={SelectedTT.NumberCVZ}");
            }
            catch (Exception)
            {
                Bindings.StatusBarText = "Необходимо выбрать ЦВЗ";
            }
        }
    }
}

