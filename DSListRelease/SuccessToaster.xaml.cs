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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для SuccessToaster1.xaml
    /// </summary>
    public partial class SuccessToaster : Window, IComponentConnector
    {

        private SuccessToaster(string message, string title, ToasterPosition position, ToasterAnimation animation, double margin)
        {
            this.InitializeComponent();
            System.Windows.Documents.Run run = (System.Windows.Documents.Run)this.SuccessToasterInstance.FindName("MessageString");
            if (run != null)
            {
                run.Text = message ?? string.Empty;
            }
            System.Windows.Documents.Run run2 = (System.Windows.Documents.Run)this.SuccessToasterInstance.FindName("TitleString");
            if (run2 != null)
            {
                run2.Text = title ?? string.Empty;
            }
            Storyboard storyboard = ToastSupport.GetAnimation(animation, ref this.SuccessToasterInstance);
            storyboard.Completed += (sender, args) => base.Close();
            storyboard.Begin(this.SuccessToasterInstance);
            base.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)delegate {
                Dictionary<string, double> dictionary = ToastSupport.GetTopandLeft(position, this, margin);
                this.Top = dictionary["Top"];
                this.Left = dictionary["Left"];
            });
        }


        public SuccessToaster()
        {
            InitializeComponent();
        }

        private void SuccessToasterInstance_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            base.Close();
        }

        public static void Toast(string message = "Сообщение", string title = "Выполнено", ToasterPosition position = ToasterPosition.PrimaryScreenBottomRight, ToasterAnimation animation = ToasterAnimation.FadeIn, double margin = 20.0)
        {
            new SuccessToaster(message, title, position, animation, margin).Show();
        }
    }
}
