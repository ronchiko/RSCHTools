using System;
using System.Text;
using RCSHTools.Doom.Interpeters;


namespace RCSHTools.Doom
{
    /// <summary>
    /// Allows access to a lumps data as text
    /// </summary>
    public class TextInterpeter : LumpReader, IAutoParseLump
    {
        /// <summary>
        /// The encoding the text is
        /// </summary>
        public virtual TextIntepeterEncoding TextEncoding => TextIntepeterEncoding.Default;
        /// <summary>
        /// The translated text from the lump
        /// </summary>
        private string Text
        {
            get;
            set;
        }

        public TextInterpeter(Lump lump) : base(lump) { }

        void IAutoParseLump.Parse()
        {
            switch (TextEncoding)
            {
                case TextIntepeterEncoding.ASCII:
                    Text = Encoding.ASCII.GetString(Raw);
                    break;
                case TextIntepeterEncoding.UTF8:
                    Text = Encoding.UTF8.GetString(Raw);
                    break;
                default:
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in Raw)
                    {
                        sb.Append((char)b);
                    }
                    Text = sb.ToString();
                    break;
            }
           
        }
        byte[] IAutoParseLump.ToRaw()
        {
            switch (TextEncoding)
            {
                case TextIntepeterEncoding.ASCII:
                    return Encoding.ASCII.GetBytes(Text);
                case TextIntepeterEncoding.UTF8:
                    return Encoding.UTF8.GetBytes(Text);
                default:
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        foreach (char c in Text)
                        {
                            stream.WriteByte((byte)c);
                        }
                        return stream.ToArray();
                    }
            }           
        }
    }

    public enum TextIntepeterEncoding
    {
        /// <summary>
        /// The default translation
        /// </summary>
        Default,
        /// <summary>
        /// ASCII translation
        /// </summary>
        ASCII,
        /// <summary>
        /// UTF-8 translation
        /// </summary>
        UTF8
    }
}
