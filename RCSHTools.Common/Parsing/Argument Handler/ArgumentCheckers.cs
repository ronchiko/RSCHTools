using System;
using System.Collections.Generic;
using System.Globalization;

namespace RCSHTools
{
    /// <summary>
    /// Collection of common <see cref="Predicate{T}"/> to be used in a <see cref="ArgumentHandler"/>
    /// </summary>
    public static class ArgumentCheckers
    {
        /// <summary>
        /// Accepts anything
        /// </summary>
        public static Predicate<string> Any => (s) => true;
        /// <summary>
        /// Accepts integers
        /// </summary>
        public static Predicate<string> Integer => (s) =>
        {
            foreach (char c in s)
                if (!char.IsDigit(c) && c != '-') return false;
            return true;
        };
        /// <summary>
        /// Accepts unsigned integers
        /// </summary>
        public static Predicate<string> UnsignedInteger => (s) =>
        {
            foreach (char c in s)
                if (!char.IsDigit(c)) return false;
            return true;
        };
        /// <summary>
        /// Accepts floats
        /// </summary>
        public static Predicate<string> Float => (s) =>
        {
            foreach (char c in s)
                if (!char.IsDigit(c) && c != '.' && char.ToLower(c) == 'e' && c != '-') return false;
            return true;
        };
        /// <summary>
        /// Accepts hexadecimal values
        /// </summary>
        public static Predicate<string> Hex => (s) =>
        {
            foreach (char c in s)
                if (!(c >= '0' && c <= '9') && !(c >= 'A' && c <= 'F') &&
                !(c >= 'a' && c <= 'f'))
                    return false;

            return true;
        };
    }
}
