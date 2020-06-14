using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// Has global hashing functions
    /// </summary>
    public static class Hashes
    {
        /// <summary>
        /// Compute SHA512 hash on a string
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string Sha512(string msg)
        {
            return Sha512(Encoding.UTF8.GetBytes(msg));
        }
        /// <summary>
        /// Compute a SHA512 hash
        /// </summary>
        /// <returns></returns>
        public static string Sha512(byte[] msg)
        {
            // Initial values
            unchecked {
                long[] h =
                {
                0x6a09e667f3bcc908, (long)0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, (long)0xa54ff53a5f1d36f1,
                0x510e527fade682d1, (long)0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179
                };

                long[] k =
                {
                  0x428a2f98d728ae22, 0x7137449123ef65cd, (long)0xb5c0fbcfec4d3b2f, (long)0xe9b5dba58189dbbc, 0x3956c25bf348b538,
                  0x59f111f1b605d019, (long)0x923f82a4af194f9b, (long)0xab1c5ed5da6d8118, (long)0xd807aa98a3030242, 0x12835b0145706fbe,
                  0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2, 0x72be5d74f27b896f, (long)0x80deb1fe3b1696b1, (long)0x9bdc06a725c71235,
                  (long)0xc19bf174cf692694, (long)0xe49b69c19ef14ad2, (long)0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
                  0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5, (long)0x983e5152ee66dfab,
                  (long)0xa831c66d2db43210, (long)0xb00327c898fb213f, (long)0xbf597fc7beef0ee4, (long)0xc6e00bf33da88fc2, (long)0xd5a79147930aa725,
                  0x06ca6351e003826f, 0x142929670a0e6e70, 0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed,
                  0x53380d139d95b3df, 0x650a73548baf63de, 0x766a0abb3c77b2a8, (long)0x81c2c92e47edaee6, (long)0x92722c851482353b,
                  (long)0xa2bfe8a14cf10364, (long)0xa81a664bbc423001, (long)0xc24b8b70d0f89791, (long)0xc76c51a30654be30, (long)0xd192e819d6ef5218,
                  (long)0xd69906245565a910, (long)0xf40e35855771202a, 0x106aa07032bbd1b8, 0x19a4c116b8d2d0c8, 0x1e376c085141ab53,
                  0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8, 0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373,
                  0x682e6ff3d6b2b8a3, 0x748f82ee5defb2fc, 0x78a5636f43172f60, (long)0x84c87814a1f0ab72, (long)0x8cc702081a6439ec,
                  (long)0x90befffa23631e28, (long)0xa4506cebde82bde9, (long)0xbef9a3f7b2c67915, (long)0xc67178f2e372532b,(long) 0xca273eceea26619c,
                 (long) 0xd186b8c721c0c207, (long)0xeada7dd6cde0eb1e, (long)0xf57d4f7fee6ed178, 0x06f067aa72176fba, 0x0a637dc5a2c898a6,
                  0x113f9804bef90dae, 0x1b710b35131c471b, 0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc,
                  0x431d67c49c100d4c, 0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
                };


                msg = ShaPad(msg);
                long[,] chunks = ShaBreak(msg);
                long[,] message = ComputeMessage(chunks);

                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    long a = h[0];
                    long b = h[1];
                    long c = h[2];
                    long d = h[3];
                    long e = h[4];
                    long f = h[5];
                    long g = h[6];
                    long _h = h[7];

                    for(int j = 0; j < 80; j++)
                    {
                        long t1 = _h + MathR.Sigma1(e) + MathR.Ch(e, f, g) + k[j] + message[i, j];
                        long t2 = MathR.Sigma0(a) + MathR.Maj(a, b, c);
                        _h = g;
                        g = f;
                        f = e;
                        e = d + t1;
                        d = c;
                        c = b;
                        b = a;
                        a = t1 + t2;
                    }

                    h[0] += a;
                    h[1] += b;
                    h[2] += c;
                    h[3] += d;
                    h[4] += e;
                    h[5] += f;
                    h[6] += g;
                    h[7] += _h;
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(h[i].ToString("X"));
                }

                return sb.ToString();
            }

           
        }

        private static byte[] ShaPad(byte[] message)
        {
            // Extend until multiple of 128
            int size = message.Length + 17;
            while (size % 128 != 0)
                size ++;

            byte[] output = new byte[size];

            // Copy old array
            for (int i = 0; i < message.Length; i++)
                output[i] = message[i];

            // Add the 1 value
            output[message.Length] = 0x80;

            byte[] lenInBytes = BitConverter.GetBytes((long)(message.Length * 8));

            for(int i = lenInBytes.Length; i > 0; i--)
            {
                output[size - i] = lenInBytes[lenInBytes.Length - i];
            }

            return output;
        }
    
        private static long[,] ShaBreak(byte[] message)
        {
            long[,] broken = new long[message.Length / 128, 16];
            
            for(int i = 0; i < broken.GetLength(0); i++)
            {
                for (int j = 0;j < 16; j++)
                {
                    broken[i, j] = BitConverter.ToInt64(message, i * 128 + j * 8);
                }
            }

            return broken;
        }
    
        private static long[,] ComputeMessage(long[,] msg)
        {
            long[,] W = new long[msg.GetLength(0), 80];

            for(int i = 0; i < W.GetLength(0); i++)
            {
                // Copy
                for (int j = 0; j < 16; j++)
                {
                    W[i, j] = msg[i, j];
                }

                for(int j = 16; j < 80; j++)
                {
                    W[i, j] = _Sigma1(W[i, j - 2]) + W[i, j - 7] + _Sigma0(W[i, j - 15]) + W[i, j - 16];
                }
            }

            return W;
        }

        // Used in the message schedule
        private static long _Sigma0(long x)
        {
            // S1(x) ^ S8(x) ^ R7(x)
            return MathR.Rotate(x, 1) ^ MathR.Rotate(x, 8) ^ (x >> 7);
        }

        // Used in the message schedule
        private static long _Sigma1(long x)
        {
            // S19(x) ^ S61(x) ^ R6(x)
            return MathR.Rotate(x, 19) ^ MathR.Rotate(x, 61) ^ (x >> 6);
        }
    }
}
