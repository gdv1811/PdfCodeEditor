// Copyright (c) 2016 Dmitry Goryachev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Text;

namespace PdfWorkbench
{
    internal enum WhiteSpaces
    {
        Null = 0x00,            // Null(NUL)
        HorizontalTab = 0x09,   // HORIZONTAL TAB (HT)
        LineFeed = 0x0A,        // LINE FEED(LF)
        FormFeed = 0x0C,        // FORM FEED(FF)
        CarriageReturn = 0x0D,  // CARRIAGE RETURN(CR)
        Space = 0x20            // SPACE(SP)
    }

    internal enum MarkerType
    {
        Comment,
        StartDictionary,
        EndDictionary,
        HexString,
        Name,
        LBracket,
        RBracket,
        LBrace,
        RBrace,
        LiteralString,
        Word,
        Number
    }

    internal class PdfMarker
    {
        private StringBuilder _builder = new StringBuilder();

        public MarkerType Type { get; set; }
        public int Offset { get; set; }

        public string Content
        {
            get { return _builder.ToString(); }
            set { _builder = new StringBuilder(value); }
        }

        public int Length { get; set; }

        public void Append(string s)
        {
            _builder.Append(s);
        }

        public void Append(char s)
        {
            _builder.Append(s);
        }
    }
}
