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
    [DebuggerDisplay("{Reduction} {Count}")]
    public struct PrimeReductionWithCount
    {
        public readonly uint Reduction;
        public readonly uint Count;

        public PrimeReductionWithCount(uint reduction, uint count)
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

            for(shift = 0; ((u | v) & 1) == 0; shift++)
            {
                u >>= 1;
                v >>= 1;
            }

            while((u & 1) == 0)
                u >>= 1;

            do
            {
                while((v & 1) == 0)
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

        private static bool IsProbablePrime(uint n, uint s, uint d, uint a)
        {
            ulong x = BarrettReduction.ModPow(a, d, n);
            ulong y = 0;

            while(s != 0)
            {
                y = (x * x) % n;
                if(y == 1 && x != 1 && x != (ulong)n - 1)
                    return false;
                x = y;
                s--;
            }
            return y == 1;
        }

        //Miller-Rabin primality test (deterministic for n < 3,215,031,751)
        public static bool IsPrime(uint n)
        {
            if((n & 1) == 0)
            {
                return n == 2;
            }
            if(n < 9)
            {
                return n > 1;
            }

            var d = n >> 1;
            uint s = 1;
            while((d & 1) == 0)
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
        private static uint PollardRho(uint n)
        {
            if((n & 1) == 0)
            {
                return 2;
            }

            int a = 2;
            int b = 2;
            uint gcd;
            do
            {
                int c = Rnd.Next(1, 10);
                a = GetRandom(a, c) % (int)n;
                b = GetRandom(GetRandom(b, c), c) % (int)n;
                gcd = (uint)Gcd(Math.Abs(a - b), (int)n);
            } while(gcd == 1);

            return gcd;
        }

        private static void CalcPrimeFactors(uint n, List<int> factors)
        {
            if(n < 2)
            {
                return;
            }
            if(IsPrime(n))
            {
                factors.Add((int)n);
                return;
            }

            var divisor = PollardRho(n);
            CalcPrimeFactors(divisor, factors);
            CalcPrimeFactors(n / divisor, factors);
        }

        public static List<int> GetPrimes(uint n)
        {
            if(n < 2)
            {
                throw new ArgumentException(nameof(n));
            }
            var factors = new List<int>();
            CalcPrimeFactors(n, factors);
            return factors;
        }

        public static PrimeReductionWithCount Reduce(uint n)
        {
            uint n1 = n;
            uint i = 1;
            while(!IsPrime(n1))
            {
                var readOnlyCollection = GetPrimes(n1);
                n1 = (uint)readOnlyCollection.Sum();
                i++;
            }

            return new PrimeReductionWithCount(n1, i);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            uint n;
            var nums = new List<uint>(20000);
            var scanner = new Scanner();
            var w = new BufferedStdoutWriter();
            var resStr = new StringBuilder(10000);

            while((n = (uint)scanner.NextInt()) != 4)
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
        }
    }
}