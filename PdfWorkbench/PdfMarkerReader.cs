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
        private int _lastChar = -1;
        private readonly PdfLiteralStringReader _stringReader;

        public PdfMarkerReader(Stream stream)
        {
            _stream = stream;
            _stringReader = new PdfLiteralStringReader(stream);
        }

        public PdfMarker GetMarker()
        {
            var firstChar = _lastChar;
            _lastChar = -1;
            if (firstChar == -1 || SpecialChars.IsWhiteSpace(firstChar))
            {
                firstChar = ReadWhile(SpecialChars.IsWhiteSpace);
                if (firstChar == -1)
                    throw new EndOfStreamException();
            }

            var marker = new PdfMarker { Offset = (int)(_stream.Position - 1) };
            marker.Append((char)firstChar);

            switch (firstChar)
            {
                case '%':
                    ReadComment(marker);
                    break;
                case '<':
                    ReadStartDictionaryOrHexString(marker);
                    break;
                case '>':
                    ReadEndDictionary(marker);
                    break;
                case '(':
                    ReadString(marker);
                    break;
                case '/':
                    ReadName(marker);
                    break;
                case '[':
                    marker.Type = MarkerType.LBracket;
                    break;
                case ']':
                    marker.Type = MarkerType.RBracket;
                    break;
                case '{':
                    marker.Type = MarkerType.LBrace;
                    break;
                case '}':
                    marker.Type = MarkerType.RBrace;
                    break;
            }
            marker.Length = (int)_stream.Position - marker.Offset;
            if (_lastChar != -1)
                marker.Length--;
            return marker;
        }

        private void ReadName(PdfMarker marker)
        {
            marker.Type = MarkerType.Name;

            var lastChar =
                ReadWhile(bt => !SpecialChars.IsWhiteSpace(bt) && !SpecialChars.IsDelimiter((char)bt),
                    marker);

            if (lastChar != -1)
                _lastChar = lastChar;
        }

        private void ReadString(PdfMarker marker)
        {
            var builder = new StringBuilder(marker.Content);
            _stringReader.ReadString(builder);
            marker.Type = MarkerType.LiteralString;
            marker.Content = builder.ToString();
        }

        private void ReadEndDictionary(PdfMarker marker)
        {
            var c = (char)_stream.ReadByte();
            if (c != '>')
                throw new Exception("Unexpected char: " + c);
            marker.Type = MarkerType.EndDictionary;
            marker.Append(c);
        }

        private void ReadStartDictionaryOrHexString(PdfMarker marker)
        {
            var lastByte = ReadWhile(arg => SpecialChars.IsHex(arg) || SpecialChars.IsWhiteSpace(arg), marker, SpecialChars.IsWhiteSpace);
            marker.Append((char)lastByte);
            switch (lastByte)
            {
                case '<':
                    if (marker.Offset + 2 != _stream.Position)
                        throw new Exception("Unexpected char in hex string: " + (char)lastByte);
                    marker.Type = MarkerType.StartDictionary;
                    return;
                case '>':
                    marker.Type = MarkerType.HexString;
                    marker.Length = (int)_stream.Position - marker.Offset;
                    return;
                default:
                    throw new Exception("Unexpected char in hex string: " + (char)lastByte);
            }
        }

        private void ReadComment(PdfMarker marker)
        {
            marker.Type = MarkerType.Comment;

            var lastChar =
                ReadWhile(bt => bt != (int)WhiteSpaces.CarriageReturn && bt != (int)WhiteSpaces.CarriageReturn,
                    marker);

            if (lastChar != -1)
                _lastChar = lastChar;
        }

        private int ReadWhile(Func<int, bool> condition, PdfMarker marker = null, Func<int, bool> ignorCondition = null)
        {
            while (true)
            {
                var bt = _stream.ReadByte();
                if (bt == -1 || !condition(bt))
                    return bt; // return last char
                if (ignorCondition == null || !ignorCondition(bt))
                    marker?.Append((char)bt);
            }
        }
    }
}
