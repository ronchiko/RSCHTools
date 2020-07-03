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
        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.States"/>
        /// </summary>
        public IAutomataState<char>[] States { get; }

        /// <summary>
        /// <inheritdoc cref="LinearStringAutomata"/>
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="noWhitespace"></param>
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

        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.Pass(StringParser)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp)
        {
            int index = 0;
            while ((index = States[index].Read(sp.Read())) >= 0) ;
            return index == FINISHED;
        }
    }

    /// <summary>
    /// A state which only accepts a single type of char
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct LinearAutomataState<T> : IAutomataState<T>
    {
        private T character;
        private int next;

        /// <summary>
        /// <inheritdoc cref="LinearAutomataState{T}"/>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="next"></param>
        public LinearAutomataState(T v, int next)
        {
            character = v;
            this.next = next;
        }

        /// <summary>
        /// <inheritdoc cref="IAutomataState{T}.Read(T)"/>
        /// </summary>
        /// <param name="read"></param>
        /// <returns></returns>
        public int Read(T read)
        {
            return read.Equals(character) ? next : -1;
        }
    }

    /// <summary>
    /// Accepts all whitespaces
    /// </summary>
    public struct WhiteSpaceAutomataState : IAutomataState<char>
    {
        private int next;
        /// <summary>
        /// <inheritdoc cref="WhiteSpaceAutomataState"/>
        /// </summary>
        /// <param name="next"></param>
        public WhiteSpaceAutomataState(int next) => this.next = next;

        /// <summary>
        /// <inheritdoc cref="IAutomataState{T}.Read(T)"/>
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int Read(char c)
        {
            return (int)c == 0 || char.IsWhiteSpace(c) ? next : -1;
        }
    }
}
