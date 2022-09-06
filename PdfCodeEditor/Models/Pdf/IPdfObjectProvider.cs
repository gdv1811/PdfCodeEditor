
namespace PdfCodeEditor.Models.Pdf
{
    internal interface IPdfObjectProvider
    {
        PdfObject GetPdfVersion();
        PdfObject GetTrailer();
        PdfObject GetPdfObject();
    }
}
