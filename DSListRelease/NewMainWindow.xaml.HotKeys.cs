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
namespace DSList
{
    public partial class NewMainWindow
    {
        #region Поля
        /// <summary>
        /// Поле расположения файла настроек с кнопками быстрого вызова.
        /// </summary>
        private static string Hotkeys_xmlfile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DSList\Hotkeys.xml");


        #endregion

        #region Свойства
        /// <summary>
        /// Свойство коллекция кнопок быстрого вызова
        /// </summary>
        private static ObservableCollection<Hotkey> Hotkeys { get; set; }

        #endregion

        #region Методы

        /// <summary>
        /// Метод сохранение коллекции быстрых кнопок в Hotkeys_xmlfile
        /// </summary>
        public void SaveHotkeys()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Hotkey>));
                StreamWriter writer = new StreamWriter(Hotkeys_xmlfile);
                serializer.Serialize((TextWriter)writer, Hotkeys);
                writer.Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Метод загрузка коллекции быстрых кнопок из Hotkeys_xmlfile
        /// </summary>
        public void LoadHotkeys()
        {

            try
            {
                if (System.IO.File.Exists(Hotkeys_xmlfile))
                {
                    using (StreamReader reader = new StreamReader(Hotkeys_xmlfile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Hotkey>));
                        Hotkeys = serializer.Deserialize(reader) as ObservableCollection<Hotkey>;
                    }
                }
                else
                {
                    Hotkeys = new ObservableCollection<Hotkey>();
                }
            }
            catch (Exception ex)
            {
                //this.Error(exception.Message, "Ошибка при загрузке списка горячих клавиш", exception.StackTrace);
                Log("Ошибка при загрузке списка горячих клавиш. " + ex.Message, true, true, ex.StackTrace);
                Hotkeys = new ObservableCollection<Hotkey>();
            }

        }

        /// <summary>
        /// Метод проверка всех комбинаций быстрых кнопок при нажатии
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckAllWindowHotKey(ref System.Windows.Input.KeyEventArgs e)
        {
            Key PressedKey;
            bool Shift;
            bool Control;
            Hotkey hk;
            if (/*Bindings.GrantedAccess.HasFlag(AccessRoles.Full)*/ true)
            {
                Shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                Control = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                bool flag = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
                PressedKey = e.Key;
                hk = Hotkeys.FirstOrDefault<Hotkey>(p => ((p.HotKey == PressedKey) && !(Shift ^ p.Shift)) && (Control == p.Control));
                if ((hk != null) && !string.IsNullOrEmpty(hk.Command))
                {
                    RibbonButton button = FindLogicalChildren<RibbonButton>(RibbonWin).FirstOrDefault<RibbonButton>(p => (p.Label != null) && (p.Label.ToString().ToLower() == hk.Command.ToLower()));
                    if (button != null)
                    {
                        if ((button.Command != null) && button.Command.CanExecute(button.CommandParameter))
                        {
                            button.Command.Execute(button.CommandParameter);
                        }
                        if (button.IsEnabled && (button.Visibility == Visibility.Visible))
                        {
                            button.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                        }
                    }
                    else
                    {
                        RibbonMenuItem item = FindLogicalChildren<RibbonMenuItem>(RibbonWin).FirstOrDefault<RibbonMenuItem>(p => (p != null) && (p.Header.ToString().ToLower() == hk.Command.ToLower()));
                        if ((item.Command != null) && item.Command.CanExecute(item.CommandParameter))
                        {
                            item.Command.Execute(item.CommandParameter);
                        }
                        if (((item != null) && item.IsEnabled) && (item.Visibility == Visibility.Visible))
                        {
                            item.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.MenuItem.ClickEvent));
                        }
                    }
                    e.Handled = true;
                    return true;
                }
                switch (e.Key)
                {
                    case Key.F1:
                        listBox.SelectedItem = lbiHelp;
                        e.Handled = true;
                        return true;

                    case Key.F:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            listBox.SelectedItem = listBoxItemSearchCVZ;
                            Dispatcher.Invoke(() => { TextBoxSearch.Focus(); });
                            e.Handled = true;
                        }
                        
                        return true;

                    //case Key.F2:
                    //    if (this.DiagnosticsAndSettingsTab.IsEnabled)
                    //    {
                    //        this.RibbonWin.SelectedIndex = 1;
                    //    }
                    //    e.Handled = true;
                    //    return true;

                    //case Key.F3:
                    //    if (this.NetworkTab.IsEnabled)
                    //    {
                    //        this.RibbonWin.SelectedIndex = 2;
                    //    }
                    //    e.Handled = true;
                    //    return true;

                    case Key.F4:
                        DSList.Properties.Settings.Default.ProvInfoExpanded = !Properties.Settings.Default.ProvInfoExpanded;
                        e.Handled = true;
                        return true;

                    case Key.F5:
                        Dispatcher.Invoke(async () => { await PingSelectedTT(false); });
                        e.Handled = true;
                        return true;

                    case Key.Escape:
                        this.ClearSearch();
                        e.Handled = true;
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Метод проверка комбинаций быстрых кнопок при нажатии в TextBoxSearch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSearch_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.CheckAllWindowHotKey(ref e) && !this.CheckCloseTabHotKey(ref e))
            {
                switch (e.Key)
                {
                    case Key.Down:
                        e.Handled = true;
                        this.SearchListBoxSetFocus();
                        break;

                    case Key.Add:
                        //this.OpenCustomTT();
                        OpenCustomSubnet();
                        e.Handled = true;
                        break;

                    case Key.Decimal:
                        {
                            System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
                            int selectionStart = box.SelectionStart;
                            box.Text = box.Text.Insert(selectionStart, ".");
                            box.SelectionStart = selectionStart + 1;
                            e.Handled = true;
                            break;
                        }
                    case Key.Tab:
                        Keyboard.Focus(this.Tabs);
                        e.Handled = true;
                        break;

                    case Key.Return:
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            if (settings.Fields.AlwaysAdvSearch)
                            {
                                this.FillSearchList();
                            }
                            else
                            {
                                this.FillSearchList(true);
                            }
                            e.Handled = true;
                        }
                        else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            this.SearchIPOrName();
                            e.Handled = true;
                        }
                        else
                        {
                            this.TextBoxSearch.SelectAll();
                            this.SearchListBoxSetFocus();
                            //if (this.searchListBox.SelectedIndex != -1)
                            //{
                            //    this.OpenTTinTab(this.searchListBox.SelectedItem as Customer, false);
                            //}
                            e.Handled = true;
                        }
                        break;

                    case Key.Up:
                        this.SearchListBoxSetFocus();
                        break;
                }
            }
        }





        #endregion
    }
}
