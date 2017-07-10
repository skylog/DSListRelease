using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{

    public static class AutoStart
    {
        public static void DisableSetAutoStart()
        {
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Startup\DSList.appref-ms");
            }
            catch (Exception)
            {
            }
        }

        public static bool EnableAutoStart()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\DSList\DSList.appref-ms"))
            {
                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\DSList\DSList.appref-ms", Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Startup\DSList.appref-ms", true);
                return true;
            }
            return false;
        }

        public static bool IsAutoStartEnabled =>
            File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Startup\DSList.appref-ms");
    }
}

