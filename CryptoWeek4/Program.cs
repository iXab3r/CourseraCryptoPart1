using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using XMLib;

namespace CryptoWeek4
{
    class Program
    {
        private static byte[] m_orderedTable = new byte[]
        {
             32, 101, 116,  97, 111, 110, 105, 115, 114, 104, 108, 100, 117,  99, 121, 109, 
            119, 103, 102, 112,  46,  98,  10, 118, 107,  44,  13,  73,  39,  45,  84,  83, 
             65,  47,  67,  77,  87,  49,  66, 120,  34,  50,  48,  41,  80, 106,  72,  40, 
             33,  79,  68,  76,  58,  78,  63,  82,  70,  69,  71, 122,  89,  51, 113,  38, 
             56,  59,  53,  86,  52,  74,  62,  35,  55,  85,  75,  60,  54,  95,  57,  36, 
             61,  90,  37,  42,  81,  88,  43,  91,  93,  64,   9, 126,  96,  94, 125, 123, 
            124,  92,  16,   1,  20,   0,  11,  12, 127, 128, 129, 130, 131, 132, 133, 134, 
            135, 136, 137, 138, 139, 140, 141,  14, 142, 143, 144, 145, 146, 147, 148, 149, 
            150, 151,  15, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 
            165, 166, 167, 168, 169, 170, 171,  17, 172, 173, 174, 175, 176, 177, 178, 179, 
            180, 181,  18, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191,  19, 192, 193, 
            194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 
            210, 211,  21, 212, 213, 214, 215, 216, 217, 218, 219,   2, 220, 221,  22, 222, 
            223, 224, 225, 226, 227, 228, 229, 230, 231,  23, 232, 233, 234, 235, 236, 237, 
            238, 239, 240, 241,  24, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,  25, 
            26,  27,  28,  29,  30,  31,   3,   4,   5,   6,   7,   8,
            252, 253, 254, 255,  
        };

        static void Main(string[] args)
        {
            try
            {
                var oracle = new WebOracle();

                var cipherTextBytes = oracle.GetCipherText();

                Console.WriteLine("CT({1}b) - {0}", StringUtils.ToHex(cipherTextBytes, ""), cipherTextBytes.Length);

                var ivBytes = cipherTextBytes.Take(16).ToArray();
                Console.WriteLine("IV({1}b) - {0}", StringUtils.ToHex(ivBytes, ""), ivBytes.Length);

                var blocks = Enumerable.Range(0, cipherTextBytes.Length / 16).Select(x => cipherTextBytes.Skip(x * 16).Take(16).ToArray()).ToArray();
                foreach (var block in blocks)
                {
                    Console.WriteLine("Block({1}b) - {0}", StringUtils.ToHex(block, ""), block.Length);
                }

                Console.WriteLine();

                for (var blockIndex = 0; blockIndex < blocks.Length-1; blockIndex++)
                {
                    var blockToChange = blocks[blockIndex];
                    var blockToDecode = blocks[blockIndex + 1];

                    Console.WriteLine("Iteration #{0}", blockIndex);
                    Console.WriteLine("Block to change({1}b) - {0}", StringUtils.ToHex(blockToChange, ""), blockToChange.Length);
                    Console.WriteLine("Block to decode({1}b) - {0}", StringUtils.ToHex(blockToDecode, ""), blockToDecode.Length);

                    var knownBytes = new byte[blockToChange.Length];
                    for (var byteToGuessIndex = (Byte)(blockToChange.Length - 1); byteToGuessIndex >= 0 && byteToGuessIndex != 255; byteToGuessIndex--)
                    {
                        Console.Write("Block{1}[#{0}] =", byteToGuessIndex, blockIndex);

                        var cursorPosition = new Point(Console.CursorLeft, Console.CursorTop);
                        foreach (var guess in m_orderedTable)
                        {
                            Console.CursorLeft = cursorPosition.X;
                            var block =  blockToChange.ToArray();
                            // восстанавливаем известные байты
                            var paddingByte = (Byte)(block.Length - byteToGuessIndex);
                            for (var knownByteIndex = (Byte)(block.Length - 1); knownByteIndex > byteToGuessIndex; knownByteIndex--)
                            {
                                block[knownByteIndex] = (byte)(block[knownByteIndex] ^ knownBytes[knownByteIndex] ^ paddingByte);
                            }

                            block[byteToGuessIndex] = (Byte)(blockToChange[byteToGuessIndex] ^ guess ^ paddingByte);
                            Console.Write("trying {0:X2}('{1}') => {2} ...", 
                                guess, 
                                char.IsLetterOrDigit((char)guess) || char.IsSeparator((char)guess) || char.IsPunctuation((char)guess) ? (char)guess : '?',
                                StringUtils.ToHex(block, "")
                                );

                            // спрашиваем корректность догадки у PaddingOracle'a
                            var previousBlocks = Enumerable.Range(0, blockIndex).SelectMany(x => blocks[x]).ToArray();
                            var ctGuess = previousBlocks.Concat(block.Concat(blockToDecode)).ToArray();
                            var guessResult = oracle.QueryOracle(ctGuess);
                            if (guessResult == HttpStatusCode.NotFound ||guessResult == HttpStatusCode.OK)
                            {
                                var ok = true;
                                if (guessResult == HttpStatusCode.OK)
                                {
                                    ok = false;
                                    var possiblePadding = knownBytes[knownBytes.Length - 1];
                                    if (guess == possiblePadding)
                                    {
                                        // ?
                                        var paddingBytes = knownBytes.Skip(knownBytes.Length - possiblePadding).ToArray();
                                        paddingBytes[0] = guess;
                                        if (paddingBytes.All(x => x == possiblePadding))
                                        {
                                            //
                                            ok = true;
                                        }
                                    }
                                }
                                if (ok)
                                {
                                    Console.WriteLine("OK");
                                    knownBytes[byteToGuessIndex] = (byte)(guess);
                                    break;
                                }
                            }
                            else if (guessResult == HttpStatusCode.Forbidden)
                            {
                            }
                            else
                            {
                                throw new ApplicationException(string.Format("Unexpected response from server - {0}", guessResult));
                            }

                            if (guess == byte.MaxValue)
                            {
                                throw new ApplicationException(string.Format("Could not guess byte #{0} of block #{1}", byteToGuessIndex, blockIndex));
                            }
                        }
                    }
                    var knownBytesPadding = knownBytes[knownBytes.Length - 1];
                    if (knownBytesPadding != 0)
                    {
                        if (knownBytesPadding <= knownBytes.Length)
                        {
                            var paddingBytes = knownBytes.Skip(knownBytes.Length - knownBytesPadding);
                            if (paddingBytes.All(x => x == knownBytesPadding))
                            {
                                knownBytes = knownBytes.Take(knownBytes.Length - knownBytesPadding).ToArray();
                            }
                        }
                    }
                    Console.WriteLine("Block{0} = '{1}'",blockIndex, Encoding.ASCII.GetString(knownBytes));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception - {0}", ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }


    }
}
