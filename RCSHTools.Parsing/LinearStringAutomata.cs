using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Parsing
{
    

    /// <summary>
    /// Can read a string from a string stream or as string parser, and tell if it matches a word
    /// </summary>
    public class LinearStringAutomata : Automata, IAutomata<char>
    {

        public IAutomataState<char>[] States { get; }

        public LinearStringAutomata(string accept, bool noWhitespace = false)
        {
            States = new IAutomataState<char>[accept.Length + (noWhitespace ? 0 : 1)];

            for (int i = 0; i < accept.Length; i++)
            {
                States[i] = new LinearAutomataState<char>(accept[i], 
                    noWhitespace && i == accept.Length - 1 ? FINISHED : i + 1);
            }
            if(!noWhitespace)
                States[States.Length - 1] = new WhiteSpaceAutomataState(FINISHED);
        }

        public bool Pass(StringParser sp)
        {
            int index = 0;
            while ((index = States[index].Read(sp.Read())) >= 0) ;
            return index == FINISHED;
        }
    }

    public struct LinearAutomataState<T> : IAutomataState<T>
    {
        private T character;
        private int next;

        public LinearAutomataState(T v, int next)
        {
            character = v;
            this.next = next;
        }

        public int Read(T read)
        {
            return read.Equals(character) ? next : -1;
        }
    }

    public struct WhiteSpaceAutomataState : IAutomataState<char>
    {
        private int next;
        public WhiteSpaceAutomataState(int next) => this.next = next;

        public int Read(char c)
        {
            return (int)c == 0 || char.IsWhiteSpace(c) ? next : -1;
        }
    }
}
