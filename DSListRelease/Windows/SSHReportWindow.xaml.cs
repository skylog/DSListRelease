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
using System.Windows.Shapes;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;
using DSList;


namespace DSList.Windows
{
    /// <summary>
    /// Логика взаимодействия для SSHReportWindow.xaml
    /// </summary>
    //public partial class SSHReportWindow : Window
    //{
    //    public SSHReportWindow()
    //    {
    //        InitializeComponent();
    //    }
    //}
    public partial class SSHReportWindow : Window, IComponentConnector
    {
        public NewMainWindow ParentWindow = null;

        public SSHReportWindow(NewMainWindow Parent)
        {
            this.ParentWindow = Parent;
            this.InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (this.ParentWindow != null)
            {
                this.ParentWindow.CopyToClipboard(this.Text.Text, true, "Результат выполнения команды скопирован в буфер обмена.");
            }
        }

        
        
    }
}
