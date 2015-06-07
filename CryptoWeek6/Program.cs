using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWeek6
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var N = BigInteger.Parse("179769313486231590772930519078902473361797697894230657273430081157732675805505620686985379449212982959585501387537164015710139858647833778606925583497541085196591615128057575940752635007475935288710823649949940771895617054361149474865046711015101563940680527540071584560878577663743040086340742855278549092581");

                var A = N.Sqrt();

                var squareA = BigInteger.ModPow(A, 2, N);
                var underRoot = squareA - N;
                var x = (squareA - N).Sqrt();

                var p = A - x;
                var q = A + x;

                Console.WriteLine("N = {0}", N);
                Console.WriteLine("A = {0}", A);
                Console.WriteLine("x = {0}", x);
                Console.WriteLine("p = {0}", p);
                Console.WriteLine("q = {0}", q);

                Console.WriteLine("pq = {0}", (p*q)%N);
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

    public static class Extensions
    {
        public static BigInteger Sqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        // Modular multiplicative inverse using extended Euclidean algorithm.
        public static BigInteger ModInverse(this BigInteger a, BigInteger b)
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
