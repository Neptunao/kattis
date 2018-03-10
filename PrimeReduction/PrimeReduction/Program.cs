using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeReduction
{
    //IMHO much better to use named Tuples in new C#
    public class PrimeReductionWithCount
    {
        public readonly int Reduction;
        public readonly int Count;

        public PrimeReductionWithCount(int reduction, int count)
        {
            Reduction = reduction;
            Count = count;
        }
    }

    public static class PrimeFunctions
    {
        private static readonly Random Rnd = new Random();

        //Right-to-left binary method
        private static decimal ModPow(int a, int exponent, int mod)
        {
            if(mod == 1)
                return 0;
            decimal res = 1;
            decimal power = a;
            while(exponent > 0)
            {
                if(exponent % 2 == 1)
                {
                    res = (res * power) % mod;
                }

                exponent = exponent >> 1;
                power = (power * power) % mod;
            }
            return res;
        }

        private static int GetRandom(int x, int a)
        {
            return x * x + a;
        }

        //Greatest common divisor - Binary GCD algorithm (https://en.wikipedia.org/wiki/Binary_GCD_algorithm)
        private static int Gcd(int u, int v)
        {
            // simple cases (termination)
            if(u == v)
                return u;
            if(u == 0)
                return v;
            if(v == 0)
                return u;

            // look for factors of 2
            if(u % 2 == 0) // u is even
            {
                if(v % 2 == 1) // v is odd
                    return Gcd(u >> 1, v);
                return Gcd(u >> 1, v >> 1) << 1;
            }

            if(v % 2 == 0) // u is odd, v is even
                return Gcd(u, v >> 1);

            // reduce larger argument
            if(u > v)
                return Gcd((u - v) >> 1, v);

            return Gcd((v - u) >> 1, u);
        }

        private static bool IsProbablePrime(int n, int s, int d, int a)
        {
            decimal x = ModPow(a, d, n);
            decimal y = 0;

            while(s != 0)
            {
                y = (x * x) % n;
                if(y == 1 && x != 1 && x != n - 1)
                    return false;
                x = y;
                s--;
            }
            return y == 1;
        }

        //Miller-Rabin primality test (deterministic for n < 3,215,031,751)
        public static bool IsPrime(int n)
        {
            if(n % 2 == 0)
            {
                return n == 2;
            }
            if(n < 9)
            {
                return n > 1;
            }

            var d = n / 2;
            var s = 1;
            while(d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            return IsProbablePrime(n, s, d, 2) &&
                   IsProbablePrime(n, s, d, 3) &&
                   IsProbablePrime(n, s, d, 5) &&
                   IsProbablePrime(n, s, d, 7);
        }

        //Pollard's rho factorization algorithm
        private static int PollardRho(int n)
        {
            if(n % 2 == 0)
            {
                return 2;
            }

            int a = 2;
            int b = 2;
            int gcd;
            do
            {
                var c = Rnd.Next(1, 10);
                a = GetRandom(a, c) % n;
                b = GetRandom(GetRandom(b, c), c) % n;
                gcd = Gcd(Math.Abs(a - b), n);
            } while(gcd == 1);

            return gcd;
        }

        private static void CalcPrimeFactors(int n, List<int> factors)
        {
            if(n < 2)
            {
                return;
            }
            if(IsPrime(n))
            {
                factors.Add(n);
                return;
            }

            var divisor = PollardRho(n);
            CalcPrimeFactors(divisor, factors);
            CalcPrimeFactors(n / divisor, factors);
        }

        public static List<int> GetPrimes(int n)
        {
            if(n < 2)
            {
                throw new ArgumentException(nameof(n));
            }
            var factors = new List<int>();
            CalcPrimeFactors(n, factors);
            return factors;
        }

        public static PrimeReductionWithCount Reduce(int n)
        {
            var n1 = n;
            int i = 1;
            while(!IsPrime(n1))
            {
                var readOnlyCollection = GetPrimes(n1);
                n1 = readOnlyCollection.Sum();
                i++;
            }
            return new PrimeReductionWithCount(n1, i);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            int n;
            var nums = new List<int>(20000);
            while((n = int.Parse(Console.ReadLine())) != 4)
            {
                nums.Add(n);
            }

            var sw = Stopwatch.StartNew();
            var res = nums.Select(PrimeFunctions.Reduce).ToArray();
            sw.Stop();
            foreach(var num in res)
            {
                Console.WriteLine($"{num.Reduction} {num.Count}");
            }
            //            Console.WriteLine($"Execution took {sw.ElapsedMilliseconds}ms");
            //            Console.ReadKey();
        }
    }
}