namespace RCSHTools
{
    /// <summary>
    /// Provides masking support
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMask<T>
    {
        /// <summary>
        /// The length of the mask
        /// </summary>
        int Length { get; }
        /// <summary>
        /// Get the item in an index relative to the masks start
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[int index] { get; }
        /// <summary>
        /// Creates a sub-mask of this mask
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IMask<T> Mask(int from, int to);
    }
}
