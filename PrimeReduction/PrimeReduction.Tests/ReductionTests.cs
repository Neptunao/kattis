using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeReduction.Tests
{
    [TestFixture]
    public class ReductionTests
    {
        [SetUp]
        public void SetUp()
        {
            PrimeFunctions.BeginPrecalc(100000);
        }

        [TestCase(2, new int[] { 2 })]
        [TestCase(10, new int[] { 2, 5 })]
        [TestCase(16, new int[] { 2, 2, 2, 2 })]
        [TestCase(231, new int[] { 3, 7, 11 })]
        [TestCase(104723, new int[] { 104723 })]
        [TestCase(999888777, new int[] { 3, 3, 37, 3002669 })]
        public void TestFactorization(int n, int[] res)
        {
            CollectionAssert.AreEquivalent(res, PrimeFunctions.GetPrimes(n));
        }

        [TestCase(2, ExpectedResult = new int[] { 2, 1 })]
        [TestCase(3, ExpectedResult = new int[] { 3, 1 })]
        [TestCase(5, ExpectedResult = new int[] { 5, 1 })]
        [TestCase(76, ExpectedResult = new int[] { 23, 2 })]
        [TestCase(100, ExpectedResult = new int[] { 5, 5 })]
        [TestCase(2001, ExpectedResult = new int[] { 5, 6 })]
        [TestCase(999888777, ExpectedResult = new int[] { 43, 7 })]
        public int[] TestReduce(int n)
        {
            var t = PrimeFunctions.Reduce(n);
            return new int[] { t.Reduction, t.Count };
        }

        [Test]
        public void BigInputTest()
        {
            var rnd = new Random(42);
            var source = Enumerable.Repeat(1, 1000)
                .Select(n => rnd.Next(0, 1000000000))
                .ToArray();
            var res = new PrimeReductionWithCount[source.Length];
            var sw = Stopwatch.StartNew();
            for(int i = 0; i < res.Length; i++)
            {
                res[i] = PrimeFunctions.Reduce(source[i]);
            }
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 4000);
        }

        [TestCase(1000, 25)]
        [TestCase(10000, 320)]
        public void PrecaclTimeTest(int size, int expectedTimeMs)
        {
            var res = new PrimeReductionWithCount[size];
            var sw = Stopwatch.StartNew();
            for(int i = 5; i < res.Length; i++)
            {
                res[i] = PrimeFunctions.Reduce(i);
            }
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, expectedTimeMs,
                $"Execution took {sw.ElapsedMilliseconds}ms");
        }

        [Test]
        public void IsPrimeTest()
        {
            Assert.True(PrimeFunctions.IsPrime(85771981));
        }
    }
}
