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

namespace DSList
{
    public partial class NewMainWindow
    {
        #region Формирование письма при замене МФУ

        public class MFU : ModelBase
        {
            private CollectionView _ReasonReplacement;
            private CollectionView _ModelMFU;

            public CollectionView ReasonReplacement
            {
                get { return this._ReasonReplacement; }

                set
                {
                    this._ReasonReplacement = value;
                    base.NotifyPropertyChanged("ReasonReplacement");
                }
            }

            public CollectionView ModelMFU
            {
                get { return this._ModelMFU; }

                set
                {
                    this._ModelMFU = value;
                    base.NotifyPropertyChanged("ModelMFU");
                }
            }

            public MFU()
            {
                List<BindingString> list = new List<BindingString>();
                list.Add(new BindingString("МФУ доставлен на время ремонта"));
                list.Add(new BindingString("Вернулся МФУ, который числится за данным ЦВЗ"));
                ReasonReplacement = new CollectionView(list);

                list = new List<BindingString>();
                list.Add(new BindingString("Kyocera M2035dn"));
                list.Add(new BindingString("Kyocera M2530dn"));
                list.Add(new BindingString("Kyocera M2535dn"));
                list.Add(new BindingString("HP MFP M225dn"));
                list.Add(new BindingString("HP LaserJet Pro M125rnw"));
                list.Add(new BindingString("HP LaserJet Pro M1212nf"));
                list.Add(new BindingString("Brother MFC-7860DW"));
                list.Add(new BindingString("Brother DCP-7060DR"));
                list.Add(new BindingString("Brother DCP-7040DR"));

                ModelMFU = new CollectionView(list);
            }
        }

        public MFU newMFU = new MFU();
        private void ComboBoxMFU_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as System.Windows.Controls.ComboBox).DataContext = this.newMFU;
        }

        /// <summary>
        /// Метод заполнения поля IP значением из выбранного хоста
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillIPFromSelectedIP(object sender, RoutedEventArgs e)
        {
            try
            {
                MFUIP_TextBox.Text = SelectedIP.IPAddress;
            }
            catch (Exception ex)
            {
                Log("Необходимо выбрать хост", true, true, ex.StackTrace);
            }
        }

        /// <summary>
        /// Метод формирования текста для письма, которое необходимо отправить Власенко
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateLetterText(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder mfuLetterBody = new StringBuilder();
                mfuLetterBody.AppendLine($"ЦВЗ {SelectedIP.Owner.NumberCVZ} {MFUReasonRepl_ComboBox.Text}");
                mfuLetterBody.AppendLine();
                mfuLetterBody.AppendLine("Добрый день.");
                mfuLetterBody.AppendLine();
                mfuLetterBody.AppendLine($"1. {SelectedIP.Owner.ToString()}");
                mfuLetterBody.AppendLine($"2. {MFUReasonRepl_ComboBox.Text}");
                mfuLetterBody.AppendLine($"Модель: {MFUModel_ComboBox.Text}");
                mfuLetterBody.AppendLine($"IP: {MFUIP_TextBox.Text}");
                mfuLetterBody.AppendLine($"MAC: ");

                CopyToClipboard(mfuLetterBody.ToString(), true, "Письмо скопировано в буфер обмена");
                Log("Письмо скопировано в буфер обмена", false, true, "Письмо сформировано, скопировано в буфер обмена и готово для отправки Власенко");
                //Clipboard.SetText(mfuLetterBody.ToString());
                //Bindings.StatusBarText = "Письмо скопировано в буфер обмена";
            }
            catch (NullReferenceException)
            {
                Log("Не выбран ЦВЗ", true, true, "Необходимо выбрать ЦВЗ для корректного формирования письма");
            }
            catch (Exception ex)
            {
                Log("Ошибка формирования текста письма. " + ex.Message, true, true, ex.StackTrace);
            }

        }


        /// <summary>
        /// Метод отправки сообщения Власенко (не доработано)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendLetterMFU(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder mfuLetterBody = new StringBuilder();
                //mfuLetterBody.AppendLine($"Замена МФУ на{SelectedIP.Owner.ToString()}");
                //mfuLetterBody.AppendLine();
                mfuLetterBody.AppendLine("Добрый день.");
                mfuLetterBody.AppendLine();
                mfuLetterBody.AppendLine(MFUReasonRepl_ComboBox.Text);
                mfuLetterBody.AppendLine($"Модель: {MFUModel_ComboBox.Text}");
                mfuLetterBody.AppendLine($"IP: {MFUIP_TextBox.Text}");

                string subject = $"Замена МФУ на{SelectedIP.Owner.ToString()}";
                Process.Start($"mailto:nvlasenko@dengisrazy.ru&subject={subject.Replace(" ", "%20")}&body={mfuLetterBody.ToString().Replace(" ", "%20")}.");
                //Process.Start("mailto:admin@ktonanovenkogo.ru%2C%20qwertydmitriy@gmail.com?subject=От%20уважаемого%20читателя&amp;body=Здравствуйте!%0D%0A%0D%0AВыражаю%20Вам%20свое%20фи!!!%0D%0AВы%20сильно%20пали%20в%20моих%20глазах!!!!");
                //letter = letter.Replace(" ", "%20");
                //letter = letter.Replace("\n", "%0A");
                //Process.Start(string.Format( letter));

                //var proc = new Process();
                //proc.StartInfo.FileName = string.Format("\"{0}\"", Process.GetProcessesByName("OUTLOOK")[0].Modules[0].FileName);
                //proc.StartInfo.Arguments = string.Format(" /c ipm.note /m {0} /a \"\"", "someone@somewhere.com");
                //proc.Start();
                //Clipboard.SetText(letter);
                Log("Письмо отправлено в почтовый клиент", false, true);
            }
            catch (Exception ex)
            {
                Log("Ошибка при отправке письма по замене МФУ Власенко. " + ex.Message, true, true, ex.StackTrace);
            }
        }

        #endregion
    }
}
