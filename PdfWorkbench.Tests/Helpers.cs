using System.IO;
using System.Text;

namespace PdfWorkbench.Tests
{
    internal static class Helpers
    {
        internal static Stream GetStream(string text)
        {
            var stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream, Encoding.Default, text.Length, true))
            {
                streamWriter.Write(text);
                streamWriter.Flush();
                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }
    }
}
