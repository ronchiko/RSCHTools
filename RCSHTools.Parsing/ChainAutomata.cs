using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Parsing
{
    /// <summary>
    /// An automata that check if a chain of automatas succeed in a row
    /// </summary>
    public class ChainAutomata : Automata , IMultiAutomata<StringParser>
    {
        public IAutomataState<StringParser>[] States { get; }

        /// <summary>
        /// Creates a new chain automata
        /// </summary>
        /// <param name="automatas"></param>
        public ChainAutomata(params IAutomata<char>[] automatas)
        {
            States = new IAutomataState<StringParser>[automatas.Length];
            for (int i = 0; i < automatas.Length; i++)
            {
                States[i] = new AutomataWrapperState(automatas[i], i == automatas.Length - 1 ? 
                    FINISHED : i + 1);
            }
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
            while ((i = States[i].Read(sp)) >= 0) state = i;
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

    /// <summary>
    /// An automata for reading all whitespaces
    /// </summary>
    public class WhiteSpaceAutomata : IAutomata<char>
    {
        IAutomataState<char>[] IAutomata<char>.States { get; } = null;

        /// <summary>
        /// <inheritdoc cref="WhiteSpaceAutomata"/>
        /// </summary>
        public WhiteSpaceAutomata()  { }

        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.Pass(StringParser)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp)
        {
            char c;
            while (((c = sp.Peek()) == 0 || char.IsWhiteSpace(c))) sp.Read();
            return false;
        }
    }

    /// <summary>
    /// An automata state that contains an automata
    /// </summary>
    public struct AutomataWrapperState : IAutomataState<StringParser>
    {
        private int next;
        private IAutomata<char> automata;

        /// <summary>
        /// Create an automata state wrapper
        /// </summary>
        /// <param name="automata"></param>
        /// <param name="next"></param>
        public AutomataWrapperState(IAutomata<char> automata, int next)
        {
            this.automata = automata;
            this.next = next;
        }

        /// <summary>
        /// <inheritdoc cref="IAutomataState{T}.Read(T)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public int Read(StringParser sp)
        {
            return automata.Pass(sp) ? next : Automata.FAILED;
        }
    }

    /// <summary>
    /// Reads until reaches the breaker or a whitespace
    /// </summary>
    public class WordAutomata : IAutomata<char>
    {
        private char escape;
        IAutomataState<char>[] IAutomata<char>.States { get; } = null;

        /// <summary>
        /// Builds a new word automata
        /// </summary>
        /// <param name="escape"></param>
        public WordAutomata(char escape)
        {
            this.escape = escape;
        }

        /// <summary>
        /// <inheritdoc cref="IAutomata{T}.Pass(StringParser)"/>
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool Pass(StringParser sp)
        {
            char c;
            while ((c = sp.Peek()) != 0 && !char.IsWhiteSpace(c) && c != escape) sp.Read();
            return true;
        }
    }
}
