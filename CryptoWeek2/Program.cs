using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using XMLib;

namespace CryptoWeek2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CbcRoutine();
                CtrRoutine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception - {0}",ex.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        static private void CtrRoutine()
        {
            var ctrCiphertexts = new[]
                {
                    "69dda8455c7dd4254bf353b773304eec0ec7702330098ce7f7520d1cbbb20fc388d1b0adb5054dbd7370849dbf0b88d393f252e764f1f5f7ad97ef79d59ce29f5f51eeca32eabedd9afa9329",
                    "770b80259ec33beb2561358a9f2dc617e46218c0a53cbeca695ae45faa8952aa0e311bde9d4e01726d3184c34451"
                };

            var ctrKeys = new[]
                {
                    "36f18357be4dbd77f050515c73fcf9f2",
                    "36f18357be4dbd77f050515c73fcf9f2"
                };


            for (int i = 0; i < ctrCiphertexts.Length; i++)
            {
                var ctWithIv = StringUtils.FromHex(ctrCiphertexts[i]);
                var key = StringUtils.FromHex(ctrKeys[i]);

                var ct = ctWithIv.Skip(16).ToArray();
                var iv = ctWithIv.Take(16).ToArray();

                var aes = new Aes128CounterMode(iv);
                var output = new byte[ct.Length];
                aes.CreateDecryptor(key,null).TransformBlock(ct, 0, ct.Length, output, 0);

                var msg = Encoding.ASCII.GetString(output);
                Console.WriteLine("CT{0} = '{1}'", i, msg);
            }
        }

        private static void CbcRoutine()
        {
            var cbcCiphertexts = new[]
                {
                    "4ca00ff4c898d61e1edbf1800618fb2828a226d160dad07883d04e008a7897ee2e4b7465d5290d0c0e6c6822236e1daafb94ffe0c5da05d9476be028ad7c1d81",
                    "5b68629feb8606f9a6667670b75b38a5b4832d0f26e1ab7da33249de7d4afc48e713ac646ace36e872ad5fb8a512428a6e21364b0c374df45503473c5242a253"
                };

            var cbcKeys = new[]
                {
                    "140b41b22a29beb4061bda66b6747e14",
                    "140b41b22a29beb4061bda66b6747e14"
                };


            for (int i = 0; i < cbcCiphertexts.Length; i++)
            {
                var ctWithIv = StringUtils.FromHex(cbcCiphertexts[i]);
                var key = StringUtils.FromHex(cbcKeys[i]);

                var ct = ctWithIv.Skip(16).ToArray();
                var iv = ctWithIv.Take(16).ToArray();

                var aes = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };

                var output = new byte[ct.Length];
                var decryptor = aes.CreateDecryptor(key, iv);
               
                output = decryptor.TransformFinalBlock(ct, 0, ct.Length);

                var msg = Encoding.ASCII.GetString(output);
                Console.WriteLine("CT{0} = '{1}'", i, msg);
            }
        }


        

       
    }
}
