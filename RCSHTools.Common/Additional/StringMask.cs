using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace RCSHTools
{
    /// <summary>
    /// Masks a string, and allow easier access to functionallies in that range, this mask only allows for reading functionalities
    /// </summary>
    public class StringMask : IEquatable<string>, IEquatable<StringMask>, ICloneable, IEnumerable, IMask<char>
    {
        private int start;
        private int end;
        private string str;

        /// <summary>
        /// Length of the masked string
        /// </summary>
        public int Length => end - start;
        /// <summary>
        /// Is this string an empty string
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Gets a char by its index relative to the mask start
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("Index out of mask's range");
                return str[start + index];
            }
        }

        /// <summary>
        /// Contructs a new string mask that contains the whole string
        /// </summary>
        /// <param name="str"></param>
        public StringMask(string str) : this(str, 0, str.Length) { }
        /// <summary>
        /// Constructs a new string mask from a given coordinates
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public StringMask(string str, int start, int end)
        {
            if (start > end)
                throw new Exception("A mask's start cannot be greater then the mask's end");
            this.str = str;
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Finds the first occurnce of a char in masked string
        /// </summary>
        /// <param name="c">Character to find</param>
        /// <returns></returns>
        public int IndexOf(char c)
        {
            return IndexOf(c, 0, 1);
        }
        /// <summary>
        /// Finds the first occurnce of a char in masked string from a given index
        /// </summary>
        /// <param name="c">Character to find</param>
        /// <param name="start">Start of search</param>
        /// <returns></returns>
        public int IndexOf(char c, int start)
        {
            return IndexOf(c, start, 1);
        }
        /// <summary>
        /// Gets the index of a character in the masked string
        /// </summary>
        /// <param name="c">Character to find</param>
        /// <param name="start">Where to start</param>
        /// <param name="count">Whick occurnce to select</param>
        /// <returns></returns>
        public int IndexOf(char c, int start, int count)
        {
            if(start < 0 || start >= Length) throw new IndexOutOfRangeException("Index out of mask's range");
            if (count <= 0) throw new Exception("Cannot find items with index less then or equals to 0");
            for (int i = this.start + start; i < end; i++)
            {
                if(str[i] == c)
                {
                    count--;
                    if (count == 0) return i - this.start;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns true if the mask starts with this string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool StartsWith(string str)
        {
            // If the length of the string is greater then this mask, it will never be contained in this mask
            if (str.Length > Length)
                return false;

            // Test start
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != this[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Returns true if the mask ends with this string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool EndsWith(string str)
        {
            // If the length of the string is greater then this mask, it will never be contained in this mask
            if (str.Length > Length)
                return false;

            // Compare
            for(int i = str.Length - 1, j = Length - 1; i >= 0; i--, j--)
            {
                if (this[j] != str[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Checks if a string contains a sub string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool Contains(string str)
        {
            // If the length of the string is greater then this mask, it will never be contained in this mask
            if (str.Length > Length)
                return false;

            // Test contains
            for (int j = 0; j < Length - str.Length; j++)
            {
                bool found = true;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != this[i])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return true;
            }
            return false;
        }
        /// <summary>
        /// Returns a copy of the string of this mask
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return str.Substring(start, Length);
        }

        /// <summary>
        /// Removes all white space characters from the start of the mask
        /// </summary>
        /// <returns></returns>
        public StringMask TrimStart()
        {
            int start = this.start;
            while (CharUnicodeInfo.GetUnicodeCategory(str[start]) == UnicodeCategory.SpaceSeparator && start < end)
                start++;
            return new StringMask(str, start, end);
        }
        /// <summary>
        /// Removes all white space characters from the end of the mask
        /// </summary>
        /// <returns></returns>
        public StringMask TrimEnd()
        {
            int end = this.end;
            while (CharUnicodeInfo.GetUnicodeCategory(str[end]) == UnicodeCategory.SpaceSeparator && start < end)
                end--;
            return new StringMask(str, start, end);
        }
        /// <summary>
        /// Removes all white space characters from the start and the end of the mask
        /// </summary>
        /// <returns></returns>
        public StringMask Trim()
        {
            int start = this.start;
            while (CharUnicodeInfo.GetUnicodeCategory(str[start]) == UnicodeCategory.SpaceSeparator && start < this.end)
                start++;
            int end = this.end;
            while (CharUnicodeInfo.GetUnicodeCategory(str[end - 1]) == UnicodeCategory.SpaceSeparator && start < end)
                end--;
            return new StringMask(str, start, end);
        }

        private bool CompareStringFromIndex(string str, int index)
        {
            if (Length - index < str.Length)
                return false;

            for(int i = 0; i < str.Length; i++)
            {
                if (str[i] != this[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Splits the string using a string a seperators
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="count"></param>
        /// <param name="stringSplitOptions"></param>
        /// <returns></returns>
        public StringMask[] Split(IEnumerable<string> strs, int count, StringSplitOptions stringSplitOptions)
        {
            Queue indexes = new Queue();
            int encounters = 0;

            for(int i = start; i < end; i++)
            {
                bool found = false;
                foreach (string str in strs)
                {
                    if(CompareStringFromIndex(str, i - start))
                    {
                        found = true;
                        break;
                    }
                }
                if(found && (count == -1 || encounters < count))
                {
                    indexes.Enqueue(i);
                    encounters++;
                    i += str.Length;
                }
            }

            indexes.Enqueue(end);
            int last = start;
            int maskIndex = 0;
            StringMask[] mask = new StringMask[indexes.Count];

            while(indexes.Count != 0)
            {
                int pop = (int)indexes.Dequeue();

                mask[maskIndex] = new StringMask(this.str, last + 
                    (stringSplitOptions == StringSplitOptions.RemoveEmptyEntries ? str.Length : 0), 
                    pop);

                maskIndex++;
                last = pop;
            }

            return mask;
        }

        #region Interfaces
        /// <summary>
        /// Is this mask equals to another string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(string b)
        {
            if (Length != b.Length)
                return false;

            for(int i = 0;i < b.Length; i++)
            {
                if (this[i] != b[i])
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Is this mask equals to another string mask
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(StringMask mask)
        {
            if (Length != mask.Length)
                return false;

            for (int i = 0; i < Length; i++)
            {
                if (this[i] != mask[i])
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Clones this string array
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public object Clone()
        {
            return new StringMask(str, start, end);
        }
        public IMask<char> Mask(int from, int to)
        {
            return new StringMask(str, start + from, start + to);
        }

        #region Enumrable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StringMaskEnumrator(this);
        }
        public class StringMaskEnumrator : IEnumerator
        {
            private StringMask mask;
            private int index;

            object IEnumerator.Current => mask[index];
            public char Current => mask[index];

            public StringMaskEnumrator(StringMask mask)
            {
                this.mask = mask;
                index = -1;
            }

            public bool MoveNext()
            {
                index++;
                return index < mask.Length;
            }
            public void Reset()
            {
                index = -1;
            }
        }
        #endregion

        #endregion
    }
}
