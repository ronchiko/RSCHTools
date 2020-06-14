using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// Represents a roman number
    /// </summary>
    public class Roman
    {
        private uint value;

        /// <summary>
        /// The decimal form of the number
        /// </summary>
        public int Integer => (int)value;

        /// <summary>
        /// Creates a <see cref="Roman"/> from an <see cref="int"/>
        /// </summary>
        /// <param name="value"></param>
        public Roman(uint value)
        {
            this.value = value;
        }

        /// <summary>
        /// The string representation of the roman number
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            uint mult = 1;
            uint value = this.value;

            while(value > 0)
            {
                uint current = value % (10 * mult);
                uint overall = 0;


                StringBuilder sub = new StringBuilder();
                while(overall != current)
                {
                    uint diffrence = (uint)Math.Abs((int)current - (int)overall);

                    GetClosestLetter(diffrence);
                }
                sb.Insert(0, sub.ToString());

                mult *= 10;
                value -= current;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string into a roman numeral
        /// </summary>
        /// <param name="roman"></param>
        /// <returns></returns>
        public static Roman Parse(string roman)
        {
            uint previous = 0;

            uint total = 0;

            foreach(char letter in roman)
            {
                uint current = LetterTable(letter);

                if (current > previous)
                    current -= 2 * previous;

                total += current;

                previous = current;
            }

            return new Roman(total);
        }
        private static uint LetterTable(char letter)
        {
            switch (letter)
            {
                case 'I':
                    return 1;
                case 'V':
                    return 5;
                case 'X':
                    return 10;
                case 'L':
                    return 50;
                case 'C':
                    return 100;
                case 'D':
                    return 500;
                case 'M':
                    return 1000;
                default:
                    throw new Exception("Unknown roman numeral");
            }
        }
        private static char GetClosestLetter(uint v)
        {
            if (v >= 1000) return 'M';
            else if(v < 1000 && v >= 100)
            {
                if (v <= 300) return 'C';
                if (v > 300 && v < 900) return 'D';
                if (v >= 900) return 'M';
            }else if(v < 100 && v >= 10)
            {
                if (v <= 30) return 'X';
                if (v > 30 && v < 90) return 'L';
                if (v >= 90) return 'C';
            }else
            {
                if (v <= 3) return 'I';
                if (v > 3 && v < 9) return 'V';
                if (v >= 9) return 'X';
            }
            throw new Exception("Illegal value");
        }
        #region Operators
        /// <summary>
        /// Creates a new roman number from a string
        /// </summary>
        /// <param name="roman"></param>
        public static implicit operator Roman(string roman) => Parse(roman);

        #region Equality
        /// <summary>
        /// Equality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(Roman r1, Roman r2)
        {
            return r1.value == r2.value;
        }
        /// <summary>
        /// Inequality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(Roman r1, Roman r2)
        {
            return r1.value != r2.value;
        }
        /// <summary>
        /// Equality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(Roman r1, string r2)
        {
            return r1.value == Parse(r2).value;
        }
        /// <summary>
        /// Inequality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(Roman r1, string r2)
        {
            return r1.value != Parse(r2).value;
        }
        /// <summary>
        /// Equality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(Roman r1, int r2)
        {
            return r1.value == r2;
        }
        /// <summary>
        /// Inequality operator for 2 roman numerals
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(Roman r1, int r2)
        {
            return r1.value != r2;
        }
        #endregion


        #endregion

    }
}
