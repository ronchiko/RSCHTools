using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace RCSHTools.Parsing
{
    public class StringParser
    {
        public readonly static Tuple<char, char> STRING = new Tuple<char, char>('"', '"');
        public readonly static Tuple<char, char> BRACKETS = new Tuple<char, char>('(', ')');
        public readonly static Tuple<char, char> SQUARE_BRACKETS = new Tuple<char, char>('[', ']');
        public readonly static Tuple<char, char> CURLY_BRACKETS = new Tuple<char, char>('{', '}');

        private string str;
        private StringBuilder sb;
        private int index;
        private Stack<int> memory;

        /// <summary>
        /// Can the parser read anymore?
        /// </summary>
        public bool CanRead => index < str.Length;

        /// <summary>
        /// Creates a new string parser
        /// </summary>
        /// <param name="str"></param>
        public StringParser(string str)
        {
            this.str = str;
            sb = new StringBuilder();
            index = 0;
            memory = new Stack<int>();
            memory.Push(0);
        }

        /// <summary>
        /// Reads the characters
        /// </summary>
        /// <returns></returns>
        public char Read() => CanRead ? str[index++] : (char)0;
        /// <summary>
        /// Show the next charater that is about to be read
        /// </summary>
        /// <returns></returns>
        public char Peek() => CanRead ? str[index] : (char)0;

        #region Reading Operations
        /// <summary>
        /// Reads the next word on the string
        /// </summary>
        /// <returns></returns>
        public string ReadNext()
        {
            // Store memory
            memory.Push(index);

            sb.Clear();
            while (CanRead && char.IsWhiteSpace(Peek()))
                Read();
            while (CanRead && !char.IsWhiteSpace(Peek())) sb.Append(Read());
            return sb.ToString();
        }
        /// <summary>
        /// <inheritdoc cref="ReadPair(char, char)"/>
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string ReadPair(Tuple<char, char> pair)
        {
            return ReadPair(pair.Item1, pair.Item2);
        }
        /// <summary>
        /// Reads all data between two character
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string ReadPair(char opener, char closer)
        {
            memory.Push(index);
            sb.Clear();
            while (CanRead && Peek() != opener) Read(); Read();
            // If the opener and the closer are the same then call a diffrent function
            if (opener == closer) return ReadPairUnleveled(opener);

            int level = 1;

            while (CanRead && (level == 1 && Peek() != closer || level > 1))
            {
                if (Peek() == opener) level++;
                else if (Peek() == closer) level--;
                sb.Append(Read());
            }
            Read();
            return sb.ToString();
        }
        /// <summary>
        /// <inheritdoc cref="ReadPair(char, char)"/> (which are equal)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string ReadPairUnleveled(char p)
        {
            while (CanRead && Peek() != p) sb.Append(Read());
            Read();
            return sb.ToString();
        }

        /// <summary>
        /// Reads every thing until it encouters a specific character
        /// </summary>
        /// <param name="closer"></param>
        /// <returns></returns>
        public string ReadUntil(char closer)
        {
            memory.Push(index);
            sb.Clear();
            while (CanRead && Peek() != closer) sb.Append(Read());

            return sb.ToString();
        }

        /// <summary>
        /// Try to read a pair, if it fails all reading is undone
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryReadPair(Tuple<char, char> pair, out string output)
        {
            return TryReadPair(pair.Item1, pair.Item2, out output);
        }
        /// <summary>
        /// <inheritdoc cref="TryReadPair(Tuple{char, char}, out string)"/>
        /// </summary>
        /// <param name="opener"></param>
        /// <param name="closer"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryReadPair(char opener, char closer, out string output)
        {
            memory.Push(index);
            sb.Clear();
            while (CanRead && Peek() != opener) Read();

            if(!CanRead)
            {
                output = null;
                Revert(); return false;
            }

            output = ReadPair(opener, closer);
            return true;
        }

        /// <summary>
        /// Trys to read every thing 
        /// </summary>
        /// <param name="closer"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool TryReadUntil(char closer, out string output)
        {
            memory.Push(index); sb.Clear();

            while(Peek() != closer)
            {
                sb.Append(Read());
                if (!CanRead) { index = memory.Pop(); output = sb.ToString(); return false; }
            }

            output = sb.ToString();
            return true;
        }
        #endregion

        #region Memory Operations
        /// <summary>
        /// Clears the parsers memory
        /// </summary>
        private void ClearMemory() => memory.Clear();
        /// <summary>
        /// Reverts the parser cursor one read back
        /// </summary>
        public void Revert() => index = memory.Count > 0 ?  memory.Pop() : 0;
        /// <summary>
        /// Reverts the parser's cursor a couple of stages back
        /// </summary>
        /// <param name="stages"></param>
        public void Revert(int stages)
        {
            while (stages-- > 0 && memory.Count > 0) Revert();
        }
        /// <summary>
        /// Records the current index into memory
        /// </summary>
        public int Record()
        {
            memory.Push(index);
            return memory.Count;
        }
        /// <summary>
        /// Moves the parser's pointer to a given index
        /// </summary>
        /// <param name="index"></param>
        public void MoveTo(int index)
        {
            Record(); MoveTo(index);
        }
        /// <summary>
        /// Clears the memory an fixed amount backwards
        /// </summary>
        /// <param name="backwards"></param>
        public void ClearMemory(int backwards)
        {
            while (memory.Count > 0 && backwards-- > 0) memory.Pop();
        }
        /// <summary>
        /// Reverts the memory to a previus state (usssualy taken with a call to Record())
        /// </summary>
        /// <param name="record"></param>
        public void RevertTo(int record)
        {
            while (record < memory.Count) memory.Pop();
        }
        #endregion

        public override string ToString()
        {
            return str;
        }
    }
}
