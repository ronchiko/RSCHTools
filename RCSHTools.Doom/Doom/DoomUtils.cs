using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Utilities for doom projects
    /// </summary>
    public static class DoomUtils
    {
        /// <summary>
        /// Turns an array to a lump qualified name
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string ToName(byte[] array)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while(i < array.Length && array[i] != 0)
            {
                sb.Append((char)array[i]);
                i++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// <inheritdoc cref="ToName(byte[])"/>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToName(byte[] array, int index, int length)
        {
            StringBuilder sb = new StringBuilder();
            int i = index;
            while (i < index + length && array[i] != 0)
            {
                sb.Append((char)array[i]);
                i++;
            }
            return sb.ToString();
        }
        /// <summary>
        /// Gets a name as a <see cref="byte"/>[]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static byte[] GetNameAsBytes(string name)
        {
            if (name.Length > 8)
                throw new Exception("Name too long, name cannot surpass 8 characters");
            byte[] array = new byte[8];
            int i = 0;
            while (i < name.Length)
                array[i] = (byte)name[i++];
            while (i < 8)
                array[i] = array[i++];
            return array;
        }
    }
}
