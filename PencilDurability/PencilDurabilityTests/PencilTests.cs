using Xunit;
using PencilDurability;
using System;

namespace PencilDurabilityTests
{
    public class PencilTests
    {
        // TODO: all characters not explicitly Uppercase or whitespace will be considered lowercase.
        private readonly Paper _paper;
        private readonly int _arbitraryDurability;
        private readonly int _arbitraryLength;
        private readonly int _arbitraryEraserDurability;

        public PencilTests()
        {
            _paper = new Paper();
            _arbitraryDurability = 999999;
            _arbitraryLength = 99999;
            _arbitraryEraserDurability = 999999;
        }

        public class Writing : PencilTests
        {
            public class WithoutDurability : Writing
            {
                [Fact]
                public void ShouldWriteCorrectNumberOfSpaces()
                {
                    const int noDurability = 0;
                    var pencil = new Pencil(noDurability, _arbitraryLength, _arbitraryEraserDurability);
                    const string testSentence = "This should not be written.";
                    const string expectedSpaces = "                           ";

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedSpaces, _paper.Text);
                }

                [Fact]
                public void ShouldAppendSpacesToText()
                {
                    const int noDurability = 0;
                    var pencil = new Pencil(noDurability, _arbitraryLength, _arbitraryEraserDurability);

                    const string preexistingText = "This already exists.";
                    const string testSentence = " This should not be written.";
                    const string expectedSpaces = "                            ";

                    _paper.Text = preexistingText;

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(preexistingText + expectedSpaces, _paper.Text);
                }

                [Fact]
                public void ShouldWriteWhitespaceCharacters()
                {
                    const int noDurability = 0;
                    var pencil = new Pencil(noDurability, _arbitraryLength, _arbitraryEraserDurability);
                    const string testWhitespace = "  \t\r\n\f\v  ";

                    pencil.Write(_paper, testWhitespace);

                    Assert.Equal(testWhitespace, _paper.Text);
                }

                [Fact]
                public void ShouldKeepWhitespaceInMiddleOfText()
                {
                    const int noDurability = 0;
                    var pencil = new Pencil(noDurability, _arbitraryLength, _arbitraryEraserDurability);
                    const string testSentence = "\vT\this is a\t sentence\n with whi\r\ntespace chara\ncters.\f";
                    const string expectedText = "\v \t        \t         \n         \r\n             \n      \f";

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedText, _paper.Text);
                }
            }

            public class WithDurability : Writing
            {
                [Fact]
                public void ShouldAddTextToPaper()
                {
                    var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                    const string testSentence = "This is a sentence";

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(testSentence, _paper.Text);
                }

                [Fact]
                public void ShouldAppendToPreExistingText()
                {
                    var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                    const string testSentence1 = "This is a sentence";
                    const string testSentence2 = " This is another sentence";
                    _paper.Text = testSentence1;

                    pencil.Write(_paper, testSentence2);

                    Assert.Equal(testSentence1 + testSentence2, _paper.Text);
                }

                [Fact]
                public void ShouldWritePartOfTheSentenceWithoutEnoughDurability()
                {
                    const int durability = 26;
                    const string testSentence = "This should not be written and This should not be written";
                    const string expectedSentence = "This should not be written and                           ";
                    var pencil = new Pencil(durability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldNotWriteUppercaseIfNotEnoughDurability()
                {
                    const int startDurability = 2;
                    const string testString = "aTat";
                    const string expectedText = "a a ";
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, testString);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldDegradeByOneWhenWritingInLowercase()
                {
                    const string lowercaseLetter = "a";
                    const int lowercaseDegradeValue = 1;
                    const int startDurability = 5;
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, lowercaseLetter);

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
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, lowercaseLetters);

                    Assert.Equal(expectedDurability, pencil.CurrentDurability);
                }

                [Fact]
                public void ShouldDegradeByTwoWhenWritingInUppercase()
                {
                    const string uppercaseLetter = "A";
                    const int uppercaseDegradeValue = 2;
                    const int startDurability = 5;
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, uppercaseLetter);

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
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, uppercaseLetters);

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
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, mixedCase);

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
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, mixedString);

                    Assert.Equal(expectedDurability, pencil.CurrentDurability);
                }

                [Theory]
                [InlineData("jT", 1)]
                [InlineData("t", 0)]
                [InlineData("asdfgh", 3)]
                [InlineData("asdT", 0)]
                [InlineData("TTs dfkD SFg ewe", 10)]
                public void ShouldNeverBecomeNegative(string sentence, int startDurability)
                {
                    const int noDurability = 0;
                    var pencil = new Pencil(startDurability, _arbitraryLength, _arbitraryEraserDurability);

                    pencil.Write(_paper, sentence);

                    Assert.Equal(noDurability, pencil.CurrentDurability);
                }
            }
        }

        public class Sharpening : PencilTests
        {
            [Fact]
            public void ShouldResetDurabilityToOriginalValue()
            {
                const int startingDurability = 100;
                const string testSentence = "Text to waste pencil durability for testing sharpening abilities";
                var pencil = new Pencil(startingDurability, _arbitraryLength, _arbitraryEraserDurability);

                pencil.Write(_paper, testSentence);
                pencil.Sharpen();

                Assert.Equal(startingDurability, pencil.CurrentDurability);
            }

            [Fact]
            public void ShouldReducePencilLengthByOne()
            {
                const int startingLength = 5;
                const int expectedLength = 4;
                var pencil = new Pencil(_arbitraryDurability, startingLength, _arbitraryEraserDurability);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }

            [Fact]
            public void ShouldNotResetDurabilityWhenZeroLength()
            {
                const int startingDurability = 100;
                const int startingLength = 0;
                const string testSentence = "Keep it simple";
                const int expectedDurability = 87;
                var pencil = new Pencil(startingDurability, startingLength, _arbitraryEraserDurability);

                pencil.Write(_paper, testSentence);
                pencil.Sharpen();

                Assert.Equal(expectedDurability, pencil.CurrentDurability);
            }

            [Fact]
            public void ShouldNotAllowLengthToBecomeNegative()
            {
                const int startingLength = 0;
                const int expectedLength = 0;
                var pencil = new Pencil(_arbitraryDurability, startingLength, _arbitraryEraserDurability);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }
        }

        public class Erasing : PencilTests
        {
            [Fact]
            public void ShouldRemoveMatchingText()
            {
                const string testWord = "word";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testWord;

                pencil.Erase(_paper, testWord);

                Assert.DoesNotContain(testWord, _paper.Text);
            }

            [Theory]
            [InlineData("sometext", "        ")]
            [InlineData("sometextvaryinglength", "                     ")]
            public void ShouldReplaceMatchingTextWithSpaces(string testWord, string expected)
            {
                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testWord;

                pencil.Erase(_paper, testWord);

                Assert.Equal(expected, _paper.Text);
            }

            [Fact]
            public void ShouldReplaceOnlyMatchingText()
            {
                const string testSentence = "this is a test sentence";
                const string eraseWord = "test";
                const string expectedSentence = "this is a      sentence";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testSentence;

                pencil.Erase(_paper, eraseWord);

                Assert.Equal(expectedSentence, _paper.Text);
            }

            [Fact]
            public void ShouldOnlyReplaceTheLastMatch()
            {
                const string testSentence = "Not this but this";
                const string eraseWord = "this";
                const string expectedSentence = "Not this but     ";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testSentence;

                pencil.Erase(_paper, eraseWord);

                Assert.Equal(expectedSentence, _paper.Text);
            }

            [Fact]
            public void ShouldDoNothingIfNoMatchIsFound()
            {
                const string testSentence = "Not this or this";
                const string eraseWord = "test";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testSentence;

                pencil.Erase(_paper, eraseWord);

                Assert.Equal(testSentence, _paper.Text);
            }

            [Fact]
            public void ShouldReplaceWhitespaceWithSpaces()
            {
                const string testWhitespace = "  \t\r\n\f\v  ";
                const string expected = "         ";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testWhitespace;

                pencil.Erase(_paper, testWhitespace);

                Assert.Equal(expected, _paper.Text);
            }

            [Fact]
            public void ShouldNotIgnoreCaseWhenMatching()
            {
                const string testSentence = "This but not this";
                const string eraseWord = "This";
                const string expectedSentence = "     but not this";

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, _arbitraryEraserDurability);
                _paper.Text = testSentence;

                pencil.Erase(_paper, eraseWord);

                Assert.Equal(expectedSentence, _paper.Text);
            }

            [Fact]
            public void ShouldDegradeByOneWhenErasingOneCharacter()
            {
                const string testSentence = "This is a sentence";
                const string eraseLetter = "a";
                const int eraserDurability = 5;
                const int expectedDurability = 4;

                var pencil = new Pencil(_arbitraryDurability, _arbitraryLength, eraserDurability);
                _paper.Text = testSentence;

                pencil.Erase(_paper, eraseLetter);

                Assert.Equal(expectedDurability, pencil.CurrentEraserDurability);
            }

            // all characters degrade by 1 point
            //       Whitespace doesn't degrade eraser
            // should degrade correctly for mixed character and whitespace
            // Should erase part of match if not enough durability
            //      erase characters right to left

            // Can't erase with no durability
            // Eraser durability can't become negative
        }
    }
}
