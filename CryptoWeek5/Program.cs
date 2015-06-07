using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using XMLib;

namespace CryptoWeek5
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             p=13407807929942597099574024998205846127479365820592393377723561443721764030073546976801874298166903427690031858186486050853753882811946569946433649006084171
             g=11717829880366207009516117596335367088558084999998952205599979459063929499736583746670572176471460312928594829675428279466566527115212748467589894601965568
             h=3239475104050450443565264378728065788649097520952449527834792452971981976143292558073856937958553180532878928001494706097394108577585732452307673444020333
             */

            try
            {
                Console.WriteLine("Q6 {0}", (ModInverse(3,19) * 5) % 19);
                //Q7 
                var q7Elements = Enumerable.Range(0, 35).Where(x => GCD(35, x) == 1).ToArray();
                Console.WriteLine("Q7({0}) = {1}", q7Elements.Length, String.Join(",", q7Elements));
                Console.WriteLine();
                
                Console.WriteLine("Q8 {0}", BigInteger.ModPow(2, 10001, 11));
                Console.WriteLine("Q9 {0}", BigInteger.ModPow(2, 245, 35));

                var q10Group =
                    new[] { BigInteger.Parse("1") }.Concat(Enumerable.Range(0, 20).Select(x => BigInteger.ModPow(2, x, 35)).Skip(1).TakeWhile(x => x != 1))
                        .ToArray();
                Console.WriteLine("Q10({1}) {0}", String.Join(", ", q10Group), q10Group.Length);
                

                Console.WriteLine("Q11");
                var possibleGenerators = new[] { 9, 6, 10, 4, 7 };
                var maxPower = 13;
                Console.WriteLine("Z{0} = {1}", maxPower, String.Join(", ", Enumerable.Range(0, maxPower).Where(x => GCD(maxPower, x) == 1).ToArray()));
                foreach (var generator in possibleGenerators)
                {
                    var elements = Enumerable.Range(0, maxPower).Select(x => BigInteger.ModPow(generator, x, maxPower)).ToArray();
                    Console.WriteLine("{0} in Z{1} = {2}",generator,maxPower, String.Join(", ",elements.OrderBy(x=>x)));
                }

                {
                    var a = 1;
                    var b = 4;
                    var c = 1;
                    var n = 23;

                    var denumerator = ModInverse(2 * a, n);
                    var underSquare = BigInteger.ModPow(b, 2, n) - (4 * a * c % n);
                    var root = BigInteger.ModPow(12, 6, 23);
                }

                // q13
                var t = BigInteger.ModPow(13, 11, 19);
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

        static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }

        private static void ProgrammingAssignment()
        {
            var p =
                BigInteger.Parse(
                    "13407807929942597099574024998205846127479365820592393377723561443721764030073546976801874298166903427690031858186486050853753882811946569946433649006084171");

            var g =
                BigInteger.Parse(
                    "11717829880366207009516117596335367088558084999998952205599979459063929499736583746670572176471460312928594829675428279466566527115212748467589894601965568");

            var h =
                BigInteger.Parse(
                    "3239475104050450443565264378728065788649097520952449527834792452971981976143292558073856937958553180532878928001494706097394108577585732452307673444020333");

            Console.WriteLine(
                "Input:\r\n\t{0}",
                String.Join("\r\n\t", new[] { String.Format("{0} = {1}", "p", p), String.Format("{0} = {1}", "g", g), String.Format("{0} = {1}", "h", h), }));
            SmartForce(h, g, p);


        }

        private static void SmartForce(BigInteger h, BigInteger g, BigInteger p)
        {
            var B = (int)Math.Pow(2, 20);

            var hashtable = new Dictionary<BigInteger, int>();

            var gInversed = ModInverse(g, p);
            var previousValue = h;
            for (int x1 = 1; x1 < B; x1++)
            {
                // h * (g^-1) ^ i
                var leftSide = (previousValue * gInversed) % p;
                previousValue = leftSide;
                hashtable[leftSide] = x1;
                if (x1 % 1000 == 0)
                {
                    Console.WriteLine("X1 = {0} / {1}", x1, B);
                }
            }

            previousValue = 1;
            var gPowB = BigInteger.ModPow(g, B, p);
            for (int x0 = 1; x0 < B; x0++)
            {
                var rightSide = (previousValue * gPowB) % p;
                previousValue = rightSide;
                int x1;
                if (hashtable.TryGetValue(rightSide, out x1))
                {
                    Console.WriteLine();
                    Console.WriteLine("X0 = {0}\r\nX1 = {1}", x0, x1);

                    long x = (long)x0 * B + x1;
                    Console.WriteLine("X = {0} | {0:X}", x);

                    var check = BigInteger.ModPow(g, x, p);
                    if (check == h)
                    {
                        Console.WriteLine("Sln found !");
                    }
                    break;
                }

                if (x0 % 1000 == 0)
                {
                    Console.WriteLine("X0 = {0} / {1}", x0, B);
                }
            }
        }

        private static void RawForce(BigInteger h, BigInteger g, BigInteger p)
        {
            var hashtable = new Dictionary<BigInteger, int>();

            var B = (int)Math.Pow(2, 20);
            for (int x1 = 1; x1 < B; x1++)
            {
                // h * (g^-1) ^ i
                var gPowX1 = BigInteger.ModPow(g, x1, p);
                var inverse = ModInverse(gPowX1, p);
                var leftSide = (h * inverse) % p;
                hashtable[leftSide] = x1;
                if (x1 % 1000 == 0)
                {
                    Console.WriteLine("X1 = {0} / {1}", x1, B);
                }
            }

            var gPowB = BigInteger.ModPow(g, B, p);
            for (int x0 = 1; x0 < B; x0++)
            {
                var rightSide = BigInteger.ModPow(gPowB, x0, p);
                int x1;
                if (hashtable.TryGetValue(rightSide, out x1))
                {
                    Console.WriteLine();
                    Console.WriteLine("X0 = {0}\r\nX1 = {1}", x0, x1);

                    var x = (x0 * B + x1) % p;
                    Console.WriteLine("X = {0}", x);

                    var check = BigInteger.ModPow(g, x, p);
                    if (check == h)
                    {
                        Console.WriteLine("Sln found !");
                    }
                    break;
                }

                if (x0 % 1000 == 0)
                {
                    Console.WriteLine("X0 = {0} / {1}", x0, B);
                }
            }
        }

        private static void BruteForce(BigInteger h, BigInteger g, BigInteger p)
        {
            var previousValue = BigInteger.One;
            var max = (long)Math.Pow(2, 40);
            for (var i = 0; i < max; i++)
            {
                var gPowX = (previousValue * g) % p;
                previousValue = gPowX;
                if (previousValue == h)
                {
                    Console.WriteLine("X = {0}", i);
                    break;
                }

                if (i % 100000 == 0)
                {
                    Console.WriteLine("X? = {0} / {1}", i, max);
                }
            }
            Console.ReadKey();
        }

        // Modular multiplicative inverse using extended Euclidean algorithm.
        private static BigInteger ModInverse(BigInteger a, BigInteger b)
        {
            BigInteger dividend = a % b;
            BigInteger divisor = b;

            BigInteger last_x = BigInteger.One;
            BigInteger curr_x = BigInteger.Zero;

            while (divisor.Sign > 0)
            {
                BigInteger quotient = dividend / divisor;
                BigInteger remainder = dividend % divisor;
                if (remainder.Sign <= 0)
                {
                    break;
                }

                /* This is quite clever, in the algorithm in form
                 * ax + by = gcd(a, b) we only keep track of the
                 * value curr_x and the last_x from last iteration,
                 * the y value is ignored anyway. After remainder
                 * runs to zero, we get our inverse from curr_x */
                BigInteger next_x = last_x - curr_x * quotient;
                last_x = curr_x;
                curr_x = next_x;

                dividend = divisor;
                divisor = remainder;
            }

            if (divisor != BigInteger.One)
            {
                throw new Exception("Numbers a and b are not relatively primes");
            }
            return (curr_x.Sign < 0 ? curr_x + b : curr_x);
        }
    }
}
