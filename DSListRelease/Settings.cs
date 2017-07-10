using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Serialization;

namespace DSList
{


    public class Settings
    {
        public event SettingsEvent OnSaveOrLoad;

        #region Поля
        /// <summary>
        /// Поля настроек
        /// </summary>
        SettingsFields _Fields = new SettingsFields();

        /// <summary>
        /// Расположение файла настроек
        /// </summary>
        string xmlFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DSList\Settings.xml");

        #endregion

        #region Свойства
        /// <summary>
        /// Свойство полей настроек
        /// </summary>
        public SettingsFields Fields
        {
            get { return _Fields; }
            set
            {
                _Fields = value;
            }
        }
        /// <summary>
        /// Свойство владельца настроек
        /// </summary>
        public NewMainWindow Owner { get; set; }

        #endregion

        #region Конструкторы
        /// <summary>
        /// Класс настроек
        /// </summary>
        public Settings()
        {
            this.LoadSettings();
        }
        /// <summary>
        /// Класс настроек
        /// </summary>
        /// <param name="owner">Владелец (класс, из под которого создается класс настроек)</param>
        public Settings(NewMainWindow owner)
        {
            this.LoadSettings();
            Owner = owner;
        }

        #endregion

        #region Методы
        /// <summary>
        /// Загрузка настроек из файла XMLFileName
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                if (File.Exists(this.xmlFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SettingsFields));
                    TextReader textReader = new StreamReader(this.xmlFileName);
                    this.Fields = serializer.Deserialize(textReader) as SettingsFields;
                    textReader.Close();
                    this.OnSaveOrLoad();
                }
                else
                {
                    this.SaveSettings(true);
                }
            }
            catch (Exception ex)
            {
                if (Owner != null)
                {
                    Owner.Log("Ошибка при загрузке настроек. " + ex.Message, true, true, ex.StackTrace);
                }

            }
        }
        /// <summary>
        /// Сохранение настроек в файл XMLFileName
        /// </summary>
        /// <param name="generate_event"></param>
        public void SaveSettings(bool generate_event = true)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(this.xmlFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(this.xmlFileName));
                }
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsFields));
                TextWriter textWriter = new StreamWriter(this.xmlFileName);
                serializer.Serialize(textWriter, this.Fields);
                textWriter.Close();
                if (generate_event)
                {
                    this.OnSaveOrLoad();
                }
            }
            catch (Exception ex)
            {
                if (Owner != null)
                {
                    Owner.Log("Ошибка при сохранении настроек. " + ex.Message, true, true, ex.StackTrace);
                }

            }

        }


        #endregion






    }
}
