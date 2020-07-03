using System;
using System.Collections.Generic;
using System.Linq;

namespace RCSHTools.Parsing
{
    public abstract class Automata
    {
        public const int FINISHED = -2;
        public const int FAILED = -1;

        public IAutomataState<char>[] CreateSingleStateAutomata(string c, bool noWhitespace, int indexShift = 0)
        {
            IAutomataState<char>[] automata = new IAutomataState<char>[c.Length
                + (noWhitespace ? 0 : 1)];
            for (int i = 0; i < c.Length; i++)
            {
                
                automata[i] = new MultiAutomataState<char>(c[i], noWhitespace &&
                    i == c.Length - 1 ? FINISHED : i + 1 + indexShift);
            }
            if (!noWhitespace) automata[c.Length] = new WhiteSpaceAutomataState(FINISHED);

            return automata;
        }

        public bool TestSubAutomata(IEnumerable<IAutomataState<char>> auto, string str, out int stateIndex, out int charIndex)
        {
            charIndex = 0;
            int li = 0, ii = 0;
            while((ii = auto.ElementAt(ii).Read(str[charIndex++])) >= 0)
            {
                li = ii;
            }
            charIndex--; stateIndex = li;
            return ii == FINISHED;
        }
    }

    public struct MultiAutomataState<T> : IAutomataState<T>
    {
        private Dictionary<T, int> passTable;

        public MultiAutomataState(T c, int t)
        {
            passTable = new Dictionary<T, int>();
            passTable.Add(c, t);
        }

        public void AddPassage(T c, int t) => passTable.Add(c, t);

        public int Read(T v)
        {
            if (passTable.ContainsKey(v)) return passTable[v];
            return Automata.FAILED;
        }
    }
}
