using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    /// <summary>
    /// Класс отображения меню
    /// </summary>
    public class MenuItemViewModel
    {
        /// <summary>
        /// Конструктор класса формирования меню элемента
        /// </summary>
        /// <param name="Header">Шапка элемента</param>
        /// <param name="isPassword">Признак отношения элементу к паролю</param>
        /// <param name="Tooltip">Подсказка для элемента</param>
        public MenuItemViewModel(string Header, bool isPassword = false, string Tooltip = "")
        {
            this.Header = Header;
            this.isPassword = isPassword;
            if (!string.IsNullOrWhiteSpace(Tooltip))
            {
                this.Tooltip = "Пароль для " + Tooltip;
            }
        }
        
        /// <summary>
        /// Свойство, представляющее шапку элемента
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Свойство, передставляющее признак отношения элемента к паролю
        /// </summary>
        public bool isPassword { get; set; }

        /// <summary>
        /// Свойство, представляющее коллекцию элементов меню
        /// </summary>
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        /// <summary>
        /// Свойство, представлящее подсказку для элемента
        /// </summary>
        public string Tooltip { get; set; }
    }
}
