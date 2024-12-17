//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/12 16:25:22</date>
//------------------------------------------------------------------------------

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;


namespace Zyh.Common.Security
{
    public class HashHelper
    {
        private static readonly byte[] RootKey = {
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,

            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,

            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,

            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
            0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
        };

        public static byte[] DoComputeHMACSHA512(byte[] sourceData)
        {
            using (HMACSHA512 hmac = new HMACSHA512(RootKey))
            {
                return hmac.ComputeHash(sourceData);
            }
        }

        public static bool CompareHash(byte[] sourceHash, byte[] targetHash)
        {
            if (sourceHash == null || targetHash == null || sourceHash.Length != targetHash.Length)
            {
                return false;
            }
            for (int i = 0; i < sourceHash.Length; i++)
            {
                if (targetHash[i] != sourceHash[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashAlgorithm">BLAKE2b ,BLAKE3</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string DoCompute(string hashAlgorithm, string filePath)
        {
            var hashValue = string.Empty;
            switch (hashAlgorithm)
            {
                case "SHA512":
                    hashValue = SHA512File(filePath);
                    break;
                default:
                    hashValue = string.Empty;
                    break;
            }
            return hashValue;
        }

        public static bool CompareHash(string Hash1, string Hash2)
        {
            if (Hash1 == Hash2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string SHA512File(string fileName)
        {
            return HashFile(fileName, "sha512");
        }

        /// <summary>
        /// 计算文件的哈希值
        /// </summary>
        /// <param name="fileName">要计算哈希值的文件名和路径</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值16进制字符串</returns>
        private static string HashFile(string fileName, string algName)
        {
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] hashBytes = HashData(fs, algName);
            fs.Close();
            return ByteArrayToHexString(hashBytes);
        }

        /// <summary>
        /// 计算哈希值
        /// </summary>
        /// <param name="stream">要计算哈希值的 Stream</param>
        /// <param name="algName">算法:sha1,md5</param>
        /// <returns>哈希值字节数组</returns>
        private static byte[] HashData(Stream stream, string algName)
        {
            System.Security.Cryptography.HashAlgorithm algorithm;
            if (algName == null)
            {
                throw new ArgumentNullException("algName 不能为 null");
            }

            if (string.Compare(algName, "sha1", true) == 0)
            {
                algorithm = System.Security.Cryptography.SHA1.Create();
            }
            else if (string.Compare(algName, "sha512", true) == 0)
            {
                algorithm = System.Security.Cryptography.SHA512.Create();
            }
            else
            {
                if (string.Compare(algName, "md5", true) != 0)
                {
                    throw new Exception("algName 只能使用 sha1,sha512 或 md5");
                }
                algorithm = System.Security.Cryptography.MD5.Create();
            }

            return algorithm.ComputeHash(stream);
        }

        /// <summary>
        /// 字节数组转换为16进制表示的字符串
        /// </summary>
        private static string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }

        public static string DoCompute(string hashAlgorithm, FileStream fs)
        {
            var hashValue = string.Empty;
            switch (hashAlgorithm)
            {
                case "BLAKE2b":
                    hashValue = ComputeBlake2b(fs);
                    break;
                case "BLAKE3":
                    //hashValue = ComputeBlake3(fs);
                    break;
                default:
                    hashValue = string.Empty;
                    break;
            }
            return hashValue;
        }

        private static string ComputeBlake2b(string filepath)
        {
            try
            {
                using (var data = File.OpenRead(filepath))
                {
                    var hasher = Blake2Fast.Blake2b.CreateIncrementalHasher();
                    var buffer = ArrayPool<byte>.Shared.Rent(4096);

                    int bytesRead;
                    while ((bytesRead = data.Read(buffer, 0, buffer.Length)) > 0)
                        hasher.Update(buffer.AsSpan(0, bytesRead));

                    ArrayPool<byte>.Shared.Return(buffer);
                    return Convert.ToBase64String(hasher.Finish());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "Error");
                return string.Empty;
            }
        }
        private static string ComputeBlake2b(FileStream fs)
        {
            try
            {
                var hasher = Blake2Fast.Blake2b.CreateIncrementalHasher();
                var buffer = ArrayPool<byte>.Shared.Rent(4096);

                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    hasher.Update(buffer.AsSpan(0, bytesRead));

                ArrayPool<byte>.Shared.Return(buffer);
                return Convert.ToBase64String(hasher.Finish());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, "Error");
                return string.Empty;
            }
        }

        //private static string ComputeBlake3(string filepath)
        //{
        //    try
        //    {

        //        using (var data = File.OpenRead(filepath))
        //        {
        //            var blake3 = new BLAKE3();
        //            var buffer = ArrayPool<byte>.Shared.Rent(4096);

        //            int bytesRead;
        //            while ((bytesRead = data.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                var bytes = buffer.AsSpan(0, bytesRead).ToArray();
        //                blake3.HashBytes(bytes);
        //            }

        //            ArrayPool<byte>.Shared.Return(buffer);
        //            var hashValue = blake3.HashFinal();
        //            return Convert.ToBase64String(hashValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(ex, "Error");
        //        return string.Empty;

        //    }
        //}
        //private static string ComputeBlake3(FileStream fs)
        //{
        //    try
        //    {
        //        var blake3 = new BLAKE3();
        //        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        //        int bytesRead;
        //        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            var bytes = buffer.AsSpan(0, bytesRead).ToArray();
        //            blake3.HashBytes(bytes);
        //        }

        //        ArrayPool<byte>.Shared.Return(buffer);

        //        var hashValue = blake3.HashFinal();
        //        return Convert.ToBase64String(hashValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(ex, "Error");
        //        return string.Empty;

        //    }
        //}


        public static string GetMd5(FileStream stream)
        {
            var algorithm = MD5.Create();
            byte[] hashBytes = algorithm.ComputeHash(stream);
            return ByteArrayToHexString(hashBytes);
        }

    }

    public class IncrementalMD5
    {
        private MD5 md5;
        private MemoryStream memoryStream;

        public IncrementalMD5()
        {
            md5 = MD5.Create();
            memoryStream = new MemoryStream();
        }

        public void Update(byte[] data)
        {
            memoryStream.Write(data, 0, data.Length);
        }

        public string Hex()
        {
            memoryStream.Position = 0;
            byte[] hashBytes = md5.ComputeHash(memoryStream);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            memoryStream.SetLength(0); // 清空内存流
            return sb.ToString();
        }
    }

    public class MD5FileStreamProcessor
    {
        private const int BufferSize = 1024 * 64; // 定义缓冲区大小
        private IncrementalMD5 incrementalHash;

        public MD5FileStreamProcessor()
        {
            incrementalHash = new IncrementalMD5();
        }

        public string CalculateMD5Hash(FileStream fileStream)
        {
            byte[] buffer = new byte[BufferSize];
            int bytesRead;
            // 循环读取数据块，直到到达文件末尾
            while ((bytesRead = fileStream.Read(buffer, 0, BufferSize)) > 0)
            {
                byte[] upBytes = new byte[bytesRead];
                Array.Copy(buffer, 0, upBytes, 0, bytesRead);
                incrementalHash.Update(upBytes);
            }
            // 返回最终的哈希值
            return incrementalHash.Hex();
        }
    }
}
