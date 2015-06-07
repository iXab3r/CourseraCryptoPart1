using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using XMLib;
using XMLib.Crypto;

namespace CryptoWeek3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var y = "11111111111111111111111111111111";
                var res = "66e94bd4ef8a2c3b884cfa59ca342b2e";
                var resXorY = CryptoUtils.XorHexStrings(res,y);
                Console.WriteLine(resXorY);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private void Question3()
        {
            try
            {


                var msg = new byte[16];
                var iv = new byte[16];

                new Random(1).NextBytes(iv);
                new Random(2).NextBytes(msg);

                var badHashGen = new BadHash(iv);
                var msgHash = badHashGen.GetHash(msg);
                Console.WriteLine("M : {0}", StringUtils.ToHex(msg));
                Console.WriteLine("IV: {0}", StringUtils.ToHex(iv));
                Console.WriteLine("HF: {0}", StringUtils.ToHex(msgHash));
                Console.WriteLine("IV XOR HF: {0}", StringUtils.ToHex(CryptoUtils.XorArrays(iv, msgHash)));

                Console.WriteLine();

                var oneArray = Enumerable.Repeat(1, 16).Select(x => (byte)x).ToArray();
                var zeroArray = Enumerable.Repeat(0, 16).Select(x => (byte)x).ToArray();
                Console.WriteLine("ECBC(iv, 0n) = {0}", StringUtils.ToHex(new BadHash(iv).GetHash(zeroArray)));
                Console.WriteLine();

                Console.WriteLine("ECBC(iv xor hash, 0n) = {0}", StringUtils.ToHex(new BadHash(CryptoUtils.XorArrays(iv, msgHash)).GetHash(zeroArray)));
                Console.WriteLine("ECBC(iv xor 1n, m xor 1n) = {0}", StringUtils.ToHex(new BadHash(CryptoUtils.XorArrays(iv, oneArray)).GetHash(CryptoUtils.XorArrays(msg, oneArray))));
                Console.WriteLine("ECBC(hash xor m, 0n) = {0}", StringUtils.ToHex(new BadHash(CryptoUtils.XorArrays(msgHash, msg)).GetHash(zeroArray)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadKey();
            }
        }

    }

    internal class BadHash
    {
        private readonly byte[] m_iv;

        private byte[] m_key1Bits = new byte[16];
        private byte[] m_key2Bits = new byte[16];

        private ICryptoTransform m_mainTransform;

        private ICryptoTransform m_lastTransfrom;

        public BadHash(byte[] _iv)
        {
            m_iv = _iv ?? new byte[16];
            var rng = new Random(0);
            rng.NextBytes(m_key1Bits);
            rng.NextBytes(m_key2Bits);

            m_mainTransform = new RijndaelManaged()
            {
                IV = m_iv,
                Key = m_key1Bits,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            }.CreateEncryptor();

            m_lastTransfrom  = new RijndaelManaged()
            {
                IV = m_iv,
                Key = m_key2Bits,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            }.CreateEncryptor();
        }

        public byte[] GetHash(byte[] _msg)
        {
            var mainBlock = m_mainTransform.TransformFinalBlock(_msg, 0, _msg.Length);
            return mainBlock;
            //var hash = m_lastTransfrom.TransformFinalBlock(mainBlock, 0, mainBlock.Length);
           // return hash;
        }

        public static bool IsValid(byte[] _iv, byte[] _msg, byte[] _tag)
        {
            var hashGen = new BadHash(_iv);
            var hash = hashGen.GetHash(_msg);
            return _tag.SequenceEqual(hash);
        }
    }
}
