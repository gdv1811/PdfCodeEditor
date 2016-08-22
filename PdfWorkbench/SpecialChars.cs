using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfWorkbench
{
    internal static class SpecialChars
    {
        public static bool IsOctal(int @char)
        {
            return "01234567".IndexOf((char)@char) != -1;
        }

        public static bool IsWhiteSpace(int @char)
        {
            switch (@char)
            {
                case (int)WhiteSpaces.Null:
                case (int)WhiteSpaces.HorizontalTab:
                case (int)WhiteSpaces.LineFeed:
                case (int)WhiteSpaces.FormFeed:
                case (int)WhiteSpaces.CarriageReturn:
                case (int)WhiteSpaces.Space:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsEOL(int @char)
        {
            return @char == (int) WhiteSpaces.CarriageReturn || @char == (int) WhiteSpaces.LineFeed;
        }

        public static bool IsHex(int @char)
        {
            return "0123456789abcdefABCDEF".IndexOf((char)@char) != -1;
        }

        public static bool IsDelimiter(int @char)
        {
            return "()<>[]{}/%".IndexOf((char)@char) != -1;
        }
    }
}
