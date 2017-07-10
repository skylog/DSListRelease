using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSList
{
    /// <summary>
    /// Класс записи лога
    /// </summary>
    public class LogWriter
    {
        private static LogWriter instance;
        private static DateTime LastFlushed = DateTime.Now;
        public static string logDir = (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DSList\Logs");
        private static string logFile = Path.GetFileName(Path.ChangeExtension(Assembly.GetEntryAssembly().GetName().Name, ".log"));
        private static Queue<Log> logQueue;
        private static int maxLogAge = 0x15180;
        private static int queueSize = 1;
        
        private LogWriter()
        {
        }

        private bool DoPeriodicFlush()
        {
            TimeSpan span = (TimeSpan)(DateTime.Now - LastFlushed);
            if (span.TotalSeconds >= maxLogAge)
            {
                LastFlushed = DateTime.Now;
                return true;
            }
            return false;
        }

        ~LogWriter()
        {
            this.FlushLog();
        }

        private void FlushLog()
        {
            while (logQueue.Count > 0)
            {
                Log log = logQueue.Dequeue();
                string path = Path.Combine(logDir, log.LogDate + "_" + logFile);
                try
                {
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    using (FileStream stream = File.Open(path, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine($"{log.LogTime}	{log.Message}");
                        }
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message + "\r\n\r\n" + log.Message, "Ошибка записи журнала", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        /// <summary>
        /// Метод записи в лог
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Error"></param>
        public void WriteToLog(object message, bool Error = false)
        {
            Queue<Log> logQueue = LogWriter.logQueue;
            lock (logQueue)
            {
                string str = Error ? "[Error]\t" : "[Information]\t";
                Log item = new Log(str + message.ToString());
                LogWriter.logQueue.Enqueue(item);
                if ((LogWriter.logQueue.Count >= queueSize) || this.DoPeriodicFlush())
                {
                    this.FlushLog();
                }
            }
        }

        /// <summary>
        /// Получить экземпляр объекта LogWriter
        /// </summary>
        public static LogWriter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogWriter();
                    logQueue = new Queue<Log>();
                }
                return instance;
            }
        }
    }
}
