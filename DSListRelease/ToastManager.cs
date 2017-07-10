using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DSList
{
    public static class ToastManager
    {
        public static ToasterAnimation animation = ToasterAnimation.FadeIn;
        public static double margin = 20.0;
        public static ToasterPosition position = ToasterPosition.PrimaryScreenBottomRight;
        private static BackgroundWorker QueueWorker = new BackgroundWorker();
        private static Queue<Tuple<string, string, ToastType>> ToastQueue = new Queue<Tuple<string, string, ToastType>>();
        public static int ToastTime = 10;

        static ToastManager()
        {
            QueueWorker.DoWork += new DoWorkEventHandler(ToastManager.QueueWorker_DoWork);
        }

        public static void Add(string Message, string Title, ToastType ToastMessageType)
        {
            try
            {
                ToastQueue.Enqueue(new Tuple<string, string, ToastType>(Message, Title, ToastMessageType));
                if (!QueueWorker.IsBusy)
                {
                    QueueWorker.RunWorkerAsync();
                }
            }
            catch
            {
            }
        }

        private static void QueueWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (ToastQueue.Count > 0)
                {
                    try
                    {
                        Tuple<string, string, ToastType> ToastTuple = ToastQueue.Dequeue();
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate {
                            try
                            {
                                switch (ToastTuple.Item3)
                                {
                                    case ToastType.Success:
                                        SuccessToaster.Toast(ToastTuple.Item1, ToastTuple.Item2, position, animation, margin);
                                        return;

                                    case ToastType.Warning:
                                        WarningToaster.Toast(ToastTuple.Item1, ToastTuple.Item2, position, animation, margin);
                                        return;

                                    case ToastType.Error:
                                        ErrorToaster.Toast(ToastTuple.Item1, ToastTuple.Item2, position, animation, margin);
                                        return;
                                }
                            }
                            catch
                            {
                            }
                        });
                        Thread.Sleep(0x2710);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
    }
}
