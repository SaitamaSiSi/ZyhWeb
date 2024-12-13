//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/12 16:59:19</date>
//------------------------------------------------------------------------------


namespace Zyh.Common.Security
{
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Security;

    public class RsaHelperExtension : RsaHelper
    {
        public static byte[] ExampleDefaultSecureRandom()
        {
            SecureRandom defaultSecureRandom = new SecureRandom();
            byte[] bytes = new byte[32];
            defaultSecureRandom.NextBytes(bytes);
            return bytes;
        }

        public static ICipherParameters KeyParameterGeneration(int keySize)
        {
            CipherKeyGenerator keyGen = new CipherKeyGenerator();
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(), keySize));
            KeyParameter keyParam = keyGen.GenerateKeyParameter();
            return keyParam;
        }
    }
}
