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
        private static ObservableCollection<Customer> _OpenedTT;

        public static ObservableCollection<Customer> OpenedTT
        {
            get
            {
                if (_OpenedTT == null)
                {
                    _OpenedTT = new ObservableCollection<Customer>();
                }
                return _OpenedTT;
            }
            set
            { _OpenedTT = value; }
        }




        public static void ShowBalloonWarning(string text, string title = "")
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "DSList";
            }
            if (IsWindows10)
            {
                cvzNotifyIcon.ShowBalloonTip(ToastManager.ToastTime, title, text==""?"-":text, ToolTipIcon.Warning);
            }
            else
            {
                ToastManager.Add(text, string.IsNullOrWhiteSpace(title) ? "DSList" : title, ToastType.Warning);
            }
        }


        private bool SaveNotes(Customer tt, bool closeQuery = false)
        {
            try
            {
                if (tt.NotesChanged)
                {
                    if (((tt.NotesLastWriteTime == DateTime.MinValue) && !System.IO.File.Exists(tt.GetNotesFileName(false, false))) || (tt.NotesLastWriteTime == System.IO.File.GetLastWriteTime(tt.GetNotesFileName(false, false))))
                    {
                        if (!closeQuery)
                        {
                            //tt.SaveNotes();
                            return true;
                        }
                        TaskDialogOptions options = new TaskDialogOptions
                        {
                            Owner = this,
                            Title = "DSList общие заметки",
                            Content = "Вы хотите сохранить изменения в заметках этой ЦВЗ?",
                            MainInstruction = $"{tt.NumberCVZ} - "
                        };
                        options.CustomButtons = new string[] { "Сохранить", "Не сохранять", "Отмена" };
                        options.DefaultButtonIndex = 0;
                        options.AllowDialogCancellation = true;
                        TaskDialogResult result = TaskDialog.Show(options);
                        if (!result.CustomButtonResult.HasValue)
                        {
                            return false;
                        }
                        switch (result.CustomButtonResult.Value)
                        {
                            case 0:
                                //tt.SaveNotes();
                                return true;

                            case 1:
                                //tt.LoadNotes();
                                return true;

                            case 2:
                                return false;
                        }
                    }
                    else
                    {
                        TaskDialogOptions options2 = new TaskDialogOptions
                        {
                            Owner = this,
                            Title = "DSList общие заметки",
                            Content = "Файл общих заметок этой ТТ был изменен другим пользователем.",
                            MainInstruction = $"{tt.NumberCVZ} - "
                        };
                        options2.CommandButtons = new string[] { "Сохранить мои изменения\nИзменения другого пользователя будут перезаписаны", "Отменить мои изменения\nИзменения другого пользователя будут сохранены", "Сохранить все изменения\nИзменения другого пользователя будут сохранены. Ваша заметка будет добавлена" };
                        options2.MainIcon = VistaTaskDialogIcon.Warning;
                        options2.DefaultButtonIndex = 0;
                        switch (TaskDialog.Show(options2).CommandButtonResult.Value)
                        {
                            case 0:
                                //tt.SaveNotes();
                                return true;

                            case 1:
                                // tt.LoadNotes();
                                return true;

                            case 2:
                                // tt.AppendNotes();
                                break;
                        }
                    }
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                this.Error(exception.Message, "Ошибка", exception.StackTrace);
                return false;
            }
        }

        private bool CheckCloseTabHotKey(ref System.Windows.Input.KeyEventArgs e)
        {
            if (Bindings.isttselected)
            {
                if (((e.Key == Key.W) && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)) && (this.Tabs.Items.Count > 0))
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        for (int i = OpenedTT.Count - 1; i >= 0; i--)
                        {
                            if (OpenedTT[i] != SelectedTT)
                            {
                                this.OpenedTTRemove(OpenedTT[i]);
                            }
                        }
                    }
                    else
                    {
                        this.CloseSelectedTab();
                    }
                    e.Handled = true;
                    return true;
                }
                if (((e.Key == Key.S) && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)) && (SelectedTT != null))
                {
                    this.SaveNotes(SelectedTT, false);
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }


        private static bool CheckFileDif(string oldfn, string newfn)
        {
            FileInfo info = new FileInfo(oldfn);
            FileInfo info2 = new FileInfo(newfn);
            return ((!System.IO.File.Exists(newfn) || (info.Length != info2.Length)) || (info.LastWriteTimeUtc != info2.LastWriteTimeUtc));
        }



        /// <summary>
        /// Метод логирования и отображения ошибки
        /// </summary>
        /// <param name="errorText">Текст ошибки</param>
        /// <param name="errorTitle">Текст шапки окна сообщения</param>
        /// <param name="errorTextMore">Расширенный текст ошибки</param>
        internal void Error(string errorText, string errorTitle = "Ошибка", string errorTextMore = "")
        {
            this.Log(errorTitle + ": " + errorText, true, false, errorTextMore, false);
            this.ShowError(errorText, errorTitle, errorTextMore, false);
        }

        /// <summary>
        /// Метод отображения ошибки
        /// </summary>
        /// <param name="errortext">Текст ошибки</param>
        /// <param name="errortitle">Текст шапки окна сообщения</param>
        /// <param name="errortextmore">Расширенный текст ошибки</param>
        /// <param name="closebutton">Отображение кнопки закрыть</param>
        private void ShowError(string errortext, string errortitle = "", string errortextmore = "", bool closebutton = false)
        {
            base.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
           {
               TaskDialogOptions options = new TaskDialogOptions
               {
                   Owner = this,
                   Title = "DSList",
                   Content = errortext
               };
               if (!string.IsNullOrWhiteSpace(errortitle))
               {
                   options.MainInstruction = errortitle;
               }
               if (!string.IsNullOrWhiteSpace(errortextmore))
               {
                   options.ExpandedInfo = errortextmore;
               }
               options.MainIcon = VistaTaskDialogIcon.Error;
               if (closebutton)
               {
                   options.CustomButtons = new string[] { "Закрыть" };
               }
               else
               {
                   options.CustomButtons = new string[] { "ОК" };
               }
               options.AllowDialogCancellation = true;
               TaskDialog.Show(options);
           });
        }

        /// <summary>
        /// Метод отправки логов (пока не используется)
        /// </summary>
        private void SendLogs()
        {
            string path = System.IO.Path.Combine(DBPath, "Logs", Environment.UserName);
            try
            {

                if (System.IO.File.Exists(System.IO.Path.Combine(DBPath, "sendlogs")))
                {
                    Directory.CreateDirectory(path);
                    foreach (string str2 in Directory.GetFiles(LogWriter.logDir))
                    {
                        string str3 = System.IO.Path.Combine(path, System.IO.Path.GetFileName(str2));
                        if (System.IO.File.Exists(str3))
                        {
                            System.IO.File.Move(str3, System.IO.Path.Combine(path, $"{System.IO.Path.GetFileNameWithoutExtension(str2)}{new Random().Next(0x3e8)}{System.IO.Path.GetExtension(str2)}"));
                        }
                        System.IO.File.Move(str2, System.IO.Path.Combine(path, System.IO.Path.GetFileName(str2)));
                    }
                }
            }
            catch (Exception exception)
            {
                this.Log("Ошибка отправки журналов: " + exception.Message, true, false, exception.StackTrace, false);
            }
        }

        /// <summary>
        /// Метод формирования и записи лога, вывод в StatusBar
        /// </summary>
        /// <param name="text">Инициатор лога</param>
        /// <param name="error">Признак того, что это ошибка</param>
        /// <param name="balloon">Отображение лога всплывающим окном</param>
        /// <param name="logMore">Расширенный текст лога</param>
        /// <param name="enableProgressBar">Отображение ProgressBar</param>
        public void Log(string text, bool error = false, bool balloon = false, string logMore = "", bool enableProgressBar = false)
        {
            string message = text;
            if (!string.IsNullOrWhiteSpace(logMore))
            {
                message = message + "\r\n" + logMore;
            }
            if (balloon | error)
            {
                if (error)
                {
                    ShowBalloonError(text, "");
                }
                else
                {
                    ShowBalloon(text, "");
                }
            }
            MainLog.WriteToLog(message, error);
            this.ShowTextOnStatusBar("[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + text, error, enableProgressBar);
        }

        /// <summary>
        /// Метод отображения текста в StatusBar
        /// </summary>
        /// <param name="text">Текст для отображения</param>
        /// <param name="error">Признак ошибки</param>
        /// <param name="enableProgressBar">Необходимость отображения ProgressBar</param>
        private void ShowTextOnStatusBar(string text, bool error = false, bool enableProgressBar = false)
        {
            base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
            {
                NewMainWindow.Bindings.StatusBarText = text;
                if (error)
                {
                    this.StatusError.Visibility = Visibility.Visible;
                }
                else
                {
                    this.StatusError.Visibility = Visibility.Collapsed;
                }
                if (this.progressBarStatus != null)
                {
                    this.progressBarStatus.Visibility = enableProgressBar ? Visibility.Visible : Visibility.Hidden;
                }
            });
        }

        /// <summary>
        /// Метод всплывающего окна
        /// </summary>
        /// <param name="text">Текст всплывающего окна</param>
        /// <param name="title">Текст шапки всплывающего окна</param>
        public static void ShowBalloon(string text, string title = "")
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "DSList";
            }
            if (IsWindows10)
            {
                cvzNotifyIcon.ShowBalloonTip(ToastManager.ToastTime, title, text, ToolTipIcon.Info);
            }
            else
            {
                ToastManager.Add(text, title, ToastType.Success);
            }
        }

        /// <summary>
        /// Метод всплывающего окна ошибки
        /// </summary>
        /// <param name="text">Текст всплывающего окна</param>
        /// <param name="title">Текст шапки всплывающего окна</param>
        public static void ShowBalloonError(string text, string title = "")
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Ошибка DSList";
            }
            if (IsWindows10)
            {
                cvzNotifyIcon.ShowBalloonTip(ToastManager.ToastTime, title, text, ToolTipIcon.Error);
            }
            else
            {
                ToastManager.Add(text, string.IsNullOrWhiteSpace(title) ? "Ошибка DSList" : title, ToastType.Error);
            }
        }


        /// <summary>
        /// Метод обработки нажатия кнопки "Настройки" в главном меню Ribbon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();
        }

        protected internal void OpenSettingsWindow(bool openPass = false, bool openPhoneIPNumber = false)
        {
            SettingsWindow window = new SettingsWindow
            {
                Owner = this,
                DataContext = base.DataContext,
                ListViewHotkeys = { ItemsSource = Hotkeys },
                ListViewCredentials = { ItemsSource = UserCredentials },
                Commands = new List<string>(),
                OpenPass = openPass,
                OpenPhoneIPNumber = openPhoneIPNumber,

            };

            foreach (RibbonButton button in FindLogicalChildren<RibbonButton>(this.RibbonWin))
            {
                if ((button.Label != null) && (button.Visibility == Visibility.Visible))
                {
                    window.Commands.Add(button.Label);
                }
            }
            foreach (RibbonMenuItem item in FindLogicalChildren<RibbonMenuItem>(this.RibbonWin))
            {
                if ((item.Header != null) && (item.Visibility == Visibility.Visible))
                {
                    window.Commands.Add(item.Header.ToString());
                }
            }
            window.Commands = (from s in window.Commands
                               orderby s
                               select s).ToList();
            window.ComboBoxCommand.ItemsSource = window.Commands;
            window.Keys = new List<Key>();
            foreach (Key key in (Key[])System.Enum.GetValues(typeof(Key)))
            {
                window.Keys.Add(key);
            }
            window.ComboBoxHotkey.ItemsSource = window.Keys;
            bool? nullable = window.ShowDialog();
            bool flag4 = true;
            if ((nullable.GetValueOrDefault() == flag4) ? nullable.HasValue : false)
            {
                this.SaveHotkeys();
                this.SaveUserCredentials();
            }
            else
            {
                this.LoadHotkeys();
                this.LoadUserCredentials();
            }
            this.LoadCredentialsFromSQLServer(settings.Fields.ConnectionString);
            RefreshCredentials();
        }

        /// <summary>
        /// Метод обработки события сохранения и загрузки настроек settings
        /// </summary>
        public void settings_OnSaveOrLoad()
        {
            this.HideOffline.IsChecked = new bool?(settings.Fields.HideOffline);
            ToastManager.ToastTime = settings.Fields.ToastShowTime;
        }

        /// <summary>
        /// Метод нажатия кнопки признака отображения Offline хостов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideOffline_Click(object sender, RoutedEventArgs e)
        {
            settings.Fields.HideOffline = this.HideOffline.IsChecked.Value;
        }

        private void ToggleSwitchTS_Click(object sender, RoutedEventArgs e)
        {
            Bindings.IsTSConnecting = this.ToggleSwitchTS.IsChecked.Value;
        }


        /// <summary>
        /// Метод запуска программы
        /// </summary>
        /// <param name="fileName">Имя программы, которая будет запускаться</param>
        /// <param name="arguments">Аргументы программы</param>
        /// <param name="wait">Параметр ожидания завершения программы</param>
        /// <param name="visible">Параметр видимости программы</param>
        /// <param name="directory">Путь к программе</param>
        /// <param name="admin">Параметр запуска программы под правами администратора</param>
        /// <param name="username">Пользователь, под которым будет запускаться программа</param>
        /// <param name="password">Пароль пользователя, при запуске с правами админа</param>
        /// <returns>Код завершения программы, при корректном завершении программы = 0</returns>
        public static int ExecuteProgram(string fileName, string arguments = "", bool wait = false, bool visible = true, string directory = "", bool admin = false, string username = "", string password = "")
        {
            Process process = null;
            int exitCode = 0;
            try
            {
                process = new Process
                {
                    StartInfo = { FileName = fileName }
                };
                if (directory != string.Empty)
                {
                    process.StartInfo.WorkingDirectory = directory;
                }
                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                if (!visible)
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                if (admin)
                {
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Verb = "runas";
                }
                if ((!admin && !string.IsNullOrWhiteSpace(username)) && !string.IsNullOrWhiteSpace(password))
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.UserName = username;
                    process.StartInfo.Password = ReadPassword(password);
                    process.StartInfo.LoadUserProfile = true;
                }
                process.Start();
                if (wait)
                {
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                }
            }
            catch (Exception)
            {
                exitCode = 1;
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
            return exitCode;
        }

        /// <summary>
        /// Метод безопасного чтения пароля, применяется для передачи пароля приложению
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SecureString ReadPassword(string password)
        {
            SecureString str = new SecureString();
            for (int i = 0; i < password.Length; i++)
            {
                str.AppendChar(password[i]);
            }
            return str;
        }



        /// <summary>
        /// Метод копирования в буфер обмена
        /// </summary>
        /// <param name="text">Текст, который копируется в буфер обмена</param>
        /// <param name="log">добавлять в лог и выводить в StatusBar</param>
        /// <param name="successText">Текст, который будет добавляться в StatusBar, по умолчанию ""</param>
        public void CopyToClipboard(string text, bool log = true, string successText = "", bool showInfo = false)
        {

            if (SelectedTT != null)
            {
                SelectedTT.CloseCounter = 0;
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetDataObject(text, true, 10, 200);
                    if (log)
                    {
                        Log((successText == string.Empty) ? ("\"" + text + "\" скопирован в буфер обмена.") : successText, false, showInfo);
                    }
                }
                catch (Exception exception)
                {
                    if (log)
                    {
                        Log("Ошибка копирования в буфер. " + exception.Message, true, true, exception.StackTrace);
                    }

                }
            }
        }


        /// <summary>
        /// Метод прохода по DependencyObject и получения логических элементов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns>IEnumerable коллекция логических элементов</returns>
        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (object rawChild in LogicalTreeHelper.GetChildren(depObj))
                {
                    if (rawChild is DependencyObject)
                    {
                        DependencyObject child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindLogicalChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Метод проверки корректности введенного IP адреса
        /// </summary>
        /// <param name="ipAddress">строковый вариант IP адреса</param>
        /// <returns>возвращает булевое значение корректности IP адреса</returns>
        private bool VerifyCorrectIpAddressInString(string ipAddress)
        {
            //Инициализируем новый экземпляр класса System.Text.RegularExpressions.Regex
            //для указанного регулярного выражения.
            Regex IpMatch = new Regex(@"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)");

            //Выполняем проверку обнаружено ли в указанной входной строке 
            //соответствие регулярному выражению, заданному в
            //конструкторе System.Text.RegularExpressions.Regex.
            //если да то возвращаем true, если нет то false
            return IpMatch.IsMatch(ipAddress);


        }


        /// <summary>
        /// Метод открытия ссылок на ресурсы в браузере по умолчанию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Process process = new Process();
                //process.StartInfo.FileName = (sender as RibbonMenuItem).Tag.ToString();
                ////process.StartInfo.FileName = "cmd.exe|/k whoami";
                //process.Start();
                StartProgramSupportTools((sender as RibbonMenuItem).Tag.ToString(), "", true);

            }
            catch (Exception ex)
            {
                Log("Ошибка открытия ресурса в браузере по умолчанию" + ex.Message, true, true, ex.StackTrace);
            }

        }

        /// <summary>
        /// Метод запуска программы
        /// </summary>
        /// <param name="exeFileName">Путь к файлу запускаемой программы</param>
        /// <param name="arguments">вводимые атрибуты</param>
        private void StartProgramSupportTools(string exeFileName, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = Environment.CurrentDirectory + "\\SupportTools\\" + exeFileName;
                // Пример аргумента для коммандной строки $"/k ping {SelectedIP.Address} /t";
                psi.Arguments = arguments;
                Process.Start(psi);
                Log($"Выполнен запуск {exeFileName} {arguments}", false, false);
            }
            catch (Exception ex)
            {
                Log("Ошибка запуска Tools." + ex.Message, true, true, ex.StackTrace);
            }

        }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        /// <param name="exeFileName">имя запускаемого файла</param>
        /// <param name="arguments">аргументы для запуска</param>
        /// <param name="defaultPath">признак выбора расположения файла( true - ОС сама ищет файл, false - файл находится в SupportTools</param>
        private void StartProgramSupportTools(string exeFileName, string arguments, bool defaultPath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                if (!defaultPath)
                    psi.FileName = Environment.CurrentDirectory + "\\SupportTools\\" + exeFileName;
                else
                    psi.FileName = exeFileName;

                // Пример аргумента для коммандной строки $"/k ping {SelectedIP.Address} /t";
                psi.Arguments = arguments;
                Process.Start(psi);
                Log($"Выполнен запуск {exeFileName} {arguments}", false, false);
            }
            catch (Exception ex)
            {
                Log("Ошибка запуска программы. " + ex.Message, true, true, ex.StackTrace);
            }

        }

        ///// <summary>
        ///// Запуск приложения
        ///// </summary>
        ///// <param name="exeFileName">имя запускаемого файла</param>
        ///// <param name="arguments">аргументы для запуска</param>
        ///// <param name="defaultPath">признак выбора расположения файла( true - ОС сама ищет файл, false - файл находится в SupportTools</param>
        //private void StartProgramSupportTools(string exeFileName, string arguments, bool defaultPath)
        //{
        //    try
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo();
        //        if (!defaultPath)
        //            psi.FileName = Environment.CurrentDirectory + "\\SupportTools\\" + exeFileName;
        //        else
        //            psi.FileName = exeFileName;

        //        // Пример аргумента для коммандной строки $"/k ping {SelectedIP.Address} /t";
        //        psi.Arguments = arguments;
        //        Process.Start(psi);
        //        Log($"Выполнен запуск {exeFileName} {arguments}", false, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("Ошибка запуска программы. " + ex.Message, true, true, ex.StackTrace);
        //    }

        //}


        /// <summary>
        /// Метод обработки открытия папок общих ресурсов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RibbonMenuItemOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //ProcessStartInfo process = new ProcessStartInfo();
                //process.FileName = "explorer.exe";
                //process.Arguments = (sender as RibbonMenuItem).Tag.ToString();
                //Process.Start(process);

                StartProgramSupportTools("explorer.exe", (sender as RibbonMenuItem).Tag.ToString(), true);

            }
            catch (Exception ex)
            {
                Log("Ошибка открытия ресурса " + ex.Message, true, true, ex.StackTrace);
            }
        }


    }
}
