using System;
using System.Net;
using System.Net.Sockets;

namespace RCSHTools.Network
{
    /// <summary>
    /// Utility class for handling socket buffers
    /// </summary>
    public class NetBuffer : ArrayMask<byte>
    {
        private int writeIndex;

        /// <summary>
        /// Does the <see cref="NetBuffer"/> has space left to write. To clear the buffer use <see cref="Reset"/>
        /// </summary>
        public bool CanWrite => start + writeIndex < ArrayLength;

        /// <summary>
        /// Creates a new net buffer
        /// </summary>
        /// <param name="size"></param>
        public NetBuffer(int size) : base(new byte[size], 0, 0)
        {

        }

        /// <summary>
        /// Gets an <see cref="int"/> from the array
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetInt(int index)
        {
            return BitConverter.ToInt32(array, start + index);
        }
        /// <summary>
        /// Gets an <see cref="uint"/> from the array
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public uint GetUInt(int index)
        {
            return BitConverter.ToUInt32(array, start + index);
        }
        /// <summary>
        /// Gets a <see cref="ulong"/> from the buffer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ulong GetULong(int index)
        {
            return BitConverter.ToUInt64(array, start + index);
        }

        #region Writing
        /// <summary>
        /// Writes a <see cref="byte"/>[] to the buffer
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public NetBuffer Write(byte[] bytes)
        {
            foreach (var byte_ in bytes)
            {
                if (!CanWrite) throw new IndexOutOfRangeException("Buffer size is too small to write");
                if (writeIndex >= Length) Resize(Length + 1);
                this[writeIndex] = byte_;
                writeIndex++;
            }
            return this;
        }
        /// <summary>
        /// Writes data to the buffer
        /// </summary>
        /// <param name="num"></param>
        /// <param name="data"></param>
        public NetBuffer Write(int num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public NetBuffer Write(ulong num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public NetBuffer Write(uint num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public NetBuffer Write(long num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public NetBuffer Write(short num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public NetBuffer Write(char character)
        {
            return Write(BitConverter.GetBytes(character));
        }
        /// <summary>
        /// <inheritdoc cref="Write(int)"/>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public NetBuffer Write(ushort num)
        {
            return Write(BitConverter.GetBytes(num));
        }
        /// <summary>
        /// Writes a string to the buffer
        /// </summary>
        /// <param name="str"></param>
        public NetBuffer Write(string str)
        {
            return Write(str, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// <inheritdoc cref="Write(string)"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        public NetBuffer Write(string str, System.Text.Encoding encoding)
        {
            return Write(encoding.GetBytes(str));
        }

        #endregion

        /// <summary>
        /// Resets the write index
        /// </summary>
        public NetBuffer Reset()
        {
            writeIndex = 0;
            Resize(0);
            return this;
        }

        internal void Receive(Socket socket)
        {
            int size = socket.Receive(array, start, ArrayLength, SocketFlags.None);
            Resize(size);
            writeIndex = 0;
        }
        internal void Send(Socket socket)
        {           
            socket.Send(array, start, Length, SocketFlags.None);
            Reset();
        }
        internal void SendTo(Socket socket, EndPoint remote)
        {
            socket.SendTo(array, start, Length, SocketFlags.None, remote);
            Reset();
        }
        internal void ReceiveFrom(Socket socket, ref EndPoint remote)
        {
            int length = socket.ReceiveFrom(array, start, ArrayLength, SocketFlags.None, ref remote);
            Resize(length);
            writeIndex = 0;
        }
    }
}
