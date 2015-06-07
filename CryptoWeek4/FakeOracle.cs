using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

using XMLib;

namespace CryptoWeek4
{
    class FakeOracle : IPaddingOracle
    {
        private byte[] m_ivBytes = StringUtils.FromHex("f20bdba6ff29eed7b046d1df9fb70000");

        private byte[] m_key = StringUtils.FromHex("bdf302936266926ff37dbf7035d5eeb4");

        private byte[] m_messageBytes = Encoding.ASCII.GetBytes("This is some message that takes 48 bytes");

        private RijndaelManaged m_crypto = new RijndaelManaged();

        public FakeOracle()
        {
            m_crypto.Mode = CipherMode.CBC;
            m_crypto.Padding = PaddingMode.None;
        }

        public byte[] GetCipherText()
        {
            m_crypto.Padding = PaddingMode.PKCS7;
            using (var encryptor = m_crypto.CreateEncryptor(m_key,m_ivBytes))
            {
                return m_ivBytes.Concat(encryptor.TransformFinalBlock(m_messageBytes, 0, m_messageBytes.Length)).ToArray();
            }

        }

        public HttpStatusCode QueryOracle(byte[] _cipherTextBytes)
        {
            m_crypto.Padding = PaddingMode.None;
            var ivBytes = _cipherTextBytes.Take(16).ToArray();

            var cipherText = _cipherTextBytes.Skip(16).ToArray();
            using (var decryptor = m_crypto.CreateDecryptor(m_key, ivBytes))
            {
                try
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

                    if (m_crypto.Padding == PaddingMode.None)
                    {
                        var padding = decryptedBytes[decryptedBytes.Length - 1];
                        if (padding == 0)
                        {
                            throw new CryptographicException();
                        }

                        if (padding > decryptedBytes.Length)
                        {
                            throw new CryptographicException();
                        }
                        var paddingBytes = decryptedBytes.Skip(decryptedBytes.Length - padding);
                        if (!paddingBytes.All(x => x == padding))
                        {
                            throw new CryptographicException();
                        }

                        decryptedBytes = decryptedBytes.Take(decryptedBytes.Length - padding).ToArray();
                    }

                    if (decryptedBytes.Length != m_messageBytes.Length)
                    {
                        return HttpStatusCode.NotFound;
                    }
                }
                catch (CryptographicException ex)
                {
                    return HttpStatusCode.Forbidden;
                }
             
            }
            return HttpStatusCode.OK;
        }
    }
}