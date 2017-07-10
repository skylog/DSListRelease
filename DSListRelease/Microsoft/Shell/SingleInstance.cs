namespace Microsoft.Shell
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using System.Xml.Serialization;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Text;

    public static class SingleInstance<TApplication> 
        where TApplication : Application, ISingleInstanceApp
    {
        #region Private Classes

        /// <summary>
        /// Remoting service class which is exposed by the server i.e the first instance and called by the second instance
        /// to pass on the command line arguments to the first instance and cause it to activate itself.
        /// </summary>
        private class IPCRemoteService : MarshalByRefObject
        {
            /// <summary>
            /// Activates the first instance of the application.
            /// </summary>
            /// <param name="args">List of arguments to pass to the first instance.</param>
            public void InvokeFirstInstance(IList<string> args)
            {
                if (Application.Current != null)
                {
                    // Do an asynchronous call to ActivateFirstInstance function
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal, new DispatcherOperationCallback(SingleInstance<TApplication>.ActivateFirstInstanceCallback), args);
                }
            }

            /// <summary>
            /// Remoting Object's ease expires after every 5 minutes by default. We need to override the InitializeLifetimeService class
            /// to ensure that lease never expires.
            /// </summary>
            /// <returns>Always null.</returns>
            public override object InitializeLifetimeService()
            {
                return null;
            }

            public IPCRemoteService()
            {

            }
        }

        #endregion

        private static IpcServerChannel channel;
        private const string ChannelNameSuffix = "SingeInstanceIPCChannel";
        private static IList<string> commandLineArgs;
        private const string Delimiter = ":";
        private const string IpcProtocol = "ipc://";
        private const string RemoteServiceName = "SingleInstanceApplicationService";
        private static Mutex singleInstanceMutex;

        private static void ActivateFirstInstance(IList<string> args)
        {
            if (Application.Current != null)
            {
                ((TApplication)Application.Current).SignalExternalCommandLineArgs(args);
            }
        }

        private static object ActivateFirstInstanceCallback(object arg)
        {
            IList<string> args = arg as IList<string>;
            SingleInstance<TApplication>.ActivateFirstInstance(args);
            return null;
        }

        public static void Cleanup()
        {
            if (SingleInstance<TApplication>.singleInstanceMutex != null)
            {
                SingleInstance<TApplication>.singleInstanceMutex.Close();
                SingleInstance<TApplication>.singleInstanceMutex = null;
            }
            if (SingleInstance<TApplication>.channel != null)
            {
                ChannelServices.UnregisterChannel(SingleInstance<TApplication>.channel);
                SingleInstance<TApplication>.channel = null;
            }
        }

        private static void CreateRemoteService(string channelName)
        {
            BinaryServerFormatterSinkProvider sinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = TypeFilterLevel.Full
            };
            IDictionary properties = new Dictionary<string, string>();
            properties["name"] = channelName;
            properties["portName"] = channelName;
            properties["exclusiveAddressUse"] = "false";
            SingleInstance<TApplication>.channel = new IpcServerChannel(properties, sinkProvider);
            ChannelServices.RegisterChannel(SingleInstance<TApplication>.channel, true);
            IPCRemoteService service = new IPCRemoteService();
            RemotingServices.Marshal(service, "SingleInstanceApplicationService");
        }



        private static IList<string> GetCommandLineArgs(string uniqueApplicationName)
        {
            string[] collection = null;
            if (AppDomain.CurrentDomain.ActivationContext == null)
            {
                collection = Environment.GetCommandLineArgs();
            }
            else
            {
                string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), uniqueApplicationName), "cmdline.txt");
                if (File.Exists(path))
                {
                    try
                    {
                        using (TextReader reader = new StreamReader(path, Encoding.Unicode))
                        {
                            collection = Microsoft.Shell.NativeMethods.CommandLineToArgvW(reader.ReadToEnd());
                        }
                        File.Delete(path);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            if (collection == null)
            {
                collection = new string[0];
            }
            return new List<string>(collection);
        }

        public static bool InitializeAsFirstInstance(string uniqueName)
        {
            bool flag;
            SingleInstance<TApplication>.commandLineArgs = SingleInstance<TApplication>.GetCommandLineArgs(uniqueName);
            string name = uniqueName + Environment.UserName;
            string channelName = name + ":" + "SingeInstanceIPCChannel";
            SingleInstance<TApplication>.singleInstanceMutex = new Mutex(true, name, out flag);
            if (flag)
            {
                SingleInstance<TApplication>.CreateRemoteService(channelName);
                return flag;
            }
            SingleInstance<TApplication>.SignalFirstInstance(channelName, SingleInstance<TApplication>.commandLineArgs);
            return flag;
        }

        private static void SignalFirstInstance(string channelName, IList<string> args)
        {
            IpcClientChannel chnl = new IpcClientChannel();
            ChannelServices.RegisterChannel(chnl, true);
            string url = "ipc://" + channelName + "/SingleInstanceApplicationService";
            IPCRemoteService service = (IPCRemoteService)RemotingServices.Connect(typeof(IPCRemoteService), url);
            if (service != null)
            {
                service.InvokeFirstInstance(args);
            }
        }

        public static IList<string> CommandLineArgs
        {
            get
            {
                return SingleInstance<TApplication>.commandLineArgs;
            }
        }

       
    }

    
}

