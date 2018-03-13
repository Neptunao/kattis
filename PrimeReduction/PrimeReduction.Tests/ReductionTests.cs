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
        [TestCase(2, new uint[] { 2 })]
        [TestCase(10, new uint[] { 2, 5 })]
        [TestCase(16, new uint[] { 2, 2, 2, 2 })]
        [TestCase(231, new uint[] { 3, 7, 11 })]
        [TestCase(104723, new uint[] { 104723 })]
        [TestCase(999888777, new uint[] { 3, 3, 37, 3002669 })]
        public void TestFactorization(int n, uint[] res)
        {
            CollectionAssert.AreEquivalent(res, PrimeFunctions.GetPrimes((uint)n));
        }

        [TestCase(2, ExpectedResult = new uint[] { 2, 1 })]
        [TestCase(3, ExpectedResult = new uint[] { 3, 1 })]
        [TestCase(5, ExpectedResult = new uint[] { 5, 1 })]
        [TestCase(76, ExpectedResult = new uint[] { 23, 2 })]
        [TestCase(100, ExpectedResult = new uint[] { 5, 5 })]
        [TestCase(2001, ExpectedResult = new uint[] { 5, 6 })]
        [TestCase(999888777, ExpectedResult = new uint[] { 43, 7 })]
        public uint[] TestReduce(int n)
        {
            var t = PrimeFunctions.Reduce((uint)n);
            return new uint[] { t.Reduction, t.Count };
        }

        [Test]
        public void BigInputTest()
        {
            var rnd = new Random(42);
            var source = Enumerable.Repeat(1, 1000)
                .Select(n => (uint)rnd.Next(100000001, 1000000000))
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
            for(uint i = 5; i < res.Length; i++)
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
