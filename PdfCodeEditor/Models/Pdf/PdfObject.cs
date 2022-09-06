using System.Collections.Generic;

namespace PdfCodeEditor.Models.Pdf
{
    internal class PdfObject
    {
        public PdfObjectType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Reference { get; set; }

        public List<PdfObject> ValuesCollection { get; set; }
    }

    internal class PdfExceptionObject:PdfObject
    {
        public PdfExceptionObject(string type, string message)
        {
            Type = PdfObjectType.Exception;
            Name = type;
            Value = message;
        }
    }

    //internal class PdfDictionary:PdfObject
    //{
    //    public override PdfObjectType Type => PdfObjectType.Dictionary;

    //    public Dictionary<string, PdfObject> Dictionary { get; set; }

    //}

    internal enum PdfObjectType
    {
        Boolean,
        Numeric,
        LiteralString,
        HexString,
        Name,
        Array,
        Dictionary,
        Stream,
        Null,
        Reference,

        Exception
    }
}
