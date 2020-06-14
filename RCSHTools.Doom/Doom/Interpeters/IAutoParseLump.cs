namespace RCSHTools.Doom.Interpeters
{
    /// <summary>
    /// Allows auto parsing of lumps when the are modified by another interpeter
    /// </summary>
    public interface IAutoParseLump
    {
        /// <summary>
        /// Called when the lump is created, and when the lumps data is modified externally
        /// </summary>
        void Parse();
        /// <summary>
        /// Turns the parsed data into a byte array that can be written to a lump
        /// </summary>
        /// <returns></returns>
        byte[] ToRaw();
    }

    internal enum AutoParseLumpMethod
    {
        Parse,
        Raw
    }
}
