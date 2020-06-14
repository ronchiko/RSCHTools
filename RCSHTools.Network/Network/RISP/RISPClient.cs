using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace RCSHTools.Network.RISP
{
    /// <summary>
    /// A RISP protocol client
    /// </summary>
    public class RISPClient
    {

        /// <summary>
        /// Called when a request for source image is recieved
        /// </summary>
        public event OnRISPImageRequest OnRequestImage;

        private Socket socket;
        private EndPoint endpoint;
        private Queue<ulong> queue;
        private IndexCompressor compressor;

        /// <summary>
        /// Creates a new RISP client
        /// </summary>
        public RISPClient()
        {
            queue = new Queue<ulong>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Pairs between a client and a server
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Pair(string ip, int port)
        {
            compressor = null;
            endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket.Connect(endpoint);

            RISPImageStructure rispImage = OnRequestImage.Invoke(this);
            if (!rispImage.Validate)
            {
                throw new RispInvalidImageException();
            }

            NetBuffer buffer = new NetBuffer(512);
            socket.Send(buffer.Write(rispImage.width).Write(rispImage.height));
            socket.Receive(buffer);

            if(buffer.Length != 8)
            {
                throw new RispDimensionsTooLargeException();
            }
            socket.Send(buffer);

            socket.Receive(buffer);
            int code = buffer.GetInt(0);

            if (code == -2) throw new RispDimensionMismatchException();
            
            for (int i = 0; i < rispImage.data.Length; i++)
            {
                buffer.Write(rispImage.data[i]);
                if (buffer.Length >= 512) { 
                    socket.Send(buffer);
                    buffer.Reset();
                }
            }
            buffer.Write(0x01000000);
            socket.Send(buffer);

            socket.Receive(buffer);
            code = buffer.GetInt(0);
            if (code != 2) throw new RispPairDeclinedException();
            compressor = new IndexCompressor((uint)rispImage.width, (uint)rispImage.height, 0x01000000);
        }

        /// <summary>
        /// Attempt to pair this client to a <see cref="RISPServer"/>
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public RISPErrors TryPair(string ip, int port)
        {
            try
            {
                Pair(ip, port);
                return RISPErrors.Successful;
            }
            catch (RispInvalidImageException)
            {
                return RISPErrors.InvalidImageData;
            }
            catch (RispDimensionsTooLargeException)
            {
                return RISPErrors.ResolutionTooLarge;
            }
            catch (RispDimensionMismatchException)
            {
                return RISPErrors.DimensionMismatch;
            }
            catch (RispPairDeclinedException)
            {
                return RISPErrors.PairDeclined;
            }
            catch (SocketException)
            {
                return RISPErrors.SocketError;
            }
            catch (Exception)
            {
                return RISPErrors.Unknown;
            }
        }
        /// <summary>
        /// Updates a color in the image localy
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, RGB color)
        {
            SetPixel(x, y, (uint)color.ToInt());
        }
        /// <summary>
        /// <inheritdoc cref="SetPixel(int, int, RGB)"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rgb"></param>
        public void SetPixel(int x, int y, uint rgb)
        {
            if (compressor == null) throw new RispUnpairedClientException();
            queue.Enqueue(compressor.Compress((uint)x, (uint)y, rgb));
        }
        /// <summary>
        /// Sends all the recorded data to a paired server
        /// </summary>
        public void Stream()
        {
            int items = 0;
            NetBuffer buffer = new NetBuffer(1024);
            while(queue.Count > 0)
            {
                buffer.Write(queue.Dequeue());
                if (!buffer.CanWrite)
                {
                    socket.Send(buffer);
                    buffer.Reset();
                }
                items++;
            }
            socket.Send(buffer);
            Console.WriteLine("Streamed " + items + " updates");
        }
        /// <summary>
        /// Disconnects from a server
        /// </summary>
        public void Unpair()
        {
            socket.Send(BitConverter.GetBytes(ulong.MaxValue));
            socket.Disconnect(true);
            compressor = null;
        }
    }
}
