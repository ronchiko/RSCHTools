using System;
using System.Collections.Generic;
using System.IO;

namespace RCSHTools.Doom.Maps
{
    public interface IMapItem
    {
        void Write(MemoryStream stream, SpecificationMode spec);
    }
}
