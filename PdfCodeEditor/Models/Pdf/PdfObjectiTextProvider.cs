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
                    Name = "PdfVersion",
                    Value = vers.ToString()
                };
            }
            catch (Exception ex)
            {
                return new PdfExceptionObject(ex.GetType().Name, ex.Message);
            }
        }

        //public Dictionary<string, string> GetTrailer()
        //{
        //    var trailer = _document.GetTrailer();
        //    var keys = trailer.KeySet();
        //    var dict = new Dictionary<string, string>();
        //    foreach (var key in keys)
        //    {
        //        dict.Add(key.ToString(), trailer.Get(key).ToString());
        //    }


        //    return dict;
        //}

        public PdfObject GetTrailer()
        //public IEnumerable<PdfObject> GetTrailer()
        {
            var trailer = _document.GetTrailer();
            var keys = trailer.KeySet();
            //foreach (var key in keys)
            //    //    {
            //    //        dict.Add(key.ToString(), trailer.Get(key).ToString());
            //    //    }
            //    yield return new PdfObject
            //    {
            //        Type = PdfObjectType.Numeric,
            //        Name = "Trailer",
            //        ValueCollection = vers.ToString()
            //    };

            return null;
        }

        public PdfObject GetPdfObject()
        {
            throw new NotImplementedException();
        }

    }
}
