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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Ribbon;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using AngleSharp;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Globalization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using TaskDialogInterop;
using System.Windows.Forms;
using System.Security;

namespace DSList
{
    public partial class NewMainWindow
    {
        private void Notes_Clear_Click(object sender, RoutedEventArgs e)
        {
            SelectedTT.Notes = string.Empty;
        }

        private void Notes_Load_Click(object sender, RoutedEventArgs e)
        {
            //SelectedTT.LoadNotes();
        }

        private void Notes_Save_Click(object sender, RoutedEventArgs e)
        {
            this.SaveNotes(SelectedTT, false);
        }
    }
}
