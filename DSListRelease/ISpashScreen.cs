using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    public interface ISplashScreen
    {
        void AddMessage(string message);
        void LoadComplete();
        void ShowError(string errortext, string errortitle = "", bool Close = true);
    }
}
