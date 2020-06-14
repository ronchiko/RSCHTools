using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represents directory markers
    /// </summary>
    public enum DirectoryMarkers
    {
        /// <summary>
        /// (TX_START -> TX_END) Texture markers used with ZDoom engine
        /// </summary>
        TexturesZDoom = 0,
        /// <summary>
        /// (S_START -> S_END) Sprites
        /// </summary>
        Sprites = 1,
        /// <summary>
        /// (SS_START -> SS_END) Sprites on user defined wads
        /// </summary>
        SpritesUserDefined = 2,
        /// <summary>
        /// (HI_START -> HI_END) Scaled Texture used with ZDoom engine
        /// </summary>
        TexturesScaledZDoom = 3,
        /// <summary>
        /// (F_START -> F_END) Flats
        /// </summary>
        Flats = 4,
        /// <summary>
        /// (F1_START -> F1_END) Flats used with heretic and hexen
        /// </summary>
        FlatsShareware = 5,
        /// <summary>
        /// (F2_START -> F2_END) Flats used with Heretic and Hexen
        /// </summary>
        FlatsRegistered = 6,
        /// <summary>
        /// (FF_START -> FF_END) Flats in user defined wads
        /// </summary>
        FlatsUserDefined = 7,
        /// <summary>
        /// (P_START -> P_END) Patches, ZDoom ignores this section 
        /// </summary>
        Patches = 8,
        /// <summary>
        /// (P1_START -> P1_END) Patches used in Heretic and Hexen
        /// </summary>
        PatchesShareware = 9,
        /// <summary>
        /// (P2_START -> P2_END) Patches used in Heretic and Hexen
        /// </summary>
        PatchesRegistered = 10,
        /// <summary>
        /// (P3_START -> P3_END) Patches used in Doom 2
        /// </summary>
        PatchesDoom2 = 11,
    }
}
