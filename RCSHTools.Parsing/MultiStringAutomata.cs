using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Parsing
{
    /// <summary>
    /// An automata that can accept many strings
    /// </summary>
    public class MultiStringAutomata : Automata, IMultiAutomata<char>
    {
        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.States"/>
        /// </summary>
        public IAutomataState<char>[] States { get; }

        private Dictionary<int, string> stateToWord;
        /// <summary>
        /// <inheritdoc cref="MultiStringAutomata"/>
        /// </summary>
        /// <param name="values"></param>
        public MultiStringAutomata(params string[] values) : this(false, values) {}
        /// <summary>
        /// <inheritdoc cref="MultiStringAutomata"/>
        /// </summary>
        /// <param name="noWhitespace"></param>
        /// <param name="values"></param>
        public MultiStringAutomata(bool noWhitespace, params string[] values)
        {
            stateToWord = new Dictionary<int, string>();
            List<IAutomataState<char>> states = new List<IAutomataState<char>>();

            states.AddRange(CreateSingleStateAutomata(values[0], noWhitespace, 0));
            stateToWord.Add(states.Count - 1, values[0]);

            for (int i = 1; i < values.Length; i++)
            {
                int si, ci;
                if(!TestSubAutomata(states, values[i], out si, out ci))
                {
                    int nas = states.Count;
                    states.AddRange(CreateSingleStateAutomata(values[i].Substring(ci + 1), noWhitespace, states.Count));
                    ((MultiAutomataState<char>)states[si]).AddPassage(values[i][ci], nas);
                    stateToWord.Add(states.Count - 1, values[i]);
                }
            }
            States = states.ToArray();
        }

        /// <summary>
        /// <inheritdoc cref="IMultiAutomata{T}.Pass(StringParser, out int)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="receivedWord"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp, out string receivedWord)
        {
            receivedWord = null;
            int state; bool result = Pass(sp, out state);
            if (result)
            {
                receivedWord = stateToWord[state];
            }
            return result;       
        }
        /// <summary>
        /// <inheritdoc cref="IMultiAutomata{T}.Pass(StringParser, out int)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp, out int state)
        {
            state = 0;
            int i = 0;
            while ((i = States[i].Read(sp.Read())) >= 0)
            {
                state = i;
            }
            return i == FINISHED;
        }
        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.Pass(StringParser)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp)
        {
            int temp; return Pass(sp, out temp);
        }
    }
}
