using System.Security.Cryptography;
using System.Text;
using System;
using System.IO;

namespace Zyh.Common.Security
{
    using Org.BouncyCastle.Asn1.Pkcs;
    using Org.BouncyCastle.Asn1.X509;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.X509;
    using System.Linq;

    public class RsaHelper
    {
        /// <summary>
        /// 去除RSA密钥的头部和尾部,通过头部尾部的换行符剔除,去除第一行和最后两行
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKeyFromString(string key)
        {
            var strippedKeys = key.Split('\n');
            if (strippedKeys.Length > 3)
            {
                strippedKeys = strippedKeys.Skip(1).ToArray();
                strippedKeys = strippedKeys.Take(strippedKeys.Length - 1).ToArray();
                strippedKeys = strippedKeys.Take(strippedKeys.Length - 1).ToArray();
                string result = string.Join(string.Empty, strippedKeys);
                return result.Replace("\r", "").Replace("\n", "");
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// base64 private key string -> xml private key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public string ToXmlPrivateKey(string privateKey)
        {
            //var str = Convert.FromBase64String(privateKey);
            //var key = PrivateKeyFactory.CreateKey(str);

            RsaPrivateCrtKeyParameters privateKeyParams =
                PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)) as RsaPrivateCrtKeyParameters;
            using (var rsa = RSA.Create())
            {
                RSAParameters rsaParams = new RSAParameters()
                {
                    Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                    Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),
                    D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                    DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                    DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                    P = privateKeyParams.P.ToByteArrayUnsigned(),
                    Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                    InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(true);
            }
        }

        /// <summary>
        /// base64 public key string -> xml public key
        /// </summary>
        /// <param name="pubilcKey"></param>
        /// <returns></returns>
        public string ToXmlPublicKey(string pubilcKey)
        {
            RsaKeyParameters p =
                PublicKeyFactory.CreateKey(Convert.FromBase64String(pubilcKey)) as RsaKeyParameters;
            using (var rsa = RSA.Create())
            {
                RSAParameters rsaParams = new RSAParameters
                {
                    Modulus = p.Modulus.ToByteArrayUnsigned(),
                    Exponent = p.Exponent.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(false);
            }
        }

        /// <summary>
        /// xml private key -> base64 private key string
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <returns></returns>
        public string FromXmlPrivateKey(string xmlPrivateKey)
        {
            string result = string.Empty;
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(xmlPrivateKey);
                RSAParameters param = rsa.ExportParameters(true);
                RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(
                    new Org.BouncyCastle.Math.BigInteger(1, param.Modulus), new Org.BouncyCastle.Math.BigInteger(1, param.Exponent),
                    new Org.BouncyCastle.Math.BigInteger(1, param.D), new Org.BouncyCastle.Math.BigInteger(1, param.P),
                    new Org.BouncyCastle.Math.BigInteger(1, param.Q), new Org.BouncyCastle.Math.BigInteger(1, param.DP),
                    new Org.BouncyCastle.Math.BigInteger(1, param.DQ), new Org.BouncyCastle.Math.BigInteger(1, param.InverseQ));
                PrivateKeyInfo privateKey = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);

                result = Convert.ToBase64String(privateKey.ToAsn1Object().GetEncoded());
            }
            return result;
        }

        /// <summary>
        /// xml public key -> base64 public key string
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <returns></returns>
        public string FromXmlPublicKey(string xmlPublicKey)
        {
            string result = string.Empty;
            using (var rsa = RSA.Create())
            {
                rsa.FromXmlString(xmlPublicKey);
                RSAParameters p = rsa.ExportParameters(false);
                RsaKeyParameters keyParams = new RsaKeyParameters(
                    false, new Org.BouncyCastle.Math.BigInteger(1, p.Modulus), new Org.BouncyCastle.Math.BigInteger(1, p.Exponent));
                SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyParams);
                result = Convert.ToBase64String(publicKeyInfo.ToAsn1Object().GetEncoded());
            }
            return result;
        }


        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="content">被加密信息</param>
        /// <returns></returns>
        public byte[] RSAEncrypt(string xmlPublicKey, byte[] content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            byte[] originalData = content;
            if (originalData == null || originalData.Length <= 0)
            {
                throw new NotSupportedException();
            }
            if (rsa == null)
            {
                throw new ArgumentNullException();
            }
            byte[] encryContent = null;
            #region 分段加密
            int bufferSize = (rsa.KeySize / 8) - 11;
            byte[] buffer = new byte[bufferSize];
            //分段加密
            using (MemoryStream input = new MemoryStream(originalData))
            using (MemoryStream ouput = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, bufferSize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] encrypt = rsa.Encrypt(temp, false);
                    ouput.Write(encrypt, 0, encrypt.Length);
                }
                encryContent = ouput.ToArray();
            }
            #endregion
            return encryContent;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="content">被解密信息</param>
        /// <returns></returns>
        public byte[] RSADecrypt(string xmlPrivateKey, byte[] content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            byte[] encryptData = content;
            //byte[] dencryContent = rsa.Decrypt(encryptData, false);
            byte[] dencryContent = null;
            #region 分段解密
            if (encryptData == null || encryptData.Length <= 0)
            {
                throw new NotSupportedException();
            }

            int keySize = rsa.KeySize / 8;
            byte[] buffer = new byte[keySize];

            using (MemoryStream input = new MemoryStream(encryptData))
            using (MemoryStream output = new MemoryStream())
            {
                while (true)
                {
                    int readLine = input.Read(buffer, 0, keySize);
                    if (readLine <= 0)
                    {
                        break;
                    }
                    byte[] temp = new byte[readLine];
                    Array.Copy(buffer, 0, temp, 0, readLine);
                    byte[] decrypt = rsa.Decrypt(temp, false);
                    output.Write(decrypt, 0, decrypt.Length);
                }
                dencryContent = output.ToArray();
            }
            #endregion
            return dencryContent;
        }

        /// <summary>
        /// RSA标注
        /// </summary>
        /// <param name="data"></param>
        /// <param name="xmlPublicKey"></param>
        /// <returns></returns>
        public string RSASignData(string data, string xmlPrivateKey)
        {
            //using (var rsa = RSA.Create())
            //{
            //    rsa.FromXmlString(xmlPrivateKey);

            //    var dataToSign = Encoding.UTF8.GetBytes(data);
            //    var signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            //    return Convert.ToBase64String(signature);
            //}

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);

                var dataToSign = Encoding.UTF8.GetBytes(data);
                var signature = rsa.SignData(dataToSign, "SHA512");
                return Convert.ToBase64String(signature);
            }
        }

        /// <summary>
        /// RSA校验
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <param name="publicKeyJson"></param>
        /// <returns></returns>
        public bool RSAVerifySignature(string data, string signature, string xmlPublicKey)
        {
            //using (var rsa = RSA.Create())
            //{
            //    rsa.FromXmlString(xmlPublicKey);

            //    var dataToVerify = Encoding.UTF8.GetBytes(data);
            //    var signatureData = Convert.FromBase64String(signature);
            //    return rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            //}

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);

                var dataToVerify = Encoding.UTF8.GetBytes(data);
                var signatureData = Convert.FromBase64String(signature);
                return rsa.VerifyData(dataToVerify, "SHA512", signatureData);
            }
        }
    }
}
