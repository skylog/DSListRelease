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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Windows.Threading;
using TaskDialogInterop;

namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для AnimatedSplashScreenWindow.xaml
    /// </summary>
    public partial class AnimatedSplashScreenWindow : Window, ISplashScreen, IComponentConnector
    {

        public AnimatedSplashScreenWindow()
        {
            this.InitializeComponent();
            this.Version.Text = "Версия " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void move(object sender, MouseEventArgs e)
        {
            DragMove();
        }

        public void AddMessage(string message)
        {
            base.Dispatcher.Invoke(() => this.Status.Text = message);
        }


        public void LoadComplete()
        {
            base.Dispatcher.InvokeShutdown();
        }


        public void ShowError(string errortext, string errortitle = "", bool Close = true)
        {
            base.Dispatcher.Invoke(delegate {
                TaskDialogOptions options = new TaskDialogOptions();
                if (this.IsVisible)
                {
                    options.Owner = this;
                }
                else
                {
                    options.Owner = Application.Current.MainWindow;
                }
                options.Title = "DSList " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                options.Content = errortext;
                if (!string.IsNullOrWhiteSpace(errortitle))
                {
                    options.MainInstruction = errortitle;
                }
                options.DefaultButtonIndex = 0;
                options.MainIcon = VistaTaskDialogIcon.Error;
                if (Close)
                {
                    options.CustomButtons = new string[] { "Закрыть", "Справка" };
                }
                else
                {
                    options.CustomButtons = new string[] { "ОК", "Справка" };
                }
                options.AllowDialogCancellation = true;
                options.Callback = new TaskDialogCallback(this.taskDialog_Callback1);
                int? customButtonResult = TaskDialog.Show(options).CustomButtonResult;
                //int num = 1;
                //if ((customButtonResult.GetValueOrDefault() == num) ? ((Delegate)customButtonResult.HasValue) : ((Delegate)false))
                //{
                //    Process.Start(@"\\FS-EKB-a0009\Группа_ИТ_сопровождения_ТТ\1_База_знаний\Справка ttlist\Открыть записную книжку.onetoc2");
                //}
            }, DispatcherPriority.Normal);
        }


        private bool taskDialog_Callback1(IActiveTaskDialog dialog, VistaTaskDialogNotificationArgs args, object callbackData)
        {
            if (args.Notification == VistaTaskDialogNotification.HyperlinkClicked)
            {
                try
                {
                    Process.Start(args.Hyperlink);
                }
                catch
                {
                }
            }
            return false;
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((App)Application.Current).Shutdown();
            Environment.Exit(1);
            //Environment.Exit(0);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                base.DragMove();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Splash_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show("Test");
            Close();
        }

        private void Splash_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SplashClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ((App)Application.Current).Shutdown();
            Environment.Exit(1);
            Close();
        }
    }
}
