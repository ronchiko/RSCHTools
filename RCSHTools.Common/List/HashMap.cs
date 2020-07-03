using System;
using System.Collections;
using System.Collections.Generic;

namespace RCSHTools
{
    /// <summary>
    /// Basic abstract hashmap class containing utitlty for creating a hashmap
    /// </summary>
    public abstract class HashMap
    {
        /// <summary>
        /// The defualt size of a hashmap
        /// </summary>
        public const int DEFAULT_SIZE = 50;
        /// <summary>
        /// Basic hash function for a string
        /// </summary>
        public static readonly Func<string, int> HASH_STRING = (s) =>
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++) count += s[i];
            return count;
        };

    }
    /// <summary>
    /// An hashmap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashMap<T> : HashMap, ICollection<T>
    {       
        private Node<T>[] array;
        private Func<T, int> hashFunc;

        /// <summary>
        /// The amount of items in the hashmap
        /// </summary>
        public int Count { get; private set; }
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// <inheritdoc cref="HashMap{T}"/>
        /// </summary>
        /// <param name="hash"></param>
        public HashMap(Func<T, int> hash) : this(DEFAULT_SIZE, hash) { }
        /// <summary>
        /// Creates a new hashmap
        /// </summary>
        public HashMap(int size, Func<T, int> hash)
        {
            Count = 0;
            hashFunc = hash;
            array = new Node<T>[size];
        }

        /// <summary>
        /// Adds an item to the hashmap
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int index = HashItem(item);
            if (!SearchIndex(index, item))
            {
                Count++;
                array[index] = new Node<T>(item, array[index]);
            }
        }

        /// <summary>
        /// Clears the hashmap
        /// </summary>
        public void Clear()
        {
            Count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = null;
            }
        }
        /// <summary>
        /// Checks if an item is on the hashmap
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            int index = HashItem(item);
            return SearchIndex(index, item);
        }

        /// <summary>
        /// Removes an item from the hashmap
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            int index = HashItem(item);
            Node<T> node = array[index];
            if (node.Value.Equals(item)) {array[index] = node.Next; Count--; return true; }
            while(node.Next != null)
            {
                if (node.Next.Value.Equals(item)) { node.Next = node.Next.Next; Count--; return true; }
                node = node.Next;
            }
            if (node.Value.Equals(item)) { node.Next = null; Count--; return true; }
            return false;
        }

        /// <summary>
        /// Copies the content of the hashmap to an array
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] arr, int arrayIndex)
        {
            if (arr == null) throw new ArgumentNullException("Array is null");
            if (arrayIndex < 0) throw new ArgumentException("Index cannot be a negative number");
            if (arr.Length - arrayIndex < Count) throw new ArgumentException("Array is too short");

            int index = arrayIndex;
            for (int i = 0; i < array.Length; i++)
            {
                Node<T> node = array[i];
                while(node != null)
                {
                    arr[index] = node.Value;
                    node = node.Next;
                    index++;
                }
            }
        }

        private int HashItem (T item)
        {
            return hashFunc(item) % array.Length;
        }
        private bool SearchIndex(int index, T value)
        {
            Node<T> node = array[index];
            while(node != null)
            {
                if (node.Value.Equals(value)) return true;
                node = node.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => new HashMapEnumerator(this);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new HashMapEnumerator(this);

        internal class HashMapEnumerator : IEnumerator, IEnumerator<T>
        {
            private int index;
            private T[] array;
            public T Current => array[index];
            object IEnumerator.Current => array[index];

            public HashMapEnumerator(HashMap<T> hmap)
            {
                index = -1;
                array = new T[hmap.Count]; hmap.CopyTo(array, 0);
            }

            public bool MoveNext()
            {
                return ++index < array.Length; 
            }

            public void Reset()
            {
                index = 0;
            }

            void IDisposable.Dispose() { }
        }
    }
}
