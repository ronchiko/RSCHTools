using System;
using System.Collections;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// Masks an array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayMask<T> : IMask<T>, IEnumerable, ICloneable, IEquatable<T[]>, IEquatable<ArrayMask<T>>
    {
        /// <summary>
        /// Where the mask starts
        /// </summary>
        protected int start;
        /// <summary>
        /// Where the mask ends
        /// </summary>
        protected int end;
        /// <summary>
        /// The masked array
        /// </summary>
        protected T[] array;

        /// <summary>
        /// The length of the mask
        /// </summary>
        public int Length => end - start;
        /// <summary>
        /// The length of the array unmasked
        /// </summary>
        public int ArrayLength => array.Length;

        /// <summary>
        /// Modifies an item in the array relative to the mask range
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException("Index out of mask range");

                return array[start + index];
            }
            set
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException("Index out of mask range");

                array[start + index] = value;
            }
        }

        /// <summary>
        /// Creates a new array mask from an existing array
        /// </summary>
        /// <param name="array">Array to mask</param>
        /// <param name="start">Where does the mask start</param>
        /// <param name="end">Where does the mask end</param>
        public ArrayMask(T[] array, int start, int end)
        {
            if (start > end)
                throw new Exception("A mask's start can't be greater the the mask's end");

            this.array = array;
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Returns the index of the first accurnce of an item in the mask
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = start; i < end; i++)
            {
                if (array[i].Equals(item))
                    return i - start;
            }
            return -1;
        }
        /// <summary>
        /// Resized the mask
        /// </summary>
        /// <param name="length"></param>
        public void Resize(int length)
        {
            if (start + length > ArrayLength) throw new Exception("Mask end goes out of the array range");
            end = start + length;
        }

        #region Interfaces
        /// <summary>
        /// Compares this mask to another mask
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<ArrayMask<T>>.Equals(ArrayMask<T> other)
        {
            return other.array == array && other.start == start && other.end == end;
        }
        /// <summary>
        /// Compares this mask to another array
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<T[]>.Equals(T[] other)
        {
            if (Length != other.Length)
                return false;
            for(int i = 0;i < other.Length; i++)
            {
                if (!other[i].Equals(this[i])) return false;
            }
            return true;
        }
        /// <summary>
        /// Clones this mask
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return new ArrayMask<T>(array, start, end);
        }
        /// <summary>
        /// Creates a mask fro this mask
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IMask<T> IMask<T>.Mask(int from, int to)
        {
            return new ArrayMask<T>(array, start + from, start + to);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ArrayMaskEnumerator(this);
        }
        /// <summary>
        /// Enumarator for an array mask
        /// </summary>
        public class ArrayMaskEnumerator : IEnumerator
        {
            private ArrayMask<T> mask;
            private int position;

            object IEnumerator.Current => mask[position];
            public T Current => mask[position];

            public ArrayMaskEnumerator(ArrayMask<T> mask)
            {
                this.mask = mask;
                position = -1;
            }

            public bool MoveNext()
            {
                position++;
                return position < mask.Length;
            }

            public void Reset()
            {
                position = -1;
            }
        }

        #endregion
    }
}
