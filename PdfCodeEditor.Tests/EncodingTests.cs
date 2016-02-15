using System;
using System.IO;
using System.Text;
using PdfCodeEditor.Models;
using NUnit.Framework;

namespace PdfCodeEditor.Tests
{
    [TestFixture]
    public class EncodingTests
    {
        [Test]
        public void EncodingTest()
        {
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, new PdfEncoding()))
                {
                    var builder = new StringBuilder();
                    for (int i = 0; i < 255; i++)
                    {
                        builder.Append(Convert.ToChar(i));
                    }

                    streamWriter.Write(builder);
                    streamWriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var bytes = new byte[256];
                    stream.Read(bytes, 0, bytes.Length);
                    for (int i = 0; i < 255; i++)
                    {
                        Assert.That(bytes[i] == i, Is.True);
                    }
                }

            }
        }
    }
}
