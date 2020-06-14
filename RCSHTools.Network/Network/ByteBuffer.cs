using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Network
{
    /// <summary>
    /// Represent an array of bytes that can be translated to 
    /// </summary>
    public class ByteBuffer
    {
        internal enum BufferType
        {
            Linked,
            Array
        }
        internal class ByteCharUnion
        {
            private byte binary;
            private char value;
            public ByteCharUnion next;

            public byte Bin => binary;
            public char Char => value;

            public ByteCharUnion(byte value)
            {
                next = null;
                this.value = (char)value;
                binary = value;
            }
            public ByteCharUnion(char value)
            {
                this.value = value;
                binary = (byte)value;
                next = null;
            }

            public bool HasNext()
            {
                return next != null;
            }

            public void Set(byte c)
            {
                binary = c;
                value = (char)c;
            }
            public void Set(char c)
            {
                value = c;
                binary = (byte)c;
            }
        }
        
        // Linked arguments
        private ByteCharUnion head, tail;
        private int count;

        /// <summary>
        /// Gets and sets a value in the union
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public char this[int index]
        {
            get
            {
                if (index >= count) throw new IndexOutOfRangeException();
                return Get(index).Char;
            }
            set
            {
                if (index >= count) throw new IndexOutOfRangeException();
                Get(index).Set(value);
            }
        }

        /// <summary>
        /// Creates a new empty resizeable byte buffer
        /// </summary>
        public ByteBuffer()
        {
            count = 0;
            tail = null;
            head = null;
        }
        /// <summary>
        /// Creates a buffer from another buffer
        /// </summary>
        /// <param name="buffer"></param>
        public ByteBuffer(byte[] buffer) : this()
        {
            foreach (var b in buffer)
            {
                Add(b);
            }
        }

        /// <summary>
        /// Adds a new item to the buffer
        /// </summary>
        /// <param name="b"></param>
        public void Add(byte b)
        {
            if(head == null)
            {
                head = new ByteCharUnion(b);
                tail = head;
            }
            else
            {
                ByteCharUnion union = new ByteCharUnion(b);
                tail.next = union;
                tail = union;
            }
            count++;
        }
        /// <summary>
        /// Adds a new item to the buffer
        /// </summary>
        /// <param name="c"></param>
        public void Add(char c) => Add((byte)c);
        /// <summary>
        /// Removes an item by its index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index >= count) throw new IndexOutOfRangeException();

            if (index == 0)
            {
                if (tail == head)
                    tail = null;
                head = head.next;
            }
            else
            {
                ByteCharUnion u = Get(index - 1);
                u.next = u.next.next;
            }
            count--;
        }

        private ByteCharUnion Get(int index)
        {
            ByteCharUnion node = head;
            while(index > 0 && node != null)
            {
                node = node.next;
                index--;
            }
            return node;
        }
    }
}
