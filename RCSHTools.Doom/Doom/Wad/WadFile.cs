using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using RCSHTools.Doom.Interpeters;

namespace RCSHTools.Doom
{
    /// <summary>
    /// Represents a .wad file
    /// </summary>
    public class WadFile
    {
        /// <summary>
        /// A collection of texture inside the wad
        /// </summary>
        public class WadTexturePool
        {
            private WadFile wad;
            private Dictionary<string, Graphic> textures;
            private Dictionary<string, MapTexture> mapTextures;

            /// <summary>
            /// Creates a new empty texture pool
            /// </summary>
            /// <param name="wad"></param>
            public WadTexturePool(WadFile wad)
            {
                this.wad = wad;
                textures = new Dictionary<string, Graphic>();
                mapTextures = new Dictionary<string, MapTexture>();
            }
            /// <summary>
            /// Creates a new texture pool
            /// </summary>
            /// <param name="wad"></param>
            /// <param name="texture1"></param>
            /// <param name="pnames"></param>
            public WadTexturePool(WadFile wad, Lump texture1, Lump pnames)
            {
                this.wad = wad;
                int count = pnames.ReadInt32(0);
                textures = new Dictionary<string, Graphic>(count);

                for (int i = 0; i < count; i++)
                {
                    string name = pnames.ReadString(i * 8 + 4, 8);
                    int index = name.IndexOf('\0');
                    if(index != -1)
                    {
                        name = name.Remove(index);
                    }

                    textures.Add(name, new Graphic(wad, name));
                }


                count = texture1.ReadInt32(0);
                mapTextures = new Dictionary<string, MapTexture>(count);

                using (MemoryStream stream = texture1.GetByteStream())
                {
                    for(int i = 0; i < count;i++)
                    {
                        long position = stream.Position;
                        stream.Position = texture1.ReadInt32(4 + i * 4);
                        MapTexture texture = new MapTexture(this, stream);
                        mapTextures.Add(texture.Name, texture);
                        stream.Position = position;
                    }
                }
            }
        
            /// <summary>
            /// Gets a texture lump from the pool
            /// </summary>
            /// <param name="lump"></param>
            /// <returns></returns>
            public Graphic GetTextureLump(string lump)
            {
                if (!textures.ContainsKey(lump)) throw new Exception("No texture " + lump + " in pool");

                Graphic textureLump = textures[lump];
                if(textures == null)
                {
                    textureLump = new Graphic(wad, lump);
                    textures[lump] = textureLump;
                }
                return textureLump;
            }
            /// <summary>
            /// <inheritdoc cref="GetTextureLump(string)"/>
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Graphic GetTextureLump(int index)
            {
                KeyValuePair<string, Graphic> kvPair = textures.ElementAt(index);
                Graphic textureLump = kvPair.Value;
                if (textures == null)
                {
                    textureLump = new Graphic(wad, kvPair.Key);
                    textures[kvPair.Key] = textureLump;
                }
                return textureLump;
            }
            /// <summary>
            /// Gets a <see cref="MapTexture"/> from the pool
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public MapTexture GetMapTexture(string name)
            {
                if (!mapTextures.ContainsKey(name)) throw new Exception("No map texture " + name + " found in pool");

                return mapTextures[name];
            }
            /// <summary>
            /// Adds a texture to the pool
            /// </summary>
            /// <param name="name"></param>
            /// <param name="lump"></param>
            public void AddTexture(string name, string lump)
            {
                if (wad.readMode == WadReadMode.Readonly) throw new Exception("This wad file cannot be modified");

                if (textures.ContainsKey(name)) throw new Exception("This pool already has a texture named " + name);

                textures.Add(name, new Graphic(wad, lump));
            }

            /// <summary>
            /// Gets a texture by its name
            /// </summary>
            /// <param name="texture"></param>
            /// <returns></returns>
            public Bitmap this[string texture]
            {
                get => textures[texture].LoadImage();
            }
            /// <summary>
            /// Gets a texture by its index
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Bitmap this[int index]
            {
                get => textures.ElementAt(index).Value.LoadImage();
            }

        }
        /// <summary>
        /// Reprensts a .Wad file header
        /// </summary>
        private struct WadHeader
        {
            public WadType Type { get; }
            public uint Directories { get; private set; }
            public uint DirectoryPointer { get; private set; }

            public WadHeader(WadType type, uint directories, uint direactoryStart)
            {
                Type = type;
                Directories = directories;
                DirectoryPointer = direactoryStart;
            }
        }
        /// <summary>
        /// Represents a marker range
        /// </summary>
        public struct LumpPtrRange
        {
            public uint Start { get; }
            public uint End { get; }

            public LumpPtrRange(uint start, uint end)
            {
                Start = start;
                End = end;
            }
        }

        private const uint HEADER_SIZE = 12;
        private const uint LUMP_POINTER_SIZE = 16;
        private static readonly Dictionary<string, DirectoryMarkers> START_MARKERS = new Dictionary<string, DirectoryMarkers>()
        {
            {"TX_START", DirectoryMarkers.TexturesZDoom},
            {"HI_START", DirectoryMarkers.TexturesScaledZDoom},
            {"S_START", DirectoryMarkers.Sprites},
            {"SS_START", DirectoryMarkers.SpritesUserDefined},
            {"P_START", DirectoryMarkers.Patches},
            {"P1_START", DirectoryMarkers.PatchesShareware},
            {"P2_START", DirectoryMarkers.PatchesRegistered},
            {"P3_START", DirectoryMarkers.PatchesDoom2},
            {"F_START", DirectoryMarkers.Flats},
            {"FF_START", DirectoryMarkers.FlatsUserDefined},
            {"F1_START", DirectoryMarkers.FlatsShareware},
            {"F2_START", DirectoryMarkers.FlatsRegistered},
        };
        private static readonly Dictionary<DirectoryMarkers, string> MARKERS_START = new Dictionary<DirectoryMarkers, string>()
        {
            {DirectoryMarkers.TexturesZDoom, "TX_START"},
            {DirectoryMarkers.TexturesScaledZDoom, "HI_START"},
            {DirectoryMarkers.Sprites, "S_START"},
            {DirectoryMarkers.SpritesUserDefined, "SS_START"},
            {DirectoryMarkers.Patches, "P_START"},
            {DirectoryMarkers.PatchesShareware, "P1_START"},
            {DirectoryMarkers.PatchesRegistered, "P2_START"},
            {DirectoryMarkers.PatchesDoom2, "P3_START"},
            {DirectoryMarkers.Flats, "F_START"},
            {DirectoryMarkers.FlatsUserDefined, "FF_START"},
            {DirectoryMarkers.FlatsShareware, "F1_START"},
            {DirectoryMarkers.FlatsRegistered, "F2_START"},
        };
        private static readonly Dictionary<string, DirectoryMarkers> END_MARKERS = new Dictionary<string, DirectoryMarkers>()
        {
            {"TX_END", DirectoryMarkers.TexturesZDoom},
            {"HI_END", DirectoryMarkers.TexturesScaledZDoom},
            {"S_END", DirectoryMarkers.Sprites},
            {"SS_END", DirectoryMarkers.SpritesUserDefined},
            {"P_END", DirectoryMarkers.Patches},
            {"P1_END", DirectoryMarkers.PatchesShareware},
            {"P2_END", DirectoryMarkers.PatchesRegistered},
            {"P3_END", DirectoryMarkers.PatchesDoom2},
            {"F_END", DirectoryMarkers.Flats},
            {"FF_END", DirectoryMarkers.FlatsUserDefined},
            {"F1_END", DirectoryMarkers.FlatsShareware},
            {"F2_END", DirectoryMarkers.FlatsRegistered},
        };
        private static readonly Dictionary<DirectoryMarkers, string> MARKERS_END = new Dictionary<DirectoryMarkers, string>()
        {
            {DirectoryMarkers.TexturesZDoom, "TX_END"},
            {DirectoryMarkers.TexturesScaledZDoom, "HI_END"},
            {DirectoryMarkers.Sprites, "S_END"},
            {DirectoryMarkers.SpritesUserDefined, "SS_END"},
            {DirectoryMarkers.Patches, "P_END"},
            {DirectoryMarkers.PatchesShareware, "P1_END"},
            {DirectoryMarkers.PatchesRegistered, "P2_END"},
            {DirectoryMarkers.PatchesDoom2, "P3_END"},
            {DirectoryMarkers.Flats, "F_END"},
            {DirectoryMarkers.FlatsUserDefined, "FF_END"},
            {DirectoryMarkers.FlatsShareware, "F1_END"},
            {DirectoryMarkers.FlatsRegistered, "F2_END"},
        };

        private byte[] contents;            // Constant read from file
        private WadHeader header;           // The header of .wad file
        private string path;                // Path to file
        private Dictionary<DirectoryMarkers, LumpPtrRange> ranges;
        private WadTexturePool texturePool;    // the texture pool for the wad
        private readonly List<Lump> real;            // Lumps ordered by how they are ordered in the lumps segement (Iterative insertion)
        private readonly List<LumpPtr> directories;  // Lumps ordered by how the directories are ordered (sorted insertion)
        private readonly WadReadMode readMode; // can the wad be modified
        private readonly Stack<WadFile> patches;
        private DoomPalleteCollection palletes;

        /// <summary>
        /// The type of the wad file
        /// </summary>
        public WadType Type => header.Type;
        /// <summary>
        /// The amount of lumps in the file
        /// </summary>
        public int Items => (int)header.Directories;
        /// <summary>
        /// Does this wad has a texture pool (Generally IWad should have one, but PWad might not)
        /// </summary>
        public bool HasTexturePool => texturePool != null;
        /// <summary>
        /// The texture pool of this wad (not all wads have one)
        /// </summary>
        public WadTexturePool TexturePool => texturePool;


        /// <summary>
        /// Loads a <see cref="Lump"/>  by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Lump this[string name]
        {
            get => real.Find((l) => name == l.Name);
        }
        /// <summary>
        /// Loads a <see cref="Lump"/> by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Lump this[int index]
        {
            get => directories[index].Lump;
        }

        #region Constructors
        /// <summary>
        /// Creates a new writable empty pwad file
        /// </summary>
        public WadFile() : this(WadType.PWad)
        {

        }
        /// <summary>
        /// Creates a new writable emprty wad file with a given type
        /// </summary>
        /// <param name="type"></param>
        public WadFile(WadType type)
        {
            patches = new Stack<WadFile>();
            ranges = new Dictionary<DirectoryMarkers, LumpPtrRange>();
            real = new List<Lump>();
            directories = new List<LumpPtr>();
            readMode = WadReadMode.Writable;
            header = new WadHeader(type, 0, 0);
        }
        /// <summary>
        /// Creates a new extendable wad file from path
        /// </summary>
        /// <param name="path"></param>
        public WadFile(string path) : this(path, WadReadMode.Extendable)
        {

        }
        /// <summary>
        /// Opens a wad file from path
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="readmode"></param>
        public WadFile(string path, WadReadMode readmode) : this()
        {
            readMode = readmode;
            this.path = path;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                // Header
                byte[] buffer = new byte[8];
                stream.Read(buffer, 0, 4);
                string typeString = DoomUtils.ToName(buffer);
                WadType type = WadType.IWad;
                switch (typeString)
                {
                    case "IWAD":
                        break;
                    case "PWAD":
                        type = WadType.PWad;
                        break;
                    default:
                        throw new Exception("No supported wad type " + typeString);
                }
                stream.Read(buffer, 0, 4);
                uint directories = BitConverter.ToUInt32(buffer, 0);
                stream.Read(buffer, 0, 4);
                uint pointer = BitConverter.ToUInt32(buffer, 0);
                header = new WadHeader(type, directories, pointer);

                // Read lumps data
                contents = new byte[(int)stream.Length - HEADER_SIZE - LUMP_POINTER_SIZE * directories];
                stream.Read(contents, 0, contents.Length);

                // Directories
                List<uint> pointers = new List<uint>();
                for (uint i = 0; i < directories; i++)
                {
                    

                    stream.Read(buffer, 0, 4);
                    uint start = (uint)BitConverter.ToInt32(buffer, 0);
                    stream.Read(buffer, 0, 4);
                    uint size = (uint)BitConverter.ToInt32(buffer, 0);
                    stream.Read(buffer, 0, 8);

                    LumpPtr lumpPtr = new LumpPtr(start, size, buffer, this);
                    this.directories.Add(lumpPtr);

                    #region Insertion into lumps list
                    // Insert allocated lump
                    bool inserted = false;
                    for (int j = 0; j < pointers.Count - 1; j++)
                    {
                        if (start >= pointers[j] && start < pointers[j + 1])
                        {
                            pointers.Insert(j + 1, start);
                            real.Insert(j + 1, lumpPtr.Lump);
                            inserted = true;
                            break;
                        }
                    }
                    if (!inserted)
                    {
                        pointers.Add(start);
                        real.Add(lumpPtr.Lump);
                    }
                    #endregion
                    if (END_MARKERS.ContainsKey(lumpPtr.Name))
                    {
                        // Add end marker
                        DirectoryMarkers markerType = END_MARKERS[lumpPtr.Name];
                        if (!ranges.ContainsKey(markerType))
                            throw new Exception("A closing marker appeared before its opening (" + lumpPtr.Name + ")");
                        ranges[markerType] = new LumpPtrRange(ranges[markerType].Start, i);
                    }
                    if (START_MARKERS.ContainsKey(lumpPtr.Name))
                    {
                        DirectoryMarkers markerType = START_MARKERS[lumpPtr.Name];
                        if (ranges.ContainsKey(markerType))
                            throw new Exception("A opening marker appeared after it has already been opened (" + lumpPtr.Name + ")");
                        ranges[markerType] = new LumpPtrRange(i, 0);
                    }
                }

                // Load pallete
                Lump playpal = SeekFirst("PLAYPAL");
                if (playpal != null)
                {
                    palletes = new DoomPalleteCollection(playpal.GetByteStream());
                }

                // If The file is PWad load textures or if the file is IWad then load only if a pallete is loaded already
                if (type == WadType.PWad || (type == WadType.IWad && palletes != null))
                {
                    Lump textures = SeekFirst("TEXTURE1", "TEXTURE2");
                    Lump pnames = SeekFirst("PNAMES");

                    if (textures != null && pnames != null)
                    {
                        texturePool = new WadTexturePool(this, textures, pnames);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Allocates a lump from the lump data
        /// </summary>
        /// <param name="name">The name of the lump</param>
        /// <param name="start">Where does it start</param>
        /// <param name="size">What is its size</param>
        /// <returns></returns>
        internal Lump AllocateLump(string name, uint start, uint size)
        {
            Lump lump = new Lump(name, contents, start - HEADER_SIZE, size, this);
            lump.Name = name;
            return lump;
        }

        /// <summary>
        /// Gets a color pallete for the PLAYPAL lump
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DoomPallete Pallete(int index) => palletes[index];

        #region Locaters
        /// <summary>
        /// Returns the index of a lump
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            return directories.FindIndex((l) => l.Name == name);
        }
        /// <summary>
        /// Returns the end of a range
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public int GetRangeEnd(DirectoryMarkers marker)
        {
            if (!ranges.ContainsKey(marker))
            {
                return -1;
            }
            return (int)ranges[marker].End;
        }
        /// <summary>
        /// Returns the start of a range
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public int GetRangeStart(DirectoryMarkers marker)
        {
            if (!ranges.ContainsKey(marker))
            {
                return -1;
            }
            return (int)ranges[marker].Start;
        }
        /// <summary>
        /// Seeks the lumps by given their names and returns the first one it finds
        /// </summary>
        /// <param name="name"></param>
        /// <param name="otherNames"></param>
        /// <returns></returns>
        public Lump SeekFirst(string name, params string[] otherNames)
        {
            Lump lump = this[name];
            int i = 0;
            while(i < otherNames.Length && lump == null)
            {
                lump = this[otherNames[i]];
                i++;
            }
            return lump;
        }
        #endregion

        #region Saving
        /// <summary>
        /// Saves the .wad file.
        /// Only usable if used with an opened wad file
        /// </summary>
        public void Save()
        {
            if (readMode != WadReadMode.Writable) throw new WadAccessModeException("Wad cant be saved");

            if (path == null)
                throw new Exception("No save path specefied for .wad file");
            Save(path);
        }
        /// <summary>
        /// Saves the .wad file into a specified file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            if (readMode != WadReadMode.Writable) throw new WadAccessModeException("Wad in not modifiable");

            using (MemoryStream lumpStream = new MemoryStream(), pointerStream = new MemoryStream())
            {
                lumpStream.Write(Encoding.ASCII.GetBytes(Type.ToString()), 0, 4);
                lumpStream.Write(BitConverter.GetBytes(Items), 0, 4);

                foreach (var lump in real)
                {
                    lump.Write(lumpStream);
                    pointerStream.Write(BitConverter.GetBytes((uint)lumpStream.Position), 0, 4);
                    pointerStream.Write(BitConverter.GetBytes(lump.Size), 0, 4);
                    pointerStream.Write(DoomUtils.GetNameAsBytes(lump.Name), 0, 8);
                }
                uint directoryPointer = (uint)lumpStream.Position;

                lumpStream.Position = 8;
                lumpStream.Write(BitConverter.GetBytes(directoryPointer), 0, 4);

                using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    lumpStream.Position = 0;
                    pointerStream.Position = 0;
                    while (lumpStream.Position < lumpStream.Length)
                        stream.WriteByte((byte)lumpStream.ReadByte());
                    while (pointerStream.Position < pointerStream.Length)
                        stream.WriteByte((byte)pointerStream.ReadByte());
                }
            }
        }
        #endregion

        #region Loaders

        /// <summary>
        /// Loads a map from the wad file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public MapInterpter LoadMap(string name, Maps.SpecificationMode mode)
        {
            return new MapInterpter(this[name], mode);
        }

        /// <summary>
        /// Loads a texture from the wad file
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MapTexture LoadMapTexture(string name)
        {
            if (!HasTexturePool) throw new NoTexturePoolInWadException("Wad does not have a texture pool");
            return texturePool.GetMapTexture(name);
        }

        #endregion

        #region Additive Methods
        #region Add
        /// <summary>
        /// Adds a range to the wad
        /// </summary>
        /// <param name="range">The range type to add</param>
        /// <returns></returns>
        public int Add(DirectoryMarkers range)
        {
            if (readMode == WadReadMode.Readonly) throw new WadAccessModeException("Wad in not modifiable");

            if (ranges.ContainsKey(range))
                throw new Exception("The wad already has a range " + range);
            int start = directories.Count;
            Add(new Lump(MARKERS_START[range], this));
            int index = directories.Count;
            Add(new Lump(MARKERS_END[range], this));
            ranges.Add(range, new LumpPtrRange((uint)start, (uint)index));
            return index;
        }
        /// <summary>
        /// Adds a lump to the file
        /// </summary>
        /// <param name="lump"></param>
        public void Add(Lump lump)
        {
            if (readMode == WadReadMode.Readonly) throw new WadAccessModeException("Wad in not modifiable");

            real.Add(lump);
            directories.Add(new LumpPtr(lump));
        }
        /// <summary>
        /// Adds a lump to the file
        /// </summary>
        /// <param name="lump"></param>
        public void Add(LumpReader lump)
        {
            if (readMode == WadReadMode.Readonly) throw new WadAccessModeException("Wad in not modifiable");

            real.Add(lump.Lump);
            directories.Add(new LumpPtr(lump.Lump));
        }
        /// <summary>
        /// Adds lumps into a range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="lumps"></param>
        public void Add(DirectoryMarkers range, params Lump[] lumps)
        {
            if (readMode == WadReadMode.Readonly) throw new WadAccessModeException("Wad in not modifiable");

            LumpPtr[] pointers = new LumpPtr[lumps.Length];
            for (int i = 0; i < lumps.Length; i++)
                pointers[i] = new LumpPtr(lumps[i]);

            real.AddRange(lumps);
            int index = GetRangeEnd(range);
            if (index < 0)   // If range doesn't exists add it
            {
                index = Add(range);
            }
            directories.InsertRange(index, pointers);
        }
        /// <summary>
        /// Adds lumps into a range
        /// </summary>
        /// <param name="range"></param>
        /// <param name="lumps"></param>
        public void Add(DirectoryMarkers range, params LumpReader[] lumps)
        {
            if (readMode == WadReadMode.Readonly) throw new WadAccessModeException("Wad in not modifiable");

            LumpPtr[] pointers = new LumpPtr[lumps.Length];
            for (int i = 0; i < lumps.Length; i++)
            {
                pointers[i] = new LumpPtr(lumps[i].Lump);
                real.Add(lumps[i].Lump);
            }

            int index = GetRangeEnd(range);
            if (index < 0)   // If range doesn't exists add it
            {
                index = Add(range);
            }
            directories.InsertRange(index, pointers);
        }
        #endregion

        /// <summary>
        /// Removes the old texture pool and replaces it with a new one
        /// </summary>
        public void NewTexturePool()
        {
            texturePool = new WadTexturePool(this);
        }
        /// <summary>
        /// Stacks a new Pwad ontop of this wad (Adds and Overrides data)
        /// </summary>
        /// <param name="pwad"></param>
        /// <returns></returns>
        public bool StackPWad(WadFile pwad)
        {
            if (readMode != WadReadMode.Extendable) throw new WadAccessModeException("Wad file is not opened with extension capablities");

            patches.Push(pwad);

            throw new NotImplementedException();
            
            return true;
        }

        #endregion
    }
}
