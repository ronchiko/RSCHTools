using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Parsing
{
    public class MultiStringAutomata : Automata, IMultiAutomata<char>
    {
        public IAutomataState<char>[] States { get; }

        private Dictionary<int, string> stateToWord;

        public MultiStringAutomata(params string[] values) : this(false, values) {}
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

        public bool Pass(StringParser sp)
        {
            int temp; return Pass(sp, out temp);
        }
    }
}
