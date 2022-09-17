// Copyright (c) 2022 Dmitry Goryachev
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

using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;

namespace PdfCodeEditor.Models.Pdf
{
    internal class PdfObjectiTextProvider:IPdfObjectProvider
    {
        private readonly PdfDocument _document;
        public PdfObjectiTextProvider(string path)
        {
            var reader = new PdfReader(path);
            _document = new PdfDocument(reader);
        }

        public PdfObject GetPdfVersion()
        {
            try
            {
                var vers = _document.GetPdfVersion();
                return new PdfObject
                {
                    Type = PdfObjectType.Numeric,
                    Name = "Version",
                    Value = vers.ToString()
                };
            }
            catch (Exception ex)
            {
                return new PdfExceptionObject(ex.GetType().Name, ex.Message);
            }
        }

        public PdfObject GetTrailer()
        {
            var trailer = _document.GetTrailer();
            var obj = FromObject(trailer);
            obj.Name = "trailer";

            return obj;
        }

        private PdfObject FromObject(iText.Kernel.Pdf.PdfObject srcObj)
        {
            switch (srcObj)
            {
                case PdfBoolean boolean:
                    return new PdfObject
                    {
                        Type = PdfObjectType.Boolean,
                        Value = boolean.ToString()
                    };
                case PdfNumber num:
                    return new PdfObject
                    {
                        Type = PdfObjectType.Numeric,
                        Value = num.ToString()
                    };
                case PdfLiteral lit:
                    return new PdfObject
                    {
                        Type = PdfObjectType.LiteralString,
                        Value = lit.ToString()
                    };
                case PdfString str:
                    if (str.IsHexWriting())
                        return new PdfObject
                        {
                            Type = PdfObjectType.HexString,
                            Value = str.ToString() // todo: change presentation to hex
                        };
                    return new PdfObject
                    {
                        Type = PdfObjectType.LiteralString,
                        Value = str.ToString()
                    };
                case PdfNull _:
                    return new PdfObject
                    {
                        Type = PdfObjectType.Null,
                        Value = "null"
                    };
                case PdfName name:
                    return new PdfObject
                    {
                        Type = PdfObjectType.Name,
                        Value = name.ToString()
                    };

                case PdfIndirectReference reference:
                    return new PdfObject
                    {
                        Type = PdfObjectType.Reference,
                        ValueReference = new PdfReference
                        {
                            Id = reference.GetObjNumber(),
                            Generation = reference.GetGenNumber(),
                            Offset = reference.GetOffset()
                        }
                    };

                //case PdfStream stm:
                case PdfDictionary dict:
                    return FromDictionary(dict);

                case PdfArray array:
                    return FromArray(array);

                default:
                    return new PdfExceptionObject("ArgumentException", $"{srcObj.GetType()} cannot be parsed.");

            }
        }

        private PdfObject FromArray(PdfArray array)
        {
            var obj = new PdfObject
            {
                Type = PdfObjectType.Array,
                ValuesCollection = new List<PdfObject>()
            };
            for (int i = 0; i < array.Size(); i++)
            {
                var childObj = FromObject(array.Get(i, false));
                childObj.Name = $"[{i}]";
                obj.ValuesCollection.Add(childObj);
            }
            return obj;
        }

        private PdfObject FromDictionary(PdfDictionary dict)
        {
            var obj = new PdfObject
            {
                Type = PdfObjectType.Dictionary,
                ValuesCollection = new List<PdfObject>()
            };
            foreach (var key in dict.KeySet())
            {
                var item = dict.Get(key, false);
                var childObj = FromObject(item);
                childObj.Name = key.ToString();
                obj.ValuesCollection.Add(childObj);
            }
            if (dict is PdfStream)
            {
                obj.Type = PdfObjectType.Stream;
                obj.ValuesCollection.Add(
                    new PdfObject { Type = PdfObjectType.Stream, Name = "stream", Value = "binary stream" });
            }
            return obj;
        }

        public PdfObject GetPdfObject(PdfReference reference)
        {
            var obj = _document.GetPdfObject(reference.Id);
            return FromObject(obj);
        }

    }
}
