using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolishPuzzle.Tests
{
    [TestFixture]
    public class PolishPuzzleTests
    {
        [Test]
        public void SolveEqualStrings()
        {
            var s = new Solver(new[] { "x y", "abc abc" });
            Assert.AreEqual("abc", s.Solve());
        }

        [Test]
        public void SolveSimpleImpossible()
        {
            var s = new Solver(new[] { "x y", "a b" });
            Assert.AreEqual("IMPOSSIBLE", s.Solve());
        }
        [Test]
        public void SolveImpossible()
        {
            var s = new Solver(new[] { "a ab", "b bb", "c cc" });
            Assert.AreEqual("IMPOSSIBLE", s.Solve());
        }
        [Test]
        public void SolveExample1()
        {
            var s = new Solver(new[]
            {
                "are yo",
                "you u",
                "how nhoware",
                "alan arala",
                "dear de",
            });
            Assert.AreEqual("dearalanhowareyou", s.Solve());
        }
        [Test]
        public void SolveExample2()
        {
            var rnd = new Random();
            var s = new Solver(new[]
            {
                "i ie",
                "ing ding",
                "resp orres",
                "ond pon",
                "oyc y",
                "hello hi",
                "enj njo",
                "or c",
            }.OrderBy(_ => rnd.Next(-1, 1)).ToArray());

            Assert.AreEqual("ienjoycorresponding", s.Solve());
        }
        [Test]
        public void SolveExample3()
        {
            var s = new Solver(new[]
            {
                "efgh efgh",
                "d cd",
                "abc ab",
            });
            Assert.AreEqual("abcd", s.Solve());
        }
        [Test]
        public void SolveWithSeveralPaths()
        {
            var s = new Solver(new[]
            {
                "efgh efgh",
                "a ca",
                "d cd",
                "abc ab",
            });
            Assert.AreEqual("abca", s.Solve());
        }

        [Test]
        public void SolveDuplicated()
        {
            var rnd = new Random();
            var s = new Solver(new[]
            {
                "i ie",
                "ing ding",
                "resp orres",
                "ond pon",
                "oyc y",
                "hello hi",
                "enj njo",
                "or c",
                "i ie",
                "ing ding",
                "resp orres",
                "ond pon",
                "oyc y",
                "hello hi",
                "enj njo",
                "or c",
                "i ie",
                "ing ding",
                "resp orres",
                "ond pon",
                "oyc y",
                "hello hi",
                "enj njo",
                "or c",
            }.OrderBy(_ => rnd.Next(-1, 1)).ToArray());

            Assert.AreEqual("ienjoycorresponding", s.Solve());
        }

        [Test]
        public void SolveDuplicated233432()
        {
            var rnd = new Random();
            var s = new Solver(new[]
            {
                "are yo",
                "you u",
                "how nhoware",
                "alan arala",
                "dear de",
                "efgh efgh",
                "d cd",
                "abc ab",
                "i ie",
                "ing ding",
                "resp orres",
                "ond pon",
                "oyc y",
                "hello hi",
                "enj njo",
                "or c",
                "a ab",
                "b bb",
                "c cc"
            }.OrderBy(_ => rnd.Next(-1, 1)).ToArray());

            Assert.AreEqual("ienjoycorresponding", s.Solve());
        }
    }
}
