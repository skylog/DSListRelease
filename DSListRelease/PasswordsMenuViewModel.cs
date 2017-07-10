using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    /// <summary>
    /// Класс, представляющий модель отображения паролей
    /// </summary>
    public class PasswordsMenuViewModel
    {
        /// <summary>
        /// Свойство, представляющее коллекцию элементов меню паролей
        /// </summary>
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }
    }
}
