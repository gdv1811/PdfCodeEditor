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
    internal class PdfLiteralStringReader
    {
        private readonly Stream _stream;
        private bool _isSequenceChar;
        private int _octalCharCount;
        private bool _isPrevCarriageReturn;

        public PdfLiteralStringReader(Stream stream)
        {
            _stream = stream;
        }

        public void ReadString(StringBuilder builder)
        {
            while (true)
            {
                var c = _stream.ReadByte();

                if (_isPrevCarriageReturn && c == (int)WhiteSpaces.LineFeed)
                {
                    _isPrevCarriageReturn = false;
                    continue;
                }
                _isPrevCarriageReturn = c == (int)WhiteSpaces.CarriageReturn;

                if (_octalCharCount != 0)
                {
                    if (SpecialChars.IsOctal(c))
                    {
                        _octalCharCount++;
                        builder[builder.Length - 1] = (char)((builder[builder.Length - 1] << 3) + (c - '0'));
                        if (_octalCharCount == 3)
                            _octalCharCount = 0;
                        continue;
                    }
                    _octalCharCount = 0;
                }

                if (_isSequenceChar)
                {
                    GetSequenceChar((char)c, builder);
                    continue;
                }

                if (c == '\\')
                {
                    _isSequenceChar = true;
                    continue;
                }

                builder.Append((char)c);

                if (c == '(')
                    ReadString(builder);

                if (c == ')')
                    return;
            }
        }

        private void GetSequenceChar(char c, StringBuilder builder)
        {
            _isSequenceChar = false;
            switch (c)
            {
                case (char)WhiteSpaces.CarriageReturn:
                case (char)WhiteSpaces.LineFeed:
                    break;
                case 'n':
                    builder.Append("\n"); // LINE FEED(0Ah)(LF)
                    break;
                case 'r':
                    builder.Append("\r"); // CARRIAGE RETURN(0Dh)(CR)
                    break;
                case 't':
                    builder.Append("\t"); // HORIZONTAL TAB(09h)(HT)
                    break;
                case 'b':
                    builder.Append("\b"); // BACKSPACE (08h) (BS)
                    break;
                case 'f':
                    builder.Append("\f"); // FORM FEED(FF)
                    break;
                default:
                    if (SpecialChars.IsOctal(c))
                    {
                        _octalCharCount = 1;
                        builder.Append((char)(c - '0'));
                    }
                    else
                        builder.Append(c);
                    break;
            }
        }
    }
}
