using System;
using System.Collections.Generic;
using System.IO;
using RCSHTools;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represents a lump
    /// </summary>
    public class Lump
    {
        private string name;
        private byte[] buffer;

        /// <summary>
        /// The name of the lump, Must be all uppercase and digit (8 characters long)
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                VerifyName(value);
                name = value;
            }
        }
        /// <summary>
        /// The size of the buffer
        /// </summary>
        public uint Size => (uint)buffer.Length;

        /// <summary>
        /// Called when the lump is modified
        /// </summary>
        public event LumpSavedEventHandler OnLumpSaved;

        /// <summary>
        /// The wad file this texture is from
        /// </summary>
        public WadFile File { get; private set; }
        /// <summary>
        /// Creates a zero size lump and names it, Used for markers an map headers
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public Lump(string name, WadFile file)
        {
            Name = name;
            buffer = new byte[0];
            File = file;
        }
        /// <summary>
        /// Creates an named lump from a an array
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buffer"></param>
        /// <param name="file"></param>
        public Lump(string name, byte[] buffer, WadFile file) : this(name, buffer, 0, (uint)buffer.Length, file)
        {
            
        }
        /// <summary>
        /// Creates a named lump from an array
        /// </summary>
        /// <param name="name">Name of the lump</param>
        /// <param name="buffer">The array to create from</param>
        /// <param name="offset">Where to start read from the array</param>
        /// <param name="size">The amount of bytes to read from the array</param>
        /// <param name="file"></param>
        public Lump(string name, byte[] buffer, uint offset, uint size, WadFile file)
        {
            File = file;
            this.buffer = new byte[size];
            Name = name;
            Array.Copy(buffer, offset, this.buffer, 0, size);
        }
    
        /// <summary>
        /// Attaches this lump to a file
        /// </summary>
        /// <param name="file"></param>
        public void Attach(WadFile file)
        {
            File = file;
        }
        /// <summary>
        /// Returns a copy of the buffer the lump uses
        /// </summary>
        /// <returns></returns>
        public byte[] CopyRaw()
        {
            byte[] copy = new byte[buffer.Length];
            Array.Copy(buffer, 0, copy, 0, copy.Length);
            return copy;
        }
        /// <summary>
        /// Creates a stream with the buffer data
        /// </summary>
        /// <returns></returns>
        public MemoryStream RawStream()
        {
            return new MemoryStream(buffer);
        }
        /// <summary>
        /// Override the buffer pointer of the lump, nessary to save resacled arrays
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="interpter"></param>
        public void Write(byte[] buffer, LumpReader interpter)
        {
            this.buffer = buffer;
            OnLumpSaved?.Invoke(in buffer, interpter);
        }
        /// <summary>
        /// Reads an <see cref="int"/> from the lump
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public int ReadInt32(int startIndex)
        {
            return BitConverter.ToInt32(buffer, startIndex);
        }
        /// <summary>
        /// Reads an <see cref="short"/> from the lump
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public short ReadInt16(int startIndex)
        {
            return BitConverter.ToInt16(buffer, startIndex);
        }
        /// <summary>
        /// Reads a byte from the bufer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte ReadByte(int index)
        {
            return buffer[index];
        }
        /// <summary>
        /// Reads a <see cref="string"/> from lump
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string ReadString(int startIndex, int length)
        {
            char[] characters = new char[length];

            for (int i = 0; i < length; i++)
            {
                characters[i] = (char)buffer[startIndex + i];
            }

            return new string(characters);
        }

        /// <summary>
        /// returns a new <see cref="MemoryStream"/> of the lump
        /// </summary>
        /// <returns></returns>
        public MemoryStream GetByteStream()
        {
            return new MemoryStream(buffer);
        }

        internal void Write(MemoryStream stream)
        {
            stream.Write(buffer, 0, buffer.Length);
        }
        internal static void VerifyName(string name)
        {
            // Console.WriteLine(name + ":" + name.Length);
            if (name.Length > 8) throw new Exception("Names must be only 8 characters long");
            foreach (var letter in name)
            {
                if(!char.IsDigit(letter) && !char.IsUpper(letter) && (letter != '[')
                    && letter != ']' && letter != '_' && letter != '-' && letter != '\\') throw new Exception("Illegal name character " + letter);
            }
        }
    }

    /// <summary>
    /// Handler for when a lump is modified permentantly by a interpeter
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="modifiedBy"></param>
    public delegate void LumpSavedEventHandler(in byte[] buffer, LumpReader modifiedBy);
}
