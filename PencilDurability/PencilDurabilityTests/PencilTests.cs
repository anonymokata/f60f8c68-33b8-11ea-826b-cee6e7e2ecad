using Xunit;
using PencilDurability;
using System;

namespace PencilDurabilityTests
{
    public class PencilTests
    {
        // TODO: all characters not explicitly Uppercase or whitespace will be considered lowercase.
        private readonly Paper _arbitraryPaper;
        private readonly int _arbitraryDurability;
        private readonly int _arbitraryLength;

        public PencilTests()
        {
            _arbitraryPaper = new Paper();
            _arbitraryDurability = 999999;
            _arbitraryLength = 99999;
        }

        public class Writing : PencilTests
        {
            [Fact]
            public void ShouldWriteToPaper()
            {
                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength);
                const string testSentence = "This is a sentence.";

                pencil.Write(_arbitraryPaper, testSentence);

                Assert.Equal(testSentence, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldAppendToPreExistingText()
            {
                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength);
                const string testSentence1 = "This is a sentence.";
                const string testSentence2 = " This is another sentence.";
                _arbitraryPaper.Text = testSentence1;

                pencil.Write(_arbitraryPaper, testSentence2);

                Assert.Equal(testSentence1 + testSentence2, _arbitraryPaper.Text);
            }
        }

        public class PencilWithNoDurability : PencilTests
        {
            [Fact]
            public void ShouldWriteCorrectNumberOfSpaces()
            {
                const int noDurability = 0;
                var pencil = new Pencil(noDurability, _arbitraryLength);
                const string testSentence = "This should not be written.";
                const string expectedSpaces = "                           ";

                pencil.Write(_arbitraryPaper, testSentence);

                Assert.Equal(expectedSpaces, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldAppendSpacesToText()
            {
                const int noDurability = 0;
                var pencil = new Pencil(noDurability, _arbitraryLength);

                const string preexistingText = "This already exists.";
                const string testSentence = " This should not be written.";
                const string expectedSpaces = "                            ";

                _arbitraryPaper.Text = preexistingText;

                pencil.Write(_arbitraryPaper, testSentence);

                Assert.Equal(preexistingText + expectedSpaces, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldWriteWhitespaceCharacters()
            {
                const int noDurability = 0;
                var pencil = new Pencil(noDurability, _arbitraryLength);
                const string testWhitespace = "  \t\r\n\f\v  ";

                pencil.Write(_arbitraryPaper, testWhitespace);

                Assert.Equal(testWhitespace, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldKeepWhitespaceInMiddleOfText()
            {
                const int noDurability = 0;
                var pencil = new Pencil(noDurability, _arbitraryLength);
                const string testSentence = "\vT\this is a\t sentence\n with whi\r\ntespace chara\ncters.\f";
                const string expectedText = "\v \t        \t         \n         \r\n             \n      \f";

                pencil.Write(_arbitraryPaper, testSentence);

                Assert.Equal(expectedText, _arbitraryPaper.Text);
            }
        }

        public class PencilWithDurability : PencilTests
        {
            [Fact]
            public void ShouldWritePartOfTheSentenceWithoutEnoughDurability()
            {
                const int durability = 26;
                const string testSentence = "This should not be written and This should not be written";
                const string expectedSentence = "This should not be written and                           ";
                var pencil = new Pencil(durability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, testSentence);

                Assert.Equal(expectedSentence, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldNotWriteUppercaseIfNotEnoughDurability()
            {
                const int startDurability = 2;
                const string testString = "aTat";
                const string expectedText = "a a ";
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, testString);

                Assert.Equal(expectedText, _arbitraryPaper.Text);
            }

            [Fact]
            public void ShouldDegradeByOneWhenWritingInLowercase()
            {
                const string lowercaseLetter = "a";
                const int lowercaseDegradeValue = 1;
                const int startDurability = 5;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, lowercaseLetter);

                Assert.Equal(startDurability - lowercaseDegradeValue, pencil.CurrentDurability);
            }

            [Theory]
            [InlineData("ab")]
            [InlineData("cdefg")]
            [InlineData("hijklmn")]
            [InlineData("opqrstuvwxyz")]
            public void ShouldDegradeCorrectlyForManyLowercaseLetters(string lowercaseLetters)
            {
                const int startDurability = 20;
                int expectedDurability = startDurability - lowercaseLetters.Length;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, lowercaseLetters);

                Assert.Equal(expectedDurability, pencil.CurrentDurability);
            }

            [Fact]
            public void ShouldDegradeByTwoWhenWritingInUppercase()
            {
                const string uppercaseLetter = "A";
                const int uppercaseDegradeValue = 2;
                const int startDurability = 5;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, uppercaseLetter);

                Assert.Equal(startDurability - uppercaseDegradeValue, pencil.CurrentDurability);
            }

            [Theory]
            [InlineData("AB")]
            [InlineData("CDEFG")]
            [InlineData("HIJKLMN")]
            [InlineData("OPQRSTUVWXYZ")]
            public void ShouldDegradeCorrectlyForManyUppercaseLetters(string uppercaseLetters)
            {
                const int startDurability = 40;
                const int uppercaseDegradeValue = 2;
                int expectedDurability = startDurability - (uppercaseLetters.Length * uppercaseDegradeValue);
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, uppercaseLetters);

                Assert.Equal(expectedDurability, pencil.CurrentDurability);
            }

            [Theory]
            [InlineData("Ah", 3)]
            [InlineData("rD", 3)]
            [InlineData("LEMDalsjLEKD", 20)]
            [InlineData("lsieLDOIlskd", 16)]
            [InlineData("LeLoFlEo", 12)]
            [InlineData("eLfOrG", 9)]
            public void ShouldDegradeCorrectlyForMixedCase(string mixedCase, int degradeAmount)
            {
                const int startDurability = 40;
                int expectedDurability = startDurability - degradeAmount;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, mixedCase);

                Assert.Equal(expectedDurability, pencil.CurrentDurability);
            }

            [Theory]
            [InlineData("A  h", 3)]
            [InlineData("\trD", 3)]
            [InlineData("L\nEM\tDa\nlsjL\r\nEKD", 20)]
            [InlineData("\vlsi\neLD\vOIl\tskd\v", 16)]
            [InlineData("  L\teL oFlEo  ", 12)]
            [InlineData("eL   f\tOrG", 9)]
            public void ShouldDegradeCorrectlyForMixedCaseAndWhitespace(string mixedString, int degradeAmount)
            {
                const int startDurability = 40;
                int expectedDurability = startDurability - degradeAmount;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, mixedString);

                Assert.Equal(expectedDurability, pencil.CurrentDurability);
            }

            [Theory]
            [InlineData("jT", 1)]
            [InlineData("t", 0)]
            [InlineData("asdfgh", 3)]
            [InlineData("asdT", 0)]
            [InlineData("TTs dfkD SFg ewe", 10)]
            public void ShouldNeverBeNegative(string sentence, int startDurability)
            {
                const int noDurability = 0;
                var pencil = new Pencil(startDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, sentence);

                Assert.Equal(noDurability, pencil.CurrentDurability);
            }
        }

        public class SharpeningPencil : PencilTests
        {
            [Fact]
            public void ShouldResetDurabilityToOriginalValue()
            {
                const int startingDurability = 100;
                const string testSentence = "Text to waste pencil durability for testing sharpening abilities";
                var pencil = new Pencil(startingDurability, _arbitraryLength);

                pencil.Write(_arbitraryPaper, testSentence);
                pencil.Sharpen();

                Assert.Equal(startingDurability, pencil.CurrentDurability);
            }

            [Fact]
            public void ShouldReducePencilLengthByOne()
            {
                const int startingLength = 5;
                const int expectedLength = 4;
                var pencil = new Pencil(_arbitraryDurability, startingLength);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }

            // if length is 0, sharpening does not reset durability
            // sharpening can't sharpen into negative length
        }
    }
}
