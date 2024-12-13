//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/12 16:21:09</date>
//------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Zyh.Common.Security
{
    public class AesHelper
    {
        private static readonly byte[] RootKey = {
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
        };

        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] ciphertext, byte[] key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key == null ? RootKey : key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
            return resultArray;
        }

        public static byte[] Decrypt(byte[] ciphertext)
        {
            return Decrypt(ciphertext, RootKey);
        }

        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] plaintext, byte[] key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key == null ? RootKey : key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);
            return resultArray;
        }

        public static byte[] Encrypt(byte[] plaintext)
        {
            return Encrypt(plaintext, RootKey);
        }
    }
}
