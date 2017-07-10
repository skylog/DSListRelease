using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    /// <summary>
    /// Класс построителя строк
    /// </summary>
    public class BindingString
    {
        public BindingString(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}
