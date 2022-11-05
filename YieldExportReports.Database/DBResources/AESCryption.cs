using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YieldExportReports.Database.DBResources
{
    public class AESCryption
    {
        private const string AES_IV = @"pf69DL6GrWFyZcMK";
        private const string AES_Key = @"9Fix4L4HB4PKeKWY";

        /// <summary>
        /// 対称鍵暗号を使って文字列を暗号化する
        /// </summary>
        /// <param name="textSource">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string textSource)
        {
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(AES_IV);
                aes.Key = Encoding.UTF8.GetBytes(AES_Key);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                byte[] encrypted;
                using (var mStream = new MemoryStream())
                {
                    using (var ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(ctStream))
                        {
                            sw.Write(textSource);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return (Convert.ToBase64String(encrypted));
            }
        }

        /// <summary>
        /// 対称鍵暗号を使って暗号文を復号する
        /// </summary>
        /// <param name="textSource">暗号化された文字列</param>
        /// <returns>復号された文字列</returns>
        public static string Decrypt(string textSource)
        {
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.IV = Encoding.UTF8.GetBytes(AES_IV);
                aes.Key = Encoding.UTF8.GetBytes(AES_Key);

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                var plain = string.Empty;
                using (MemoryStream mStream = new MemoryStream(Convert.FromBase64String(textSource)))
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(ctStream))
                        {
                            plain = sr.ReadLine();
                        }
                    }
                }

                return plain ?? string.Empty;
            }
        }
    }
}
