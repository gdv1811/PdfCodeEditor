using System.IO;
using NUnit.Framework;

namespace PdfWorkbench.Tests
{
    [TestFixture]
    public class PdfMarkerReaderTests
    {
        [Test]
        public void GetMarker_ReadingComment_CommentMarker()
        {
            const string comment = "%PDF-1.0";
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(comment);
                    streamWriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                    Assert.That(marker.Content, Is.EqualTo(comment));
                    Assert.That(marker.Offset, Is.EqualTo(0));
                    Assert.That(marker.Length, Is.EqualTo(comment.Length));
                }
            }
        }

        [Test]
        public void GetMarker_ReadingCommentWithWhiteCharsBefore_CommentMarker()
        {
            const string comment = "%PDF-1.0";
            using (var stream = new MemoryStream())
            {
                stream.Write(new[]
                {
                    (byte) WhiteSpaces.FormFeed,
                    (byte) WhiteSpaces.CarriageReturn,
                    (byte) WhiteSpaces.HorizontalTab,
                    (byte) WhiteSpaces.LineFeed,
                    (byte) WhiteSpaces.Null,
                    (byte) WhiteSpaces.Space
                }, 0, 6);
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(comment);
                    streamWriter.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                    Assert.That(marker.Content, Is.EqualTo(comment));
                    Assert.That(marker.Offset, Is.EqualTo(6));
                    Assert.That(marker.Length, Is.EqualTo(comment.Length));
                }
            }
        }

        [Test]
        public void GetMarker_ReadingCommentWithWhiteCharsAfter_CommentMarker()
        {
            const string comment = "%PDF-1.0";
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(comment);
                    streamWriter.Flush();

                    stream.Write(new[]
                    {
                        (byte) WhiteSpaces.FormFeed,
                        (byte) WhiteSpaces.HorizontalTab,
                        (byte) WhiteSpaces.CarriageReturn,
                        (byte) WhiteSpaces.LineFeed,
                        (byte) WhiteSpaces.Null,
                        (byte) WhiteSpaces.Space
                    }, 0, 6);

                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                    Assert.That(marker.Content,
                        Is.EqualTo(comment + (char)WhiteSpaces.FormFeed + (char)WhiteSpaces.HorizontalTab));
                    Assert.That(marker.Offset, Is.EqualTo(0));
                    Assert.That(marker.Length, Is.EqualTo(comment.Length + 2));
                }
            }
        }

        [Test]
        public void GetMarker_ReadingHexString_HexStringMarker()
        {
            const string hex = "<4E6F762073686D6F7A206B6120706F702E>";
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(hex);
                    streamWriter.Flush();

                    //stream.Write(new[]
                    //{
                    //    (byte) WhiteSpaces.FormFeed,
                    //    (byte) WhiteSpaces.HorizontalTab,
                    //    (byte) WhiteSpaces.CarriageReturn,
                    //    (byte) WhiteSpaces.LineFeed,
                    //    (byte) WhiteSpaces.Null,
                    //    (byte) WhiteSpaces.Space
                    //}, 0, 6);

                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.HexString));
                    Assert.That(marker.Content, Is.EqualTo(hex));
                    Assert.That(marker.Offset, Is.EqualTo(0));
                    Assert.That(marker.Length, Is.EqualTo(hex.Length));
                }
            }
        }

        [Test]
        public void GetMarker_ReadingHexStringWithWhiteSpaces_HexStringMarker()
        {
            var hex = "<4E6" + (char) WhiteSpaces.FormFeed +
                      "F762" + (char) WhiteSpaces.HorizontalTab +
                      "0736" + (char) WhiteSpaces.CarriageReturn +
                      "86D6" + (char) WhiteSpaces.LineFeed +
                      "F7A2" + (char) WhiteSpaces.Null +
                      "06B6" + (char) WhiteSpaces.Space + "120706F702E>";
            const string hexExpect = "<4E6F762073686D6F7A206B6120706F702E>";
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(hex);
                    streamWriter.Flush();

                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.HexString));
                    Assert.That(marker.Content, Is.EqualTo(hexExpect));
                    Assert.That(marker.Offset, Is.EqualTo(0));
                    Assert.That(marker.Length, Is.EqualTo(hex.Length));
                }
            }
        }

        [Test]
        public void GetMarker_ReadingStartDictionary_StartDictionaryMarker()
        {
            var dictStart = "<<";
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(dictStart);
                    streamWriter.Flush();

                    stream.Seek(0, SeekOrigin.Begin);

                    var reader = new PdfMarkerReader(stream);
                    var marker = reader.GetMarker();

                    Assert.That(marker.Type, Is.EqualTo(MarkerType.StartDictionary));
                    Assert.That(marker.Content, Is.EqualTo(dictStart));
                    Assert.That(marker.Offset, Is.EqualTo(0));
                    Assert.That(marker.Length, Is.EqualTo(dictStart.Length));
                }
            }
        }
    }
}
