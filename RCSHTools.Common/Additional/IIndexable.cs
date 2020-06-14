namespace RCSHTools
{
    /// <summary>
    /// A structure that can be compressed an decompressed with an <see cref="IndexCompressor"/>
    /// </summary>
    public interface IIndexable
    {
        /// <summary>
        /// Reads the items from the structure
        /// </summary>
        /// <returns></returns>
        uint[] Read();
        /// <summary>
        /// Writes to the structure
        /// </summary>
        /// <param name="data"></param>
        void Write(uint[] data);
    }
}
