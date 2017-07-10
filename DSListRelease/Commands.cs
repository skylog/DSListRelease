using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DSList
{
    public static class Commands
    {
        #region ControlsAndFilesTab

        public static RoutedCommand ClickTest = new RoutedCommand();
        public static RoutedCommand ClickTest1 = new RoutedCommand();
        public static RoutedCommand TestPlink = new RoutedCommand();
        public static RoutedCommand RunVNC = new RoutedCommand();
        public static RoutedCommand RunVNCPassFile = new RoutedCommand();
        public static RoutedCommand PuttySSH = new RoutedCommand();
        public static RoutedCommand PuttyTelnet = new RoutedCommand();
        public static RoutedCommand WinboxMikrotik = new RoutedCommand();
        public static RoutedCommand BrowserHostRun = new RoutedCommand();
        public static RoutedCommand FileZilla = new RoutedCommand();
        public static RoutedCommand WinSCP = new RoutedCommand();
        public static RoutedCommand WineComplect = new RoutedCommand();
        public static RoutedCommand ZimbraComplect = new RoutedCommand();
        public static RoutedCommand RunTightVNC = new RoutedCommand();

        #endregion


        #region DiagnosticsAndSettingsTab

        public static RoutedCommand WineComplectScript = new RoutedCommand();
        public static RoutedCommand ZimbraComplectScript = new RoutedCommand();
        public static RoutedCommand LockScript = new RoutedCommand();
        public static RoutedCommand UnLockScript = new RoutedCommand();
        #endregion

        #region NetworkTab

        public static RoutedCommand PingControl = new RoutedCommand();
        public static RoutedCommand PathPing = new RoutedCommand();
        public static RoutedCommand Tracert = new RoutedCommand();
        public static RoutedCommand PingExtended = new RoutedCommand();
        public static RoutedCommand MacInfo = new RoutedCommand();
        public static RoutedCommand WinMTR = new RoutedCommand();
        public static RoutedCommand ARPWin = new RoutedCommand();
        #endregion

        #region Buttons panel

        public static RoutedCommand ZabbixButton = new RoutedCommand();
        public static RoutedCommand GmsButton = new RoutedCommand();
        public static RoutedCommand MikroTikwinboxButton = new RoutedCommand();
        public static RoutedCommand JasperButton = new RoutedCommand();

        #endregion

        #region Scripts

        public static RoutedCommand TESTScript = new RoutedCommand();
        public static RoutedCommand HostPCInfo = new RoutedCommand();
        public static RoutedCommand RestartHostPC = new RoutedCommand();
        public static RoutedCommand ClearMotionHostPC = new RoutedCommand();
        public static RoutedCommand ZabbixAgentUpdate = new RoutedCommand();
        public static RoutedCommand SmartCtl = new RoutedCommand();
        public static RoutedCommand DS_Benchmark = new RoutedCommand();
        public static RoutedCommand AddAlias1СInBash = new RoutedCommand();
        #region 1C

        public static RoutedCommand Ink1CWine = new RoutedCommand();
        public static RoutedCommand Calendar1CDll = new RoutedCommand();
        public static RoutedCommand Calendar1CDlldownload = new RoutedCommand();
        public static RoutedCommand DоntOpenPDF1С = new RoutedCommand();

        #endregion

        #region Linphone
        public static RoutedCommand LinphoneInstInk = new RoutedCommand();
        #endregion

        #region Tab ARM

        public static RoutedCommand DeleteProfileGoogleChrome = new RoutedCommand();
        public static RoutedCommand DeleteProfileChromium = new RoutedCommand();
        public static RoutedCommand Kill1CWine = new RoutedCommand();
        public static RoutedCommand KillZimbraProcess = new RoutedCommand();
        public static RoutedCommand KillChromeFirefoxProcess = new RoutedCommand();
        public static RoutedCommand RefreshProfileZimbra = new RoutedCommand();
        #endregion

        public static RoutedCommand UsbPowerReset = new RoutedCommand();
        public static RoutedCommand MikrotikIPSecTunelsPrint = new RoutedCommand();
        public static RoutedCommand MikrotikIPSec = new RoutedCommand();
        public static RoutedCommand MikrotikIPSecTunelsFlush = new RoutedCommand();

        #region Tab Маршрутизатор



        #endregion

        #endregion

    }


}
