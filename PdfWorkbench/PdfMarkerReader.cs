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

using System;
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
            var bt = ReadWhile(IsWhiteSpace);
            if (bt == -1)
                throw new EndOfStreamException();

            var firstChar = (char)bt;

            switch (firstChar)
            {
                case '%':
                    return ReadComment(firstChar);
                case '<':
                    return ReadHexString(firstChar);
            }

            return null;
        }

        private PdfMarker ReadHexString(char firstChar)
        {
            var marker = new PdfMarker { Offset = (int)(_stream.Position - 1) };
            var builder = new StringBuilder(firstChar.ToString());

            var lastByte = ReadWhile(arg => IsHex((char)arg) || IsWhiteSpace(arg), builder, IsWhiteSpace);

            switch (lastByte)
            {
                case '<':
                    if (marker.Offset + 2 != _stream.Position)
                        throw new Exception("Unexpected char in hex string: " + (char)lastByte);
                    builder.Append((char)lastByte);
                    marker.Type = MarkerType.StartDictionary;
                    marker.Content = builder.ToString();
                    marker.Length = (int)_stream.Position - marker.Offset;
                    return marker;
                case '>':
                    builder.Append((char)lastByte);
                    marker.Type = MarkerType.HexString;
                    marker.Content = builder.ToString();
                    marker.Length = (int)_stream.Position - marker.Offset;
                    return marker;
            }
            throw new Exception("Unexpected char in hex string: " + (char)lastByte);
        }

        private PdfMarker ReadComment(char firstChar)
        {
            var marker = new PdfMarker { Type = MarkerType.Comment, Offset = (int)(_stream.Position - 1) };
            var builder = new StringBuilder(firstChar.ToString());

            var lastChar =
                ReadWhile(bt => bt != (int)WhiteSpaces.CarriageReturn && bt != (int)WhiteSpaces.CarriageReturn,
                    builder);

            marker.Content = builder.ToString();
            marker.Length = (int)_stream.Position - marker.Offset;
            if (lastChar != -1)
                marker.Length--;
            return marker;
        }

        private int ReadWhile(Func<int, bool> condition, StringBuilder builder = null, Func<int, bool> ignorCondition = null)
        {
            while (true)
            {
                var bt = _stream.ReadByte();
                if (bt == -1 || !condition(bt))
                    return bt; // return last char
                if (ignorCondition == null || !ignorCondition(bt))
                    builder?.Append((char)bt);
            }
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

        private bool IsHex(char c)
        {
            return "0123456789abcdefABCDEF".IndexOf(c) != -1;
        }
    }
}
