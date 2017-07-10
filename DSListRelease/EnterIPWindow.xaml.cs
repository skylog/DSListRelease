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
using System.Net;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для EnterIPWindow.xaml
    /// </summary>
    public partial class EnterIPWindow : Window, IComponentConnector
    {
        public EnterIPWindow()
        {
            this.InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private static bool CheckIP(string IP)
        {
            IPAddress address;
            return IPAddress.TryParse(IP, out address);
        }

        private static bool CheckIP(TextBox TB, string TooltipText)
        {
            if (!CheckIP(TB.Text))
            {
                TB.Focus();
                TB.SelectAll();
                ToolTip tip = new ToolTip
                {
                    Content = TooltipText,
                    Placement = PlacementMode.Right,
                    PlacementTarget = TB
                };
                TB.ToolTip = tip;
                tip.IsOpen = true;
                TB.Background = Brushes.LightPink;
                return false;
            }
            TB.ClearValue(BackgroundProperty);
            TB.ClearValue(ToolTipProperty);
            return true;
        }


        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (CheckIP(this.TextBoxIP, "Введен неверный IP адрес") & CheckIP(this.TextBoxSM, "Введена неверная маска подсети"))
            {
                DialogResult = true;
                Close();
            }
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            ToolTip toolTip = (ToolTip)((TextBox)sender).ToolTip;
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
            if (e.Key == Key.Decimal)
            {
                TextBox box = (TextBox)sender;
                int selectionStart = box.SelectionStart;
                box.Text = box.Text.Insert(selectionStart, ".");
                box.SelectionStart = selectionStart + 1;
                e.Handled = true;
            }
        }
    }
}
