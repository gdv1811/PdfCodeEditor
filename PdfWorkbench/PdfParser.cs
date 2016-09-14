﻿// Copyright (c) 2016 Dmitry Goryachev
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

namespace PdfWorkbench
{
    internal class PdfParser
    {
        private readonly PdfMarkerReader _markerReader;

        public PdfParser(PdfMarkerReader markerReader)
        {
            _markerReader = markerReader;
        }

        public PdfVersion GetVersion()
        {
            _markerReader.Stream.Seek(0, SeekOrigin.Begin);
            while (_markerReader.Stream.Position < 1024)
            {
                var marker = _markerReader.GetMarker();
                if (marker.Type != MarkerType.Comment)
                    continue;
                if (!marker.Content.StartsWith("%PDF-"))
                    continue;
                return new PdfVersion(marker.Content);
            }

            return null;
        }
    }
}
