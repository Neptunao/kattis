using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kattis.IO;

namespace PrimeReduction
{
    //IMHO much better to use named Tuples in new C#
    [DebuggerDisplay("{Reduction} {Count}")]
    public struct PrimeReductionWithCount
    {
        public readonly int Reduction;
        public readonly int Count;

        public PrimeReductionWithCount(int reduction, int count)
        {
            Reduction = reduction;
            Count = count;
        }

        public override string ToString()
        {
            return $"{Reduction} {Count}";
        }
    }

    public static class PrimeFunctions
    {
        private static readonly Random Rnd = new Random();
        private static readonly Dictionary<int, PrimeReductionWithCount> Precalc
            = new Dictionary<int, PrimeReductionWithCount>
            {
                { 2, new PrimeReductionWithCount(2, 1) },
                { 3, new PrimeReductionWithCount(3, 1) }
            };

        public static Task BeginPrecalc(int min, int max)
        {
            return Task.Factory.StartNew(() =>
            {
                for(int i = min; i <= max; i++)
                {
                    Precalc[i] = Reduce(i);
                }
            });
        }

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

            int shift;

            for(shift = 0; (u | v) % 2 == 0; shift++)
            {
                u >>= 1;
                v >>= 1;
            }

            while(u % 2 == 0)
                u >>= 1;

            do
            {
                while(v % 2 == 0)
                    v >>= 1;

                if(u > v)
                {
                    int t = v;
                    v = u;
                    u = t;
                }

                v = v - u;
            } while(v != 0);

            return u << shift;
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
            if(Precalc.ContainsKey(n))
            {
                return Precalc[n];
            }
            var n1 = n;
            int i = 1;
            while(!IsPrime(n1))
            {
                Debug.WriteLine($"PrimeReductionWithCount: {n1} step {i}");
                PrimeReductionWithCount v;
                if(Precalc.TryGetValue(n1, out v))
                {
                    Debug.WriteLine("Hit!");
                    return new PrimeReductionWithCount(v.Reduction,
                        v.Count + i - 1);
                }

                Debug.WriteLine("Miss!");
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
            PrimeFunctions.BeginPrecalc(5, 10000);

            int n;
            var nums = new List<int>(20000);
            var scanner = new Scanner();
            var w = new BufferedStdoutWriter();
            var resStr = new StringBuilder(10000);

            while((n = scanner.NextInt()) != 4)
            {
                nums.Add(n);
            }

            var sw = Stopwatch.StartNew();
            var t = Task.Factory.StartNew(() =>
            {
                foreach(var reduce in nums.Select(PrimeFunctions.Reduce))
                {
                    resStr.AppendLine(reduce.ToString());
                }
            });
            t.Wait();
            sw.Stop();
            w.Write(resStr.ToString());
            w.Flush();
            Console.WriteLine($"Execution took {sw.ElapsedMilliseconds}ms");
        }
    }
}