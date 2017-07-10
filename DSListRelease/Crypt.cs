using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DSList
{
    /// <summary>
    /// Класс шифрования
    /// </summary>
    public class Crypt
    {
        /// <summary>
        /// Метод расшифровки
        /// </summary>
        /// <param name="data">Данные в байтовом массиве, которые необходимо расшифровать</param>
        /// <param name="password">Ключ шифрования в string формате</param>
        /// <returns>Расшифрованный массив байт</returns>
        private static byte[] Decrypt(byte[] data, string password)
        {
            //using (BinaryReader reader = new BinaryReader(InternalDecrypt(data, password)))
            //{
            //    return reader.ReadBytes((int)reader.BaseStream.Length);
            //}
            BinaryReader reader = new BinaryReader(InternalDecrypt(data, password));

            return reader.ReadBytes((int)reader.BaseStream.Length);
        }

        /// <summary>
        /// Метод расшифровки
        /// </summary>
        /// <param name="data">Данные в строковом формате, которые необходимо расшифровать</param>
        /// <param name="password">Ключ шифрования в string формате</param>
        /// <returns>Расшифрованный массив байт</returns>
        public static string Decrypt(string data, string password)
        {
            //using (StreamReader reader = new StreamReader(InternalDecrypt(Convert.FromBase64String(data), password)))
            //{
            //    return reader.ReadToEnd();
            //}
            StreamReader reader = new StreamReader(InternalDecrypt(Convert.FromBase64String(data), password));
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Метод шифрования
        /// </summary>
        /// <param name="data">Массив байт, который необходимо зашифровать</param>
        /// <param name="password">Ключ шифрования в string формате</param>
        /// <returns>Расшифрованную строку</returns>
        private static byte[] Encrypt(byte[] data, string password)
        {
            ICryptoTransform transform = Rijndael.Create().CreateEncryptor(new PasswordDeriveBytes(password, null).GetBytes(0x10), new byte[0x10]);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(data, 0, data.Length);
            stream2.FlushFinalBlock();
            return stream.ToArray();
        }

        /// <summary>
        /// Метод шифрования
        /// </summary>
        /// <param name="data">Данные, которые необходимо зашифровать в string формате</param>
        /// <param name="password">Ключ шифрования в string формате</param>
        /// <returns>Шифрованную строку</returns>
        public static string Encrypt(string data, string password) =>
            Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), password));

        /// <summary>
        /// Метод, который переводит заданную строку, представляющую двоичные данные в виде цифр в кодировке
        /// Base64, в эквивалентный массив 8-разрядных целых чисел без знака и дальше в String формат
        /// </summary>
        /// <param name="input">Входные строковые данные</param>
        /// <returns>Конвертированную строку</returns>
        public static string FromBase64(string input)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(input));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Метод формирования крипто-потока
        /// </summary>
        /// <param name="data">Входные данные в виде массива byte</param>
        /// <param name="password">Ключ шифрования в string формате</param>
        /// <returns></returns>
        private static CryptoStream InternalDecrypt(byte[] data, string password)
        {
            //using (ICryptoTransform transform = Rijndael.Create().CreateDecryptor(new PasswordDeriveBytes(password, null).GetBytes(0x10), new byte[0x10]))
            //{
            //    return new CryptoStream(new MemoryStream(data), transform, CryptoStreamMode.Read);
            //}
            ICryptoTransform transform = Rijndael.Create().CreateDecryptor(new PasswordDeriveBytes(password, null).GetBytes(0x10), new byte[0x10]);
            return new CryptoStream(new MemoryStream(data), transform, CryptoStreamMode.Read);
        }

        /// <summary>
        /// Метод преобразования string формата в Base64
        /// </summary>
        /// <param name="input">Входные данные в string формате</param>
        /// <returns>Выходные данные в Base64String формате</returns>
        public static string ToBase64(string input)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
