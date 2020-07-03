using System;
using System.Collections.Generic;
using System.Linq;

namespace RCSHTools.Parsing
{
    /// <summary>
    /// A base class the contains utilites for building automatas
    /// </summary>
    public abstract class Automata
    {
        /// <summary>
        /// Index when the automata is finished successfully
        /// </summary>
        public const int FINISHED = -2;
        /// <summary>
        /// Index when the automata have faild
        /// </summary>
        public const int FAILED = -1;

        /// <summary>
        /// Creates a linear sub automata for a string
        /// </summary>
        /// <param name="c"></param>
        /// <param name="noWhitespace"></param>
        /// <param name="indexShift">Index to shift</param>
        /// <returns></returns>
        protected IAutomataState<char>[] CreateSingleStateAutomata(string c, bool noWhitespace, int indexShift = 0)
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

        /// <summary>
        /// Tests a subautomata and outputs if it failed or succeeded and the final character index in the string and the final state index brefore the failure
        /// </summary>
        /// <param name="auto"></param>
        /// <param name="str"></param>
        /// <param name="stateIndex"></param>
        /// <param name="charIndex"></param>
        /// <returns></returns>
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

    /// <summary>
    /// State that accepts many inputs and outputs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct MultiAutomataState<T> : IAutomataState<T>
    {
        private Dictionary<T, int> passTable;

        /// <summary>
        /// <inheritdoc cref="MultiAutomataState{T}"/>
        /// </summary>
        /// <param name="c"></param>
        /// <param name="t"></param>
        public MultiAutomataState(T c, int t)
        {
            passTable = new Dictionary<T, int>();
            passTable.Add(c, t);
        }

        /// <summary>
        /// Adds a new passage to the passage table
        /// </summary>
        /// <param name="c"></param>
        /// <param name="t"></param>
        public void AddPassage(T c, int t) => passTable.Add(c, t);

        /// <summary>
        /// <inheritdoc cref="IAutomataState{T}.Read(T)"/>
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int Read(T v)
        {
            if (passTable.ContainsKey(v)) return passTable[v];
            return Automata.FAILED;
        }
    }
}
