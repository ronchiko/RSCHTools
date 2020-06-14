using System;
using System.Collections.Generic;
using System.Text;

namespace RCSHTools.Doom.Maps.UDMF
{
    public static class UDMFUtils
    {
        private static BidirectionalHashMap<UDMFLineDefenitionProperties, string> linedefprop_str = new BidirectionalHashMap<UDMFLineDefenitionProperties, string>();
        private static BidirectionalHashMap<LineDefenitionFlags, string> linedefflag_str = new BidirectionalHashMap<LineDefenitionFlags, string>();

        private static bool initaited;

        private static void Initiate()
        {
            if (!initaited)
            {
                linedefprop_str.Append(UDMFLineDefenitionProperties.Alpha, "alpha");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Argument0, "arg0");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Argument1, "arg1");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Argument2, "arg2");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Argument3, "arg3");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Argument4, "arg4");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Arguemnt0String, "arg0str");
                linedefprop_str.Append(UDMFLineDefenitionProperties.Comment, "comment");
                linedefprop_str.Append(UDMFLineDefenitionProperties.LockNumber, "locknumber");

                linedefflag_str.Append(LineDefenitionFlags.Blocking, "blocking");
                linedefflag_str.Append(LineDefenitionFlags.BlockMonsters, "blockmonsters");
                linedefflag_str.Append(LineDefenitionFlags.TwoSide, "twosided");
                linedefflag_str.Append(LineDefenitionFlags.DontPegTop, "dontpegtop");
                linedefflag_str.Append(LineDefenitionFlags.DontPegBottom, "dontpegbottom");
                linedefflag_str.Append(LineDefenitionFlags.Secret, "secret");
                linedefflag_str.Append(LineDefenitionFlags.Secret, "blocksound");
                linedefflag_str.Append(LineDefenitionFlags.Secret, "dontdraw");
                linedefflag_str.Append(LineDefenitionFlags.Secret, "mapped");
                linedefflag_str.Append(LineDefenitionFlags.Railing, "railing");
                linedefflag_str.Append(LineDefenitionFlags.PassUse, "passuse");
                linedefflag_str.Append(LineDefenitionFlags.RepeatSpecial, "repeatspecial");


            }
        }

        public static string ToString(this UDMFLineDefenitionProperties prop)
        {
            Initiate();
            return linedefprop_str[prop];
        }
        public static UDMFLineDefenitionProperties ToLineProp(this string str)
        {
            Initiate();
            return linedefprop_str[str];
        }
        public static string ToString(this LineDefenitionFlags flag)
        {
            Initiate();
            return linedefflag_str[flag];
        }
        public static LineDefenitionFlags ToString(this string flag)
        {
            Initiate();
            return linedefflag_str[flag];
        }
    }
}
