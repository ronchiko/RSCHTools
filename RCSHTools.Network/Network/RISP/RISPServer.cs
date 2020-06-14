using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RCSHTools.Network.RISP
{
    /// <summary>
    /// Repsentes a Risp Server that receiver an image
    /// </summary>
    public class RISPServer
    {
        #region Protocol Constants
        private const int DIMENSIONS_TOO_LARGE = -1; // The sent dimensions are too large
        private const int DIMENSIONS_MISMATCH = -2;  // The verfied dimensions don't match

        private const int REQUEST_IMAGE = 1;         // The connection is set, needs the current image
        private const int END_OF_IMAGE = 0x01000000; // Not the end of the image
        private const int PAIR_SUCCESSFUL = 2;       // Not the end of the image

        private const ulong TERMINATE_CONNECTION = ulong.MaxValue;  // Value to terminate the connection
        #endregion

        private const int MAX_DIMENSION_SIZE = 8192;

        private EndPoint endpoint;
        private Socket server;
        private int bufferSize;

        /// <summary>
        /// Called when a new connection is astablished
        /// </summary>
        public event OnRISPImageInit OnConnectionAstablished;
        /// <summary>
        /// Called when the stream is updated
        /// </summary>
        public event OnRISPImageUpdate OnStreamUpdate;

        /// <summary>
        /// Creates a new RITP server
        /// </summary>
        public RISPServer(string ip, int port, int bufferSize = 1024)
        {
            this.bufferSize = bufferSize;
            endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endpoint);
        }

        /// <summary>
        /// Starts the server as a thread
        /// </summary>
        /// <returns></returns>
        public CancellationTokenSource Run()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Start), cts.Token);
            return cts;
        }

        /// <summary>
        /// Runs the server
        /// </summary>
        public void Start(object state)
        {   
            NetBuffer buffer = new NetBuffer(bufferSize);
            CancellationToken cts = (CancellationToken)state;

            while (true)
            {
                // Listen for a new connection
                server.Listen(1);
                using (Socket socket = server.Accept())
                {
                    // Attempt pairing with the new client
                    socket.Receive(buffer);
                    // Receive the dimensions of the image
                    uint requestedWidth = buffer.GetUInt(0);
                    uint requestedHeight = buffer.GetUInt(4);

                    // Dimesions are too large
                    if (requestedHeight > MAX_DIMENSION_SIZE || requestedWidth > MAX_DIMENSION_SIZE)
                    {
                        socket.Send(buffer.Reset().Write(DIMENSIONS_TOO_LARGE));
                        continue;
                    }

                    // Sends the dimensions to client to verify
                    socket.Send(buffer);
                    socket.Receive(buffer);
                    // Dimensions mismatch
                    if (requestedHeight != buffer.GetUInt(4) || requestedWidth != buffer.GetUInt(0))
                    {
                        socket.Send(buffer.Write(DIMENSIONS_MISMATCH));
                        continue;
                    }

                    StreamImage image = new StreamImage((int)requestedWidth, (int)requestedHeight);
                    IndexCompressor compressor = new IndexCompressor(requestedWidth, requestedHeight, 0x01000000);

                    // Initialize image
                    socket.Send(buffer.Reset().Write(REQUEST_IMAGE));

                    int x = 0;
                    int y = 0;
                    bool ended = false;

                    do
                    {
                        socket.Receive(buffer);

                        int i = 0;
                        while(i < buffer.Length - sizeof(int))
                        {
                            image.SetPixel(x, y, buffer.GetInt(i));
                            i += sizeof(int);
                            x++;
                            if(x >= requestedWidth)
                            {
                                x = 0;
                                y++;
                            }
                        }

                        if(buffer.GetUInt(i) == END_OF_IMAGE)
                        {
                            ended = true;
                        }
                        else
                        {
                            image.SetPixel(x, y, buffer.GetInt(i));
                            x++;
                            if (x >= requestedWidth)
                            {
                                x = 0;
                                y++;
                            }
                        }

                    } while (!ended);
                    OnConnectionAstablished?.Invoke(image.Width, image.Height, image.image, this);
                    socket.Send(buffer.Reset().Write(PAIR_SUCCESSFUL));

                    IIndexable pixel = new RISPImagePixel();
                    // Start streaming loop
                    while (true)
                    {
                        try
                        {
                            socket.Receive(buffer);

                            if (buffer.GetULong(0) == TERMINATE_CONNECTION) break;

                            int index = 0;
                            RISPImagePixel[] diffrences = new RISPImagePixel[buffer.Length / sizeof(ulong)];
                            while (index < buffer.Length)
                            {
                                ulong compressedValue = buffer.GetULong(index);
                                compressor.Decompress(compressedValue, ref pixel);

                                RISPImagePixel iPixel = (RISPImagePixel)pixel;
                                System.Console.WriteLine(iPixel.color.ToString("X"));
                                diffrences[index / sizeof(ulong)] = iPixel;
                                image.SetPixel((int)iPixel.x, (int)iPixel.y, new RGB((int)iPixel.color));

                                index += sizeof(ulong);
                            }
                            // Call stream updates
                            if (diffrences.Length > 0) OnStreamUpdate?.Invoke(diffrences, this);
                        }
                        catch (SocketException)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
