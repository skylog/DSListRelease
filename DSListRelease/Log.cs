using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DSList
{
    /// <summary>
    /// Класс лога событий
    /// </summary>
    public class Log
    {
        public Log(string message)
        {
            this.Message = message;
            this.LogDate = DateTime.Now.ToString("yyyy-MM-dd");
            this.LogTime = DateTime.Now.ToString("HH:mm:ss.fff");
        }

        public string LogDate { get; set; }

        public string LogTime { get; set; }

        public string Message { get; set; }
    }
}
