using NUnit.Framework;

namespace PdfWorkbench.Tests
{
    [TestFixture]
    public class PdfParserTests
    {
        [Test]
        public void GetVersion_ReadingVersion_Version14()
        {
            using (var stream = Helpers.GetStream("%PDF-1.4"))
            {
                var parser = new PdfParser(new PdfMarkerReader(stream));
                Assert.That(parser.GetVersion().ToString(), Is.EqualTo("1.4"));
            }
        }

        [Test]
        public void GetVersion_ReadingVersion_Version1()
        {
            using (var stream = Helpers.GetStream("%PDF-1"))
            {
                var parser = new PdfParser(new PdfMarkerReader(stream));
                Assert.That(parser.GetVersion().ToString(), Is.EqualTo("1.0"));
            }
        }
    }
}
