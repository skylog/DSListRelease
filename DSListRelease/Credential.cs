using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSList
{
    /// <summary>
    /// Класс, представляющий удостоверение личности
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Свойство, представляющее шифрованный пароль
        /// </summary>
        public string CryptedPassword { get; set; }

        /// <summary>
        /// Свойство, представляющее признак пользовательского пароля
        /// </summary>
        public bool CustomPassword { get; set; }

        /// <summary>
        /// Свойство, представляющее тип хоста IPType
        /// </summary>
        public IPType HostType { get; set; }

        /// <summary>
        /// Свойство, представляющее логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Свойство,  устанавливающее и выводящее CryptedPassword шифруя и дешифруя его
        /// </summary>
        [XmlIgnore]
        public string Password
        {
            get
            {
                try
                {
                    // Получение пароля, дешифруя пользовательский пароль, применяя ключ шифрования текущего пользователя, выполнившего вход в систему или стандартного ключа шифрования
                    return Crypt.Decrypt(this.CryptedPassword, this.CustomPassword ? Environment.UserName.ToLower() : Access.sv_password);
                }
                catch
                {
                    //System.Windows.Forms.MessageBox.Show("Test");
                    return string.Empty;
                }
            }
            set
            {
                // Создание пароля, дешифруя пользовательский пароль, применяя ключ шифрования текущего пользователя, выполнившего вход в систему или стандартного ключа шифрования
                this.CryptedPassword = Crypt.Encrypt(value, this.CustomPassword ? Environment.UserName.ToLower() : Access.sv_password);
            }
        }

        public RegionEnum Region { get; set; }
    }
}
