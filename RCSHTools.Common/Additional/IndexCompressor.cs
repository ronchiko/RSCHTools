using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools
{
    /// <summary>
    /// Compresses an array of values to an unsigned long.
    /// The maximum values of each element must be determent, and the values must be greater then or equal to 0.
    /// </summary>
    public class IndexCompressor
    {
        private uint[] dimensions;
        private int size;

        /// <summary>
        /// The maximum value this compressor can reach
        /// </summary>
        public ulong MaxValue
        {
            get
            {
                uint[] values = new uint[dimensions.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = dimensions[i] - 1;
                }
                return Compress(values);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimensionsSizes">The maximum size of each dimension</param>
        public IndexCompressor(params uint[] dimensionsSizes)
        {
            dimensions = dimensionsSizes;
            size = dimensionsSizes.Length;
        }

        /// <summary>
        /// Compresses an array of values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public ulong Compress(params uint[] values)
        {
            if (values.Length != size) throw new Exception("There needs to be excatly " + size + " values");
            return Compress(values, 0);
        }

        private ulong Dimension(int index)
        {
            if (index == 0) return 1;
            return dimensions[index - 1];
        }
        private ulong Compress(uint[] values, int index)
        {
            if (index == values.Length - 1) return values[index];
            return values[index] + Dimension(index + 1) * Compress(values, index + 1);
        }

        /// <summary>
        /// Returns an array of values out of a compressed value
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        public uint[] Decompress(in ulong compressed)
        {
            uint[] outputs = new uint[size];
            Decompress(compressed, outputs, 0);
            return outputs;
        }
        /// <summary>
        /// The compress a value into a <see cref="IIndexable"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indexable"></param>
        public void Decompress(in ulong value, ref IIndexable indexable)
        {
            indexable.Write(Decompress(value));
        }

        private void Decompress(ulong compressed, uint[] outputs, int index)
        {
            if (index >= outputs.Length) return;
            outputs[index] = (uint)(compressed % Dimension(index + 1));
            Decompress(compressed / Dimension(index+ 1), outputs, index + 1);
        }
    }
}
