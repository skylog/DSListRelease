using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DSList
{
    public partial class NewMainWindow
    {
        private string MRU_xmlfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DSList\MRU.xml");

        private static ObservableCollection<MRUTT> MRU { get; set; }

        //MainWindow.MRU = new ObservableCollection<MRUTT>();
        //    this.<> 4__this.LoadMRU();
        //      this.<> 4__this.ListViewMRU.ItemsSource = MainWindow.MRU;




        //internal System.Windows.Controls.ListView ListViewMRU;


        private void MRUItem_Click(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null)
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is ListViewItem)
                {
                    Customer selCust = FindTTbyCode(((dep as ListViewItem).Content as MRUTT).Code, this.AllTT);
                    if (selCust != null)
                    {
                        OpenTTinTab(selCust, true);
                        //searchListBox.SelectedValue = selCust;
                        OnMouseClickSearchListBox(ListViewMRU);
                    }
                    else
                    {
                        this.TextBoxSearch.Text = ((dep as ListViewItem).Content as MRUTT).Code;
                    }
                }
            }
            listBox.SelectedItem = listBoxItemSearchCVZ;
            //tabItemSearchCVZ.Focus();

            //this.ListViewMRU.SelectedItem = null;
            //this.RibbonMenu.IsDropDownOpen = false;
        }


        private void LoadMRU()
        {
            try
            {
                using (StreamReader reader = new StreamReader(this.MRU_xmlfile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MRUTT>));
                    MRU = serializer.Deserialize(reader) as ObservableCollection<MRUTT>;
                }
            }
            catch
            {
            }
        }

        private void SaveMRU()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<MRUTT>));
                StreamWriter writer = new StreamWriter(this.MRU_xmlfile);
                serializer.Serialize((TextWriter)writer, MRU);
                writer.Close();
            }
            catch
            {
            }
        }

        

        private void Search_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (!this.CheckAllWindowHotKey(ref e) && !this.CheckCloseTabHotKey(ref e))
            //{
            //    if ((e.Key == Key.Return) && (this.ListViewSearch.SelectedIndex != -1))
            //    {
            //        this.OpenTTinTab(this.ListViewSearch.SelectedItem as Customer, true);
            //    }
            //    Key[] source = new Key[] { Key.Tab };
            //    if (!source.Contains<Key>(e.Key))
            //    {
            //        this.TextBoxSearch.Focus();
            //        this.TextBoxSearch.SelectAll();
            //    }
            //}
        }

        void test()
        {
            //this.<> 8__1.MRU_temp = new MRUTT();
            //this.<> 8__1.MRU_temp.Code = this.selected.Code;
            //this.<> 8__1.MRU_temp.Name = this.selected.Name;
            //this.<> 8__1.MRU_temp.Time = $"{DateTime.Now:HH:mm}";
            //MainWindow.MRU = new ObservableCollection<MRUTT>(MainWindow.MRU.Where<MRUTT>(new Func<MRUTT, bool>(this.<> 8__1.< OpenTTinTab > b__0)).Take<MRUTT>(11).ToList<MRUTT>());
            //MainWindow.MRU.Insert(0, this.<> 8__1.MRU_temp);
            //this.<> 4__this.ListViewMRU.ItemsSource = MainWindow.MRU;
            //this.<> 4__this.SaveMRU();
        }
    }
}
