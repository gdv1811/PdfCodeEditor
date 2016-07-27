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

using System.IO;
using System.Text;

namespace PdfWorkbench
{
    internal class PdfMarkerReader
    {
        private readonly Stream _stream;
        public PdfMarkerReader(Stream stream)
        {
            _stream = stream;
        }

        public PdfMarker GetMarker()
        {
            int bt;
            do
            {
                bt = _stream.ReadByte();
                if (bt == -1)
                    throw new EndOfStreamException();
            } while (IsWhiteSpace(bt));

            var firstChar = (char)bt;

            switch (firstChar)
            {
                case '%':
                    return ReadComment(firstChar);
            }

            return null;
        }

        private PdfMarker ReadComment(char firstChar)
        {
            var marker = new PdfMarker { Type = MarkerType.Comment, Offset = (int)(_stream.Position - 1) };
            var builder = new StringBuilder(firstChar.ToString());
            int bt;
            while (true)
            {
                bt = _stream.ReadByte();
                if (bt == -1 || bt == (int)WhiteSpaces.CarriageReturn || bt == (int)WhiteSpaces.CarriageReturn)
                    break;
                builder.Append((char)bt);
            }

            marker.Content = builder.ToString();
            marker.Length = (int)_stream.Position - marker.Offset;
            if (bt != -1)
                marker.Length--;
            return marker;
        }

        private bool IsWhiteSpace(int c)
        {
            switch (c)
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
    }
}
