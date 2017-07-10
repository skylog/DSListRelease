using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using TaskDialogInterop;

namespace DSList
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IComponentConnector
    {
        #region Свойства

        public List<string> Commands { get; set; }

        public List<Key> Keys { get; set; }
        public bool OpenPass { get; set; }

        public bool OpenPhoneIPNumber { get; set; }

        #endregion

        public SettingsWindow()
        {
            InitializeComponent();

        }

        #region Общие настройки

        /// <summary>
        /// SettingsWindow Метод обработки нажатия кнопки "Обзор" DW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filezilla_path_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = this.filezilla_path.Text;
                if (this.LocateFile(ref text, "FileZilla*.exe"))
                {
                    this.filezilla_path.Text = text;
                }
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// SettingsWindow Метод обработки нажатия кнопки "Обзор" CmRcViewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void putty_path_Click(object sender, RoutedEventArgs e)
        {
            string text = this.putty_path.Text;
            if (this.LocateFile(ref text, "putty.exe"))
            {
                this.putty_path.Text = text;
            }
        }

        /// <summary>
        /// SettingsWindow Метод обработки нажатия кнопки "Обзор" SQL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winboxmikrotik_path_Click(object sender, RoutedEventArgs e)
        {
            string text = this.winboxmikrotik_path.Text;
            if (this.LocateFile(ref text, "winbox.exe"))
            {
                this.winboxmikrotik_path.Text = text;
            }
        }


        #endregion

        #region HotKeys

        /// <summary>
        /// SettingsWindow Метод редактирования горячей кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditHotkey_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(this.ComboBoxCommand.Text) && !string.IsNullOrWhiteSpace(this.ComboBoxCommand.Text)) && (this.ListViewHotkeys.SelectedIndex > -1))
            {
                ObservableCollection<Hotkey> itemsSource = this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>;
                Hotkey selectedItem = this.ListViewHotkeys.SelectedItem as Hotkey;
                selectedItem.Control = this.CheckBoxControl.IsChecked.Value;
                selectedItem.Shift = this.CheckBoxShift.IsChecked.Value;
                selectedItem.HotKey = (Key)this.ComboBoxHotkey.SelectedItem;
                selectedItem.Command = this.ComboBoxCommand.Text;

            }
        }

        /// <summary>
        /// SettingsWindow Метод обработки изменения выбора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hotkeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListViewHotkeys.SelectedIndex != -1)
            {
                Hotkey selectedItem = this.ListViewHotkeys.SelectedItem as Hotkey;
                this.CheckBoxControl.IsChecked = new bool?(selectedItem.Control);
                this.CheckBoxShift.IsChecked = new bool?(selectedItem.Shift);
                for (int i = 0; i < this.Keys.Count; i++)
                {
                    Key key = this.Keys[i];
                    if (key.ToString() == selectedItem.HotKey.ToString())
                    {
                        this.ComboBoxHotkey.SelectedIndex = i;
                        break;
                    }
                }
                for (int j = 0; j < this.Commands.Count; j++)
                {
                    if (this.Commands[j] == selectedItem.Command)
                    {
                        this.ComboBoxCommand.SelectedIndex = j;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// SettingsWindow Метод удаления выбранной горячей кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteHotkey_Click(object sender, RoutedEventArgs e)
        {
            if ((this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>).Count != 0)
            {
                if (this.ListViewHotkeys.SelectedIndex == -1)
                {
                    this.ListViewHotkeys.SelectedIndex = (this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>).Count - 1;
                }
                int selectedIndex = this.ListViewHotkeys.SelectedIndex;
                if (this.ListViewHotkeys.SelectedIndex > -1)
                {
                    (this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>).RemoveAt(this.ListViewHotkeys.SelectedIndex);
                    if (selectedIndex <= ((this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>).Count - 1))
                    {
                        this.ListViewHotkeys.SelectedIndex = selectedIndex;
                    }
                }
            }
        }

        /// <summary>
        /// SettingsWindow Очистка списка горячих кнопок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearHotkey_Click(object sender, RoutedEventArgs e)
        {
            (this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>).Clear();
        }

        /// <summary>
        /// SettingsWindow Нажатие кнопки добавления связки горячая кнопка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddHotkey_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.ComboBoxCommand.Text) && !string.IsNullOrWhiteSpace(this.ComboBoxCommand.Text))
            {
                ObservableCollection<Hotkey> itemsSource = this.ListViewHotkeys.ItemsSource as ObservableCollection<Hotkey>;
                Hotkey hk = new Hotkey
                {
                    Control = this.CheckBoxControl.IsChecked.Value,
                    Shift = this.CheckBoxShift.IsChecked.Value,
                    HotKey = (Key)this.ComboBoxHotkey.SelectedItem,
                    Command = this.ComboBoxCommand.Text
                };
                if (itemsSource.FirstOrDefault<Hotkey>(p => (((p.Control == hk.Control) && (p.Shift == hk.Shift)) && (p.HotKey == hk.HotKey))) == null)
                {
                    itemsSource.Add(hk);
                }
            }
        }
        #endregion

        #region Логины и пароли

        /// <summary>
        /// SettingsWindow Метод обработки нажатия кнопки удаления выбранной связки логин, пароль и т.д.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCredentials_Click(object sender, RoutedEventArgs e)
        {
            if ((this.ListViewCredentials.ItemsSource as ObservableCollection<Credential>).Count != 0)
            {
                if (this.ListViewCredentials.SelectedIndex == -1)
                {
                    this.ListViewCredentials.SelectedIndex = (this.ListViewCredentials.ItemsSource as ObservableCollection<Credential>).Count - 1;
                }
                int selectedIndex = this.ListViewCredentials.SelectedIndex;
                if (this.ListViewCredentials.SelectedIndex > -1)
                {
                    (this.ListViewCredentials.ItemsSource as ObservableCollection<Credential>).RemoveAt(this.ListViewCredentials.SelectedIndex);
                    if (selectedIndex <= ((this.ListViewCredentials.ItemsSource as ObservableCollection<Credential>).Count - 1))
                    {
                        this.ListViewCredentials.SelectedIndex = selectedIndex;
                    }
                }
            }
        }


        /// <summary>
        /// SettingsWindow Метод очистки всего списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearCredentials_Click(object sender, RoutedEventArgs e)
        {
            (this.ListViewCredentials.ItemsSource as ObservableCollection<Credential>).Clear();
        }

        /// <summary>
        /// SettingsWindow Нажатие кнопки добавления связки логина, пароля и т.д.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddCredentials_Click(object sender, RoutedEventArgs e)
        {

            ObservableCollection<Credential> itemsSource = ListViewCredentials.ItemsSource as ObservableCollection<Credential>;
            Credential item = new Credential
            {
                CustomPassword = true,
                Password = string.Empty,
                Login = string.Empty
            };
            itemsSource.Add(item);
        }

        #endregion

        #region Общие кнопки в SettingsWindow и методы

        /// <summary>
        /// SettingsWindow Нажатие кнопки "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        /// <summary>
        /// SettingsWindows Метод обработки выбора файла
        /// </summary>
        /// <param name="FullFilePath"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        internal bool LocateFile(ref string FullFilePath, string fn)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Укажите расположение " + fn,
                    InitialDirectory = /*Path.GetExtension(*/FullFilePath/*)*/,
                    Filter = fn + "|" + fn + "|Программы|*.exe|Все файлы|*.*"
                };
                bool? nullable = dialog.ShowDialog();
                bool flag2 = true;
                if ((nullable.GetValueOrDefault() == flag2) ? nullable.HasValue : false)
                {
                    FullFilePath = dialog.FileName;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Укажите расположение " + fn,
                    //InitialDirectory = /*Path.GetExtension(*/FullFilePath/*)*/,
                    Filter = fn + "|" + fn + "|Программы|*.exe|Все файлы|*.*"
                };
                bool? nullable = dialog.ShowDialog();
                bool flag2 = true;
                if ((nullable.GetValueOrDefault() == flag2) ? nullable.HasValue : false)
                {
                    FullFilePath = dialog.FileName;
                    return true;
                }
                return false;
            }

        }

        /// <summary>
        /// SettingsWindow Метод обработки кнопки "ОК"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBoxAutorun.IsChecked.Value)
            {
                if (!AutoStart.EnableAutoStart())
                {
                    TaskDialogOptions options = new TaskDialogOptions
                    {
                        Owner = this,
                        Title = "DSList",
                        Content = "Программа установлена некорректно. Невозможно добавить в автозагрузку.",
                        MainIcon = VistaTaskDialogIcon.Error
                    };
                    options.CustomButtons = new string[] { "ОК" };
                    options.AllowDialogCancellation = true;
                    TaskDialog.Show(options);
                    this.CheckBoxAutorun.IsChecked = false;
                    return;
                }
            }
            else
            {
                AutoStart.DisableSetAutoStart();
            }
            NewMainWindow.settings.Fields.autoupdate = this.CheckBoxAutoUpdate.IsChecked.Value;
            NewMainWindow.settings.Fields.restart_confirm = this.CheckBoxAskRestart.IsChecked.Value;
            NewMainWindow.settings.Fields.PingDelay = this.CheckBoxPingDelay.IsChecked.Value;
            NewMainWindow.settings.Fields.DoubleClickPing = this.CheckBoxDoubleClickPing.IsChecked.Value;
            NewMainWindow.settings.Fields.ChangeLayout = this.CheckBoxChangeLayout.IsChecked.Value;
            NewMainWindow.settings.Fields.OneTab = this.CheckBoxOneTab.IsChecked.Value;
            NewMainWindow.settings.Fields.PopupMenuDelayed = this.CheckPopupMenuDelayed.IsChecked.Value;
            NewMainWindow.settings.Fields.AutoCloseTab = this.CheckBoxAutoCloseTab.IsChecked.Value;
            //MainWindow.settings.Fields.DWDirect = this.CheckBoxDWDirect.IsChecked.Value;
            NewMainWindow.settings.Fields.ShowClosed = this.CheckBoxShowClosed.IsChecked.Value;
            NewMainWindow.settings.Fields.SearchGroupByStatus = this.CheckBoxSearchGroupByStatus.IsChecked.Value;
            NewMainWindow.settings.Fields.AlwaysAdvSearch = this.CheckBoxAlwaysAdvSearch.IsChecked.Value;
            NewMainWindow.settings.Fields.MinimizeOnClose = this.CheckBoxMinimize.IsChecked.Value;
            //MainWindow.settings.Fields.AdditionalProvIP = this.CheckBoxAdditionalProvIP.IsChecked.Value;
            NewMainWindow.settings.Fields.DetectTypeByName = this.CheckBoxDetectTypeByName.IsChecked.Value;
            NewMainWindow.settings.Fields.filezilla_path = this.filezilla_path.Text;
            NewMainWindow.settings.Fields.putty_path = this.putty_path.Text;
            NewMainWindow.settings.Fields.winboxmikrotik_path_usedefault = winboxmikrotik_path_usedefault.IsChecked.Value;
            NewMainWindow.settings.Fields.winboxmikrotikpath = this.winboxmikrotik_path.Text;
            NewMainWindow.settings.Fields.CloseTabTime = int.Parse(this.ComboBoxCloseTabTime.Text);
            NewMainWindow.settings.Fields.BigPopupToolbar = this.CheckBoxBigPopupToolbar.IsChecked.Value;
            NewMainWindow.settings.Fields.ToastShowTime = (int)this.ToastShowTime.Value;
            NewMainWindow.settings.Fields.PhoneIPNumber = tbPhoneIPNumber.Text;
            NewMainWindow.settings.SaveSettings(true);
            base.DialogResult = true;
            base.Close();
        }

        /// <summary>
        /// SettingsWindow Метод заполнения элементов окна после загрузки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            base.MinWidth = base.Width;
            base.MinHeight = base.Height;
            this.CheckBoxBigPopupToolbar.IsChecked = new bool?(NewMainWindow.settings.Fields.BigPopupToolbar);
            this.ToastShowTime.Value = NewMainWindow.settings.Fields.ToastShowTime;
            this.CheckBoxManualRestart.IsChecked = new bool?(!NewMainWindow.settings.Fields.restart_confirm || !NewMainWindow.settings.Fields.autoupdate);
            this.CheckBoxAutorun.IsChecked = new bool?(AutoStart.IsAutoStartEnabled);
            this.CheckBoxAskRestart.IsChecked = new bool?(NewMainWindow.settings.Fields.restart_confirm);
            this.CheckBoxAutoUpdate.IsChecked = new bool?(NewMainWindow.settings.Fields.autoupdate);
            this.CheckBoxPingDelay.IsChecked = new bool?(NewMainWindow.settings.Fields.PingDelay);
            this.CheckPopupMenuDelayed.IsChecked = new bool?(NewMainWindow.settings.Fields.PopupMenuDelayed);
            this.CheckBoxDoubleClickPing.IsChecked = new bool?(NewMainWindow.settings.Fields.DoubleClickPing);
            this.CheckBoxChangeLayout.IsChecked = new bool?(NewMainWindow.settings.Fields.ChangeLayout);
            this.CheckBoxOneTab.IsChecked = new bool?(NewMainWindow.settings.Fields.OneTab);
            this.CheckBoxAutoCloseTab.IsChecked = new bool?(NewMainWindow.settings.Fields.AutoCloseTab);
            this.CheckBoxSearchGroupByStatus.IsChecked = new bool?(NewMainWindow.settings.Fields.SearchGroupByStatus);
            //this.CheckBoxDWDirect.IsChecked = new bool?(NewMainWindow.settings.Fields.DWDirect);
            this.CheckBoxShowClosed.IsChecked = new bool?(NewMainWindow.settings.Fields.ShowClosed);
            this.CheckBoxAlwaysAdvSearch.IsChecked = new bool?(NewMainWindow.settings.Fields.AlwaysAdvSearch);
            this.CheckBoxMinimize.IsChecked = new bool?(NewMainWindow.settings.Fields.MinimizeOnClose);
            //this.CheckBoxAdditionalProvIP.IsChecked = new bool?(NewMainWindow.settings.Fields.AdditionalProvIP);
            this.CheckBoxDetectTypeByName.IsChecked = new bool?(NewMainWindow.settings.Fields.DetectTypeByName);
            this.CheckBoxStandartTab.IsChecked = new bool?(!NewMainWindow.settings.Fields.AutoCloseTab && !NewMainWindow.settings.Fields.OneTab);
            this.ComboBoxCloseTabTime.Text = NewMainWindow.settings.Fields.CloseTabTime.ToString();
            this.filezilla_path.Text = NewMainWindow.settings.Fields.filezilla_path;
            this.putty_path.Text = NewMainWindow.settings.Fields.putty_path;
            winboxmikrotik_path_usedefault.IsChecked = new bool?(NewMainWindow.settings.Fields.winboxmikrotik_path_usedefault);
            this.winboxmikrotik_path.Text = NewMainWindow.settings.Fields.winboxmikrotikpath;
            tbPhoneIPNumber.Text = NewMainWindow.settings.Fields.PhoneIPNumber;
            if (OpenPass)
            {
                TabPass.Focus();
            }
            if (OpenPhoneIPNumber)
            {
                tbPhoneIPNumber.Focus();
            }
        }
        #endregion


    }
}
