using System;
using System.Collections.Generic;
using System.Linq;

namespace PolishPuzzle
{
    public class Solver
    {
        private readonly string[][] m_strings;
        private readonly int m_length;

        public Solver(string[] strings)
        {
            m_length = strings.Length;
            m_strings = strings.
                Select(s => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).
                ToArray();
        }

        private IEnumerable<int> FindRoots()
        {
            for(int i = 0; i < m_strings.Length; i++)
            {
                var left = m_strings[i][0];
                var right = m_strings[i][1];

                if(left == right && left != "")
                {
                    yield return i;
                }
                else if(left.Length != right.Length)
                {
                    var minIdx = left.Length < right.Length ? 0 : 1;
                    var maxIdx = left.Length < right.Length ? 1 : 0;
                    if(m_strings[i][maxIdx].StartsWith(m_strings[i][minIdx]))
                        yield return i;
                }
            }
        }

        private string FindCommonPrefix(string str1, string str2)
        {
            if(str1 == str2)
                return str1;
            if(str1.Length < str2.Length && str2.StartsWith(str1))
                return str1;
            if(str2.Length < str1.Length && str1.StartsWith(str2))
                return str2;
            return null;
        }

        private void SolveWithRoot(int rootIdx, string acc, List<string> solutions)
        {
            var strings = m_strings[rootIdx];
            var commonPrefix = FindCommonPrefix(strings[0], strings[1]);
            if(commonPrefix == null)
                return;
            strings[0] = strings[0].Remove(0, commonPrefix.Length);
            strings[1] = strings[1].Remove(0, commonPrefix.Length);
            var notEmptyColumn = string.IsNullOrEmpty(strings[0]) ? 1 : 0;
            var targetStr = strings[notEmptyColumn];
            if(string.IsNullOrEmpty(targetStr))
            {
                solutions.Add(acc + commonPrefix);
                strings[0] = commonPrefix + strings[0]; //TODO remove dup
                strings[1] = commonPrefix + strings[1];
                return;
            }

            strings[notEmptyColumn] = "";
            var targetColumn = notEmptyColumn == 0 ? 1 : 0;
            for(int i = 0; i < m_length; i++)
            {
                if(m_strings[i][targetColumn].StartsWith(targetStr))
                {
                    m_strings[i][targetColumn] = m_strings[i][targetColumn].Remove(0, targetStr.Length);
                    SolveWithRoot(i, acc + commonPrefix + targetStr, solutions);
                    m_strings[i][targetColumn] = targetStr + m_strings[i][targetColumn];
                }
            }

            strings[notEmptyColumn] = targetStr;
            strings[0] = commonPrefix + strings[0];
            strings[1] = commonPrefix + strings[1];
        }

        public string Solve()
        {
            var solutions = new List<string>();
            var roots = new Queue<int>(FindRoots().ToArray());
            while(roots.Count > 0)
            {
                var rootIdx = roots.Dequeue();
                if(m_strings[rootIdx][0] == m_strings[rootIdx][1])
                {
                    solutions.Add(m_strings[rootIdx][0]);
                    m_strings[rootIdx][0] = m_strings[rootIdx][1] = "";
                    continue;
                }

                SolveWithRoot(rootIdx, "", solutions);
            }
            return solutions.Count == 0 ? "IMPOSSIBLE" : solutions.OrderBy(_ => _).First();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string countStr = null;
            var testCases = new List<string[]>(5);
            while((countStr = Console.ReadLine()) != null)
            {
                int count = int.Parse(countStr);
                testCases.Add(new string[count]);
                for(int i = 0; i < count; i++)
                {
                    testCases[testCases.Count - 1][i] = Console.ReadLine();
                }
            }
            for(int i = 0; i < testCases.Count; i++)
            {
                var solver = new Solver(testCases[i]);
                var res = solver.Solve();
                Console.WriteLine($"Case {i + 1}: {res}");
            }
        }
    }
}
