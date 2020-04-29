using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace bm.common
{
    public static class Utils
    {
        public static string Md5(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            using (var m = MD5.Create())
            {
                var bts = m.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(bts).ToLower().Replace("-", "");
            }
        }

        public static string Sha256(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            using (var m = SHA256.Create())
            {
                var bts = m.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(bts).ToLower().Replace("-", "");
            }
        }

        /// <summary>
        /// aes 加密
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static string AesEncrypt(string text, string password)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(text), "原文和密码都不能为空");
            var aes_key = Md5(password).Substring(8, 16);
            var aes_iv = Md5(aes_key).Substring(8, 16);

            using (var aes = Rijndael.Create())
            {
                var bits = Encoding.UTF8.GetBytes(text);
                aes.Key = Encoding.UTF8.GetBytes(aes_key);
                aes.IV = Encoding.UTF8.GetBytes(aes_iv);
                byte[] cipherBytes = null;
                using (var ms = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(bits, 0, bits.Length);
                            cs.FlushFinalBlock();
                            //得到加密后的字节数组
                            cipherBytes = ms.ToArray();
                        }
                    }
                }
                return Convert.ToBase64String(cipherBytes);
            }
        }

        /// <summary>
        /// aes 解密
        /// </summary>
        /// <param name="text">密文</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static string AesDecrypt(string text, string password)
        {
            var cipherText = Convert.FromBase64String(text);
            var aes_key = Md5(password).Substring(8, 16);
            var aes_iv = Md5(aes_key).Substring(8, 16);
            using (var aes = Rijndael.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(aes_key);
                aes.IV = Encoding.UTF8.GetBytes(aes_iv);
                using (var ms = new MemoryStream(cipherText))
                {
                    using (var decryptor = aes.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                var decrypt = sr.ReadToEnd();
                                return decrypt;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加密敏感数据
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string EncryptSensitive(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            var aes_key = "112177cc5f584b9fa6e989ce9ca0425b";
            using (var aes = Rijndael.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(aes_key);
                aes.IV = new byte[] { 127, 10, 45, 150, 148, 165, 194, 123, 52, 10, 0, 139, 243, 171, 21, 253 };
                using (var ms = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            var byts = Encoding.UTF8.GetBytes(text);
                            cs.Write(byts, 0, byts.Length);
                            cs.FlushFinalBlock();
                            var cipherBytes = ms.ToArray();
                            return Convert.ToBase64String(cipherBytes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解密敏感数据
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string DecryptSensitive(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            var cipherText = Convert.FromBase64String(text);
            var aes_key = "112177cc5f584b9fa6e989ce9ca0425b";
            using (var aes = Rijndael.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(aes_key);
                aes.IV = new byte[] { 127, 10, 45, 150, 148, 165, 194, 123, 52, 10, 0, 139, 243, 171, 21, 253 };
                using (var ms = new MemoryStream(cipherText))
                {
                    using (var decryptor = aes.CreateDecryptor())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                var decrypt = sr.ReadToEnd();
                                return decrypt;
                            }
                        }
                    }
                }
            }
        }
    }
}
