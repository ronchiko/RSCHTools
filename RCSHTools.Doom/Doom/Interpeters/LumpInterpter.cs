using System;
using System.Collections.Generic;
using RCSHTools.Doom.Interpeters;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Warps a lump and allows interaction with it
    /// </summary>
    public class LumpReader
    {
        /// <summary>
        /// The raw data of the lump
        /// </summary>
        public byte[] Raw { get; protected set; }

        /// <summary>
        /// The interpeted lump
        /// </summary>
        public Lump Lump { get; }

        /// <summary>
        /// Creates a new lump interpeter
        /// </summary>
        /// <param name="lump"></param>
        public LumpReader(Lump lump)
        {
            Lump = lump;
            lump.OnLumpSaved += Reload;
            Raw = Lump.CopyRaw();
            CallAutoParser(AutoParseLumpMethod.Parse);
        }

        /// <summary>
        /// Called when a lumps buffer pointer is changed
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="interpter"></param>
        private void Reload(in byte[] buffer, LumpReader interpter)
        {
            Raw = buffer;
            CallAutoParser(AutoParseLumpMethod.Parse);
        }
        /// <summary>
        /// Adds support for the IAutoParseLump interface
        /// </summary>
        private void CallAutoParser(AutoParseLumpMethod method)
        {
            if (typeof(IAutoParseLump).IsAssignableFrom(GetType()))
            {
                IAutoParseLump autoParse = (IAutoParseLump)this;
                switch (method)
                {
                    case AutoParseLumpMethod.Parse:
                        autoParse.Parse();
                        break;
                    case AutoParseLumpMethod.Raw:
                        Raw = autoParse.ToRaw();
                        break;
                }
            }
        }
        /// <summary>
        /// Saves the data
        /// </summary>
        public void Save()
        {
            CallAutoParser(AutoParseLumpMethod.Raw);
            Lump.Write(Raw, this);
        }
    }
}
