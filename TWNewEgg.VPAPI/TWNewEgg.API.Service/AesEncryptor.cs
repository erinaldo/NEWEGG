using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// AES Encrypt/Decrypt
    /// </summary>
    public class AesEncryptor
    {
        static string key = System.Configuration.ConfigurationManager.AppSettings["AesKey"];

        /// <summary>
        /// Encrypt
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AesEncrypt(string source)
        {
            byte[] bkey = new byte[32];
            byte[] iv = new byte[16];
            iv = Encoding.Unicode.GetBytes("ggEwenwT");
            byte[] shaBuffer = new SHA384Managed().ComputeHash(Encoding.UTF8.GetBytes(key));
            RijndaelManaged aes = new RijndaelManaged();

            Array.Copy(shaBuffer, 0, bkey, 0, 32);
            Array.Copy(shaBuffer, 32, iv, 0, 16);

            ICryptoTransform ict = aes.CreateEncryptor(bkey, iv);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write);
            byte[] rawData = UTF8Encoding.UTF8.GetBytes(source);

            cs.Write(rawData, 0, rawData.Length);
            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AesDecrypt(string source)
        {
            byte[] bkey = new byte[32];
            byte[] iv = new byte[16];
            iv = Encoding.Unicode.GetBytes("ggEwenwT");
            byte[] shaBuffer = new SHA384Managed().ComputeHash(Encoding.UTF8.GetBytes(key));
            RijndaelManaged aes = new RijndaelManaged();
            Array.Copy(shaBuffer, 0, bkey, 0, 32);
            Array.Copy(shaBuffer, 32, iv, 0, 16);
            ICryptoTransform ict = aes.CreateDecryptor(bkey, iv);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write);
            try
            {
                byte[] rawData = Convert.FromBase64String(source);
                cs.Write(rawData, 0, rawData.Length); 
                cs.Close();
                return UTF8Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return "Decrypt Fail.";
            }

        }
    }
}
