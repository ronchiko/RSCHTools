using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Parsing
{
    public class ChainAutomata : Automata , IMultiAutomata<StringParser>
    {
        public IAutomataState<StringParser>[] States { get; }

        public ChainAutomata(params IAutomata<char>[] automatas)
        {
            States = new IAutomataState<StringParser>[automatas.Length];
            for (int i = 0; i < automatas.Length; i++)
            {
                States[i] = new AutomataWrapperState(automatas[i], i == automatas.Length - 1 ? 
                    FINISHED : i + 1);
            }
        }

        public bool Pass(StringParser sp, out int state)
        {
            state = 0;
            int i = 0;
            while ((i = States[i].Read(sp)) >= 0) state = i;
            return i == FINISHED;
        }
        public bool Pass(StringParser sp)
        {
            int temp; return Pass(sp, out temp);
        }
    }

    public struct WhiteSpaceAutomata : IAutomata<char>
    {
        public IAutomataState<char>[] States { get; }

        private char breaker;

        public WhiteSpaceAutomata(char breaker)
        {
            States = null;
            this.breaker = breaker;
        }

        public bool Pass(StringParser sp)
        {
            char c; int count = 0;
            while (sp.CanRead)
            {
                sp.Record();
                c = sp.Read(); count++;
                if (c == 0 || char.IsWhiteSpace(c)) continue;
                else
                {
                    sp.Revert(); sp.ClearMemory(--count);
                    return c == breaker;
                }
            }
            return false;
        }
    }

    public struct AutomataWrapperState : IAutomataState<StringParser>
    {
        private int next;
        private IAutomata<char> automata;

        public AutomataWrapperState(IAutomata<char> automata, int next)
        {
            this.automata = automata;
            this.next = next;
        }

        public int Read(StringParser sp)
        {
            return automata.Pass(sp) ? next : Automata.FAILED;
        }
    }

    public struct WordAutomata : IAutomata<char>
    {
        private char escape;
        public IAutomataState<char>[] States { get; }

        public WordAutomata(char escape)
        {
            this.escape = escape;
            States = null;
        }

        public bool Pass(StringParser sp)
        {
            char c;
            while ((c = sp.Read()) != 0 && !char.IsWhiteSpace(c) && c != escape) ;
            return true;
        }
    }
}
