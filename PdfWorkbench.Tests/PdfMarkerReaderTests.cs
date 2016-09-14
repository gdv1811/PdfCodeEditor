using System.Text;
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
            using (var stream = Helpers.GetStream(comment))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                Assert.That(marker.Content, Is.EqualTo(comment));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(comment.Length));
            }
        }

        [Test]
        public void GetMarker_ReadingCommentWithWhiteCharsBefore_CommentMarker()
        {
            const string comment = "%PDF-1.0";
            var text = "" + (char)WhiteSpaces.FormFeed +
                       (char)WhiteSpaces.CarriageReturn +
                       (char)WhiteSpaces.HorizontalTab +
                       (char)WhiteSpaces.LineFeed +
                       (char)WhiteSpaces.Null +
                       (char)WhiteSpaces.Space +
                       comment;
            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                Assert.That(marker.Content, Is.EqualTo(comment));
                Assert.That(marker.Offset, Is.EqualTo(6));
                Assert.That(marker.Length, Is.EqualTo(comment.Length));
            }
        }

        [Test]
        public void GetMarker_ReadingCommentWithWhiteCharsAfter_CommentMarker()
        {
            const string comment = "%PDF-1.0";
            var text = comment +
                       (char)WhiteSpaces.FormFeed +
                       (char)WhiteSpaces.HorizontalTab +
                       (char)WhiteSpaces.CarriageReturn +
                       (char)WhiteSpaces.LineFeed +
                       (char)WhiteSpaces.Null +
                       (char)WhiteSpaces.Space;
            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.Comment));
                Assert.That(marker.Content,
                    Is.EqualTo(comment + (char)WhiteSpaces.FormFeed + (char)WhiteSpaces.HorizontalTab));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(comment.Length + 2));
            }
        }

        [Test]
        public void GetMarker_ReadingHexString_HexStringMarker()
        {
            const string hex = "<4E6F762073686D6F7A206B6120706F702E>";
            using (var stream = Helpers.GetStream(hex))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.HexString));
                Assert.That(marker.Content, Is.EqualTo(hex));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(hex.Length));
            }
        }

        [Test]
        public void GetMarker_ReadingHexStringWithWhiteSpaces_HexStringMarker()
        {
            var hex = "<4E6" + (char)WhiteSpaces.FormFeed +
                      "F762" + (char)WhiteSpaces.HorizontalTab +
                      "0736" + (char)WhiteSpaces.CarriageReturn +
                      "86D6" + (char)WhiteSpaces.LineFeed +
                      "F7A2" + (char)WhiteSpaces.Null +
                      "06B6" + (char)WhiteSpaces.Space + "120706F702E>";
            const string hexExpect = "<4E6F762073686D6F7A206B6120706F702E>";
            using (var stream = Helpers.GetStream(hex))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.HexString));
                Assert.That(marker.Content, Is.EqualTo(hexExpect));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(hex.Length));
            }
        }

        [Test]
        public void GetMarker_ReadingStartDictionary_StartDictionaryMarker()
        {
            const string dictStart = "<<";
            using (var stream = Helpers.GetStream(dictStart))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.StartDictionary));
                Assert.That(marker.Content, Is.EqualTo(dictStart));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(dictStart.Length));
            }
        }

        [Test]
        public void ReadString_ReadingOctalChar_OctalChar()
        {
            const string octalChar = "\\053)";

            using (var stream = Helpers.GetStream(octalChar))
            {
                var reader = new PdfLiteralStringReader(stream);
                var builder = new StringBuilder("(");
                reader.ReadString(builder);
                Assert.That(builder.ToString(), Is.EqualTo("(+)"));
            }
        }

        [Test]
        public void ReadString_ReadingStringWithEOL_String()
        {
            var text = "1234" + (char)WhiteSpaces.CarriageReturn + (char)WhiteSpaces.LineFeed + "zxcv)";

            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfLiteralStringReader(stream);
                var builder = new StringBuilder("(");
                reader.ReadString(builder);
                Assert.That(builder.ToString(), Is.EqualTo("(1234\rzxcv)"));
            }
        }

        [Test]
        public void GetMarker_ReadingString_StringWithoutSplit()
        {
            var text = "(These \\" + (char)WhiteSpaces.CarriageReturn + (char)WhiteSpaces.LineFeed +
                       "two strings \\" + (char)WhiteSpaces.CarriageReturn + (char)WhiteSpaces.LineFeed +
                       "are the same.)";

            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfMarkerReader(stream);
                var marker = reader.GetMarker();

                Assert.That(marker.Type, Is.EqualTo(MarkerType.LiteralString));
                Assert.That(marker.Content, Is.EqualTo("(These two strings are the same.)"));
                Assert.That(marker.Offset, Is.EqualTo(0));
                Assert.That(marker.Length, Is.EqualTo(text.Length));
            }
        }

        [Test]
        public void GetMarker_ReadingWordAndNumber_ThreeMarkers()
        {
            var text = "12 0 obj";

            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfMarkerReader(stream);
                var num1 = reader.GetMarker();
                var num2 = reader.GetMarker();
                var word = reader.GetMarker();
                
                Assert.That(num1.Type, Is.EqualTo(MarkerType.Number));
                Assert.That(num1.Content, Is.EqualTo("12"));

                Assert.That(num2.Type, Is.EqualTo(MarkerType.Number));
                Assert.That(num2.Content, Is.EqualTo("0"));

                Assert.That(word.Type, Is.EqualTo(MarkerType.Word));
                Assert.That(word.Content, Is.EqualTo("obj"));
            }
        }

        [Test]
        public void GetMarker_ReadingWord_Word()
        {
            var text = "12obj";

            using (var stream = Helpers.GetStream(text))
            {
                var reader = new PdfMarkerReader(stream);
                var word = reader.GetMarker();

                Assert.That(word.Type, Is.EqualTo(MarkerType.Word));
                Assert.That(word.Content, Is.EqualTo("12obj"));
            }
        }
    }
}
