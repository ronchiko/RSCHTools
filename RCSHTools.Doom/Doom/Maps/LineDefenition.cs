using System;
using System.Collections.Generic;
using System.IO;

namespace RCSHTools.Doom.Maps
{
    /// <summary>
    /// Line defenition information.
    /// More info: https://zdoom.org/wiki/Linedef
    /// </summary>
    public struct LineDefenition : IMapItem
    {
        private const int BLOCKING = 1 << 0;
        private const int BLOCKMONSTERS = 1 << 1;
        private const int TWOSIDED = 1 << 2;
        private const int UNPEGTOP = 1 << 3;
        private const int UNPEGBOTTOM = 1 << 4;
        private const int SECRET = 1 << 5;
        private const int BLOCKSOUND = 1 << 6;
        private const int DONTDRAW = 1 << 6;
        private const int MAPPED = 1 << 6;
        private const int RAILING = 1 << 6;

        /// <summary>
        /// Value for no side def
        /// </summary>
        public const ushort NO_SIDEDEF = 0xFFFF;

        private byte[] actionArguments;

        /// <summary>
        /// The index of the start vertex of the line (v1 in UDMF)
        /// </summary>
        public ushort StartIndex { get; set; }
        /// <summary>
        /// The index of the end vertex of the line (v2 in UDMF)
        /// </summary>
        public ushort EndIndex { get; set; }

        /// <summary>
        /// The flags of this line
        /// </summary>
        public List<LineDefenitionFlags> Flags { get; set; }
        /// <summary>
        /// <b>Only in doom map format</b>, The type of the line, not used in Hexen format
        /// </summary>
        public ushort Type { get; set; }
        /// <summary>
        /// <b>Only in doom map format</b>, The tag of the sector this line is part of
        /// </summary>
        public ushort Sector { get; set; }
        #region Hexen Exclusive
        // Parameters here are only used in hexen map format
        /// <summary>
        /// <b>Only in hexen map format</b>, The action special of this line 
        /// </summary>
        public byte ActionSpecial { get; set; }
        /// <summary>
        /// <b>Only in hexen map format</b>, The arguements for the action special , up to 5 arguments
        /// See: https://zdoom.org/wiki/Action_specials
        /// </summary>
        public byte[] ActionArguments
        {
            get => actionArguments;
            set
            {
                if (value.Length > 5)
                    throw new Exception("Arguments can be up to 5 item long");
            }
        }
        #endregion

       
        /// <summary>
        /// Index of the right side defenition
        /// </summary>
        public ushort RightSideDef { get; set; }
        /// <summary>
        /// Index of the left side defenition
        /// </summary>
        public ushort LeftSideDef { get; set; }

        /// <summary>
        /// Opens a map line defenition in a Doom or Hexen format
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="specifiactions"></param>
        public LineDefenition(MemoryStream stream, SpecificationMode specifiactions)
        {
            Flags = new List<LineDefenitionFlags>();
            byte[] buffer;
            ushort rawFlags;
            switch (specifiactions)
            {
                case SpecificationMode.Doom:
                    buffer = new byte[14];
                    stream.Read(buffer, 0, 14);
                    StartIndex = BitConverter.ToUInt16(buffer, 0);
                    EndIndex = BitConverter.ToUInt16(buffer, 2);
                    rawFlags = BitConverter.ToUInt16(buffer, 4);
                    AddFlags(Flags, rawFlags);
                    Type = BitConverter.ToUInt16(buffer, 6);
                    Sector = BitConverter.ToUInt16(buffer, 8);
                    RightSideDef = BitConverter.ToUInt16(buffer, 10);
                    LeftSideDef = BitConverter.ToUInt16(buffer, 12);

                    ActionSpecial = 0;
                    actionArguments = new byte[0];
                    break;
                case SpecificationMode.Hexen:
                    buffer = new byte[16];
                    stream.Read(buffer, 0, 16);
                    StartIndex = BitConverter.ToUInt16(buffer, 0);
                    EndIndex = BitConverter.ToUInt16(buffer, 2);
                    rawFlags = BitConverter.ToUInt16(buffer, 4);
                    AddFlags(Flags, rawFlags);
                    ActionSpecial = buffer[6];
                    actionArguments = new byte[5];
                    Array.Copy(buffer, 7, actionArguments, 0, 5);                    
                    RightSideDef = BitConverter.ToUInt16(buffer, 12);
                    LeftSideDef = BitConverter.ToUInt16(buffer, 14);

                    Sector = 0;
                    Type = 0;
                    break;
                case SpecificationMode.UDMF:
                    throw new NotImplementedException();
                default:
                    throw new Exception("Map format not supported");
            }
        }

        /// <summary>
        /// Does this line has a left side def
        /// </summary>
        /// <returns></returns>
        public bool HasLeft() => LeftSideDef != NO_SIDEDEF;
        /// <summary>
        /// Does this line has a right side def
        /// </summary>
        /// <returns></returns>
        public bool HasRight() => RightSideDef != NO_SIDEDEF;

        void IMapItem.Write(MemoryStream stream, SpecificationMode spec)
        {
            switch (spec)
            {
                case SpecificationMode.Doom:
                    stream.Write(BitConverter.GetBytes(StartIndex), 0, 2);
                    stream.Write(BitConverter.GetBytes(EndIndex), 0, 2);
                    stream.Write(BitConverter.GetBytes(GetFlags()), 0, 2);
                    stream.Write(BitConverter.GetBytes(Type), 0, 2);
                    stream.Write(BitConverter.GetBytes(Sector), 0, 2);
                    stream.Write(BitConverter.GetBytes(RightSideDef), 0, 2);
                    stream.Write(BitConverter.GetBytes(LeftSideDef), 0, 2);
                    break;
                case SpecificationMode.Hexen:
                    stream.Write(BitConverter.GetBytes(StartIndex), 0, 2);
                    stream.Write(BitConverter.GetBytes(EndIndex), 0, 2);
                    stream.Write(BitConverter.GetBytes(GetFlags()), 0, 2);
                    stream.WriteByte(ActionSpecial);
                    int fill = 5 - ActionArguments.Length;
                    stream.Write(ActionArguments, 0, ActionArguments.Length);
                    for(int i = 0; i < fill; i++)
                    {
                        stream.WriteByte(0);
                    }
                    stream.Write(BitConverter.GetBytes(RightSideDef), 0, 2);
                    stream.Write(BitConverter.GetBytes(LeftSideDef), 0, 2);
                    break;
                case SpecificationMode.UDMF:
                default:
                    throw new NotImplementedException();
            }
        }

        private static void AddFlags(List<LineDefenitionFlags> Flags, ushort flags)
        {
            Flags.Clear();
            if ((flags & BLOCKING) != 0)
                Flags.Add(LineDefenitionFlags.Blocking);
            if ((flags & BLOCKMONSTERS) != 0)
                Flags.Add(LineDefenitionFlags.BlockMonsters);
            if((flags & TWOSIDED) != 0)
                Flags.Add(LineDefenitionFlags.TwoSide);
            if ((flags & UNPEGTOP) != 0)
                Flags.Add(LineDefenitionFlags.DontPegTop);
            if ((flags & UNPEGBOTTOM) != 0)
                Flags.Add(LineDefenitionFlags.DontPegBottom);
            if ((flags & SECRET) != 0)
                Flags.Add(LineDefenitionFlags.Secret);
            if ((flags & BLOCKSOUND) != 0)
                Flags.Add(LineDefenitionFlags.SoundBlock);
            if ((flags & DONTDRAW) != 0)
                Flags.Add(LineDefenitionFlags.DontDraw);
            if ((flags & MAPPED) != 0)
                Flags.Add(LineDefenitionFlags.Mappped);
            if ((flags & RAILING) != 0)
                Flags.Add(LineDefenitionFlags.Railing);
        }
        private ushort GetFlags()
        {
            ushort raw = 0;
            foreach (var flag in Flags)
            {
                if(flag < 0)
                {
                    continue;
                }
                raw |= (ushort)flag;
            }
            return raw;
        }
    }

    /// <summary>
    /// <see cref="LineDefenition"/> Flags
    /// </summary>
    public enum LineDefenitionFlags
    {
        /// <summary>
        /// Blocks players and monsters
        /// </summary>
        Blocking = 0x0001,
        /// <summary>
        /// Blocks only monsters
        /// </summary>
        BlockMonsters = 0x0002,
        /// <summary>
        /// Is the wall 2 sided
        /// </summary>
        TwoSide = 0x0004,
        /// <summary>
        /// The wall has an unpegged top
        /// </summary>
        DontPegTop = 0x0008,
        /// <summary>
        /// The wall has an unpegged bottom
        /// </summary>
        DontPegBottom = 0x0010,
        /// <summary>
        /// The wall is a secret
        /// </summary>
        Secret = 0x0020,
        /// <summary>
        /// The wall blocks sound
        /// </summary>
        SoundBlock = 0x0040,
        /// <summary>
        /// The wall will never appear on the automap
        /// </summary>
        DontDraw = 0x0080,
        /// <summary>
        /// The wall always appear on the automap
        /// </summary>
        Mappped = 0x0100,
        /// <summary>
        /// Line is railing (0x0200)
        /// </summary>
        Railing,
        /// <summary>
        /// Passes use action (0x0200)
        /// </summary>
        PassUse,
        /// <summary>
        /// Can be activated more that once (0x0200)
        /// </summary>
        RepeatSpecial = 0x0200
    }

    /// <summary>
    /// Represents map specifications in differnt games
    /// </summary>
    public enum SpecificationMode
    {
        /// <summary>
        /// Doom map specifiactions
        /// </summary>
        Doom,
        /// <summary>
        /// Hexen map specifiaction
        /// </summary>
        Hexen,
        /// <summary>
        /// UDMF map specifiaction, ussually in text
        /// </summary>
        UDMF,
    }

    namespace UDMF
    {
        /// <summary>
        /// <see cref="LineDefenition"/> properties on udmf maps.
        /// </summary>
        public enum UDMFLineDefenitionProperties
        {
            /// <summary>
            /// The translucency of the line defaults to 1 (alpha:<see cref="float"/>)
            /// </summary>
            Alpha,
            /// <summary>
            /// Alternate string version of arg0 used for named scripts (arg0str:<see cref="string"/>)
            /// </summary>
            Arguemnt0String,
            /// <summary>
            /// The first argument (arg0:<see cref="int"/>)
            /// </summary>
            Argument0,
            /// <summary>
            /// The second argument (arg1:<see cref="int"/>)
            /// </summary>
            Argument1,
            /// <summary>
            /// The third argument (arg2:<see cref="int"/>)
            /// </summary>
            Argument2,
            /// <summary>
            /// The fourth argument (arg3:<see cref="int"/>)
            /// </summary>
            Argument3,
            /// <summary>
            /// The fifth argument (arg4:<see cref="int"/>)
            /// </summary>
            Argument4,
            /// <summary>
            /// A comment used by the authors, ignored by the engines (comment:<see cref="string"/>)
            /// </summary>
            Comment,
            /// <summary>
            /// Line special is locked (locknumber:<see cref="int"/>)
            /// </summary>
            LockNumber
        }
    }
}
