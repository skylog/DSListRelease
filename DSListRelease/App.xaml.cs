using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Shell;
using System.Windows;
using System.Windows.Markup;
//using Microsoft.Shell;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
//using System.Windows.Forms;
using System.Windows.Threading;
using TaskDialogInterop;


namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App:Application/*, ISingleInstanceApp*/
    {
        private ManualResetEvent ResetSplashCreated;
        public static ISplashScreen splashScreen;
        private Thread SplashThread;

        //[STAThread]
        //public static void Main()
        //{
        //    if (SingleInstance<App>.InitializeAsFirstInstance("dslist"))
        //    {
        //        App app = new App();
        //        app.InitializeComponent();
        //        app.Run();
        //        SingleInstance<App>.Cleanup();
        //    }
        //}

        public App()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.ResetSplashCreated = new ManualResetEvent(false);
            this.SplashThread = new Thread(new ThreadStart(this.ShowSplash));
            this.SplashThread.SetApartmentState(ApartmentState.STA);
            this.SplashThread.IsBackground = true;
            this.SplashThread.Name = "Splash Screen";
            this.SplashThread.Start();
            this.ResetSplashCreated.WaitOne();
            Access access = new Access();
            base.OnStartup(e);
        }

        private void ShowSplash()
        {
            AnimatedSplashScreenWindow window = new AnimatedSplashScreenWindow();
            splashScreen = window;
            window.Show();
            this.ResetSplashCreated.Set();
            Dispatcher.Run();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (base.MainWindow != null)
            {
                base.MainWindow.Visibility = Visibility.Visible;
                base.MainWindow.ShowInTaskbar = true;
                if (base.MainWindow.WindowState == WindowState.Minimized)
                {
                    base.MainWindow.WindowState = WindowState.Normal;
                }
                base.MainWindow.Activate();
                return ((NewMainWindow)base.MainWindow).ProcessCommandLineArgs(args);
            }
            return false;
        }
    }
}
