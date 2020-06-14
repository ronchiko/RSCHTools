using System;
using System.Drawing;
using System.IO;

namespace RCSHTools.Doom.Interpeters
{
    public class TextureInterpeter : LumpReader
    {
        private bool compressed;
        
        public byte[] Binary { get; }

        public TextureInterpeter(Lump lump) : base(lump)
        {
            if((lump.Name[0] & 0x80) != 0)
            {
                compressed = true;
                throw new NotImplementedException();
            }
            else
            {
                Binary = Raw;
            }
        }
    }
}
