using Xunit;
using PencilDurability;
using System;

namespace PencilDurabilityTests
{
    public class PencilTests
    {
        private readonly IPaper _paper;
        private const int _arbitraryDurability = 999999;
        private const int _arbitraryLength = 99999;
        private const int _arbitraryEraserDurability = 999999;

        // TODO: edit test: ShouldKeepWhitespaceInMiddleOfText

        public PencilTests()
        {
            _paper = new Paper();
        }

        private IPencil MakePencil(int pointDurability = _arbitraryDurability,
                                   int length = _arbitraryLength,
                                   int eraserDurability = _arbitraryEraserDurability)
        {
            return new Pencil(pointDurability, length, eraserDurability);
        }

        public class Initialization : PencilTests
        {
            [Fact]
            public void ShouldMakeValuePositiveForNegativeDurability()
            {
                const int negativeDurability = -10;
                const int expectedDurability = 10;

                IPencil pencil = MakePencil(pointDurability: negativeDurability);

                Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldSharpenToThePositiveValueForNegativeDurability()
            {
                const int negativeDurability = -10;
                const int expectedDurability = 10;
                IPencil pencil = MakePencil(pointDurability: negativeDurability);

                pencil.Sharpen();

                Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldMakeValuePositiveForNegativeLength()
            {
                const int negativeLength = -10;
                const int expectedLength = 10;

                IPencil pencil = MakePencil(length: negativeLength);

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }

            [Fact]
            public void ShouldMakeValuePositiveForNegativeEraserDurability()
            {
                const int negativeEraserDurability = -10;
                const int expectedEraserDurability = 10;

                IPencil pencil = MakePencil(eraserDurability: negativeEraserDurability);

                Assert.Equal(expectedEraserDurability, pencil.CurrentEraserDurability);
            }
        }

        public class Writing : PencilTests
        {
            public class WithoutDurability : Writing
            {
                [Fact]
                public void ShouldWriteCorrectNumberOfSpaces()
                {
                    const int noDurability = 0;
                    const string testSentence = "This should not be written.";
                    const string expectedSpaces = "                           ";
                    IPencil pencil = MakePencil(pointDurability: noDurability);

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedSpaces, _paper.Text);
                }

                [Fact]
                public void ShouldAppendSpacesToText()
                {
                    const int noDurability = 0;
                    const string preexistingText = "This already exists.";
                    const string testSentence = " This should not be written.";
                    const string expectedSpaces = "                            ";
                    IPencil pencil = MakePencil(pointDurability: noDurability);
                    _paper.Text = preexistingText;

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(preexistingText + expectedSpaces, _paper.Text);
                }

                [Fact]
                public void ShouldWriteWhitespaceCharacters()
                {
                    const int noDurability = 0;
                    const string testWhitespace = "  \t\r\n\f\v  ";
                    IPencil pencil = MakePencil(pointDurability: noDurability);

                    pencil.Write(_paper, testWhitespace);

                    Assert.Equal(testWhitespace, _paper.Text);
                }

                [Fact]
                public void ShouldKeepWhitespaceInMiddleOfText()
                {
                    const int noDurability = 0;
                    const string testSentence = "\vT\this is a\t sentence\n with whi\r\ntespace chara\ncters.\f";
                    const string expectedText = "\v \t        \t         \n         \r\n             \n      \f";
                    IPencil pencil = MakePencil(pointDurability: noDurability);

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedText, _paper.Text);
                }
            }

            public class WithDurability : Writing
            {
                [Fact]
                public void ShouldAddTextToPaper()
                {
                    const string testSentence = "This is a sentence";
                    IPencil pencil = MakePencil();

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(testSentence, _paper.Text);
                }

                [Fact]
                public void ShouldAppendToExistingText()
                {
                    const string testSentence1 = "This is a sentence";
                    const string testSentence2 = " This is another sentence";
                    IPencil pencil = MakePencil();
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
                    IPencil pencil = MakePencil(pointDurability: durability);

                    pencil.Write(_paper, testSentence);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldNotWriteUppercaseIfNotEnoughDurability()
                {
                    const int startDurability = 2;
                    const string testString = "aTat";
                    const string expectedText = "a a ";
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, testString);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldDegradeByOneWhenWritingInLowercase()
                {
                    const string lowercaseLetter = "a";
                    const int lowercaseDegradeValue = 1;
                    const int startDurability = 5;
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, lowercaseLetter);

                    Assert.Equal(startDurability - lowercaseDegradeValue, pencil.CurrentPointDurability);
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
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, lowercaseLetters);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldDegradeByTwoWhenWritingInUppercase()
                {
                    const string uppercaseLetter = "A";
                    const int uppercaseDegradeValue = 2;
                    const int startDurability = 5;
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, uppercaseLetter);

                    Assert.Equal(startDurability - uppercaseDegradeValue, pencil.CurrentPointDurability);
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
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, uppercaseLetters);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldDefaultDegradingByOneIfNotWhitespaceOrUppercase()
                {
                    const string otherCharecter = "?";
                    const int startDurability = 5;
                    const int expectedDurability = 4;
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, otherCharecter);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("321654987")]
                [InlineData(")(*&^%%#$@#$!~`_+-=")]
                [InlineData("{}:\">?<[];',.|\\")]
                public void ShouldDegradCorrectlyforManyRandomCharacters(string miscCharacters)
                {
                    const int startDurability = 20;
                    int expectedDurability = startDurability - miscCharacters.Length;
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, miscCharacters);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
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
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, mixedCase);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
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
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, mixedString);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("jT", 1)]
                [InlineData("t", 0)]
                [InlineData("TTs dfkD SFg ewe", 10)]
                public void ShouldNeverBecomeNegative(string sentence, int startDurability)
                {
                    const int noDurability = 0;
                    IPencil pencil = MakePencil(pointDurability: startDurability);

                    pencil.Write(_paper, sentence);

                    Assert.Equal(noDurability, pencil.CurrentPointDurability);
                }
            }
        }

        public class Erasing : PencilTests
        {
            public class WithEnoughDurability : PencilTests
            {
                [Fact]
                public void ShouldRemoveMatchingText()
                {
                    const string testWord = "word";
                    IPencil pencil = MakePencil();
                    _paper.Text = testWord;

                    pencil.Erase(_paper, testWord);

                    Assert.DoesNotContain(testWord, _paper.Text);
                }

                [Theory]
                [InlineData("sometext", "        ")]
                [InlineData("sometextvaryinglength", "                     ")]
                public void ShouldReplaceMatchingTextWithCorrectAmountOfSpaces(string testWord, string expected)
                {
                    IPencil pencil = MakePencil();
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
                    IPencil pencil = MakePencil();
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
                    IPencil pencil = MakePencil();
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseWord);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldDoNothingIfNoMatchIsFound()
                {
                    const string testSentence = "Not this or this";
                    const string eraseWord = "test";
                    IPencil pencil = MakePencil();
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseWord);

                    Assert.Equal(testSentence, _paper.Text);
                }

                [Fact]
                public void ShouldReplaceWhitespaceWithSpaces()
                {
                    const string testWhitespace = "  \t\r\n\f\v  ";
                    const string expected = "         ";
                    IPencil pencil = MakePencil();
                    _paper.Text = testWhitespace;

                    pencil.Erase(_paper, testWhitespace);

                    Assert.Equal(expected, _paper.Text);
                }

                [Fact]
                public void ShouldMatchOnCaseWhenMatching()
                {
                    const string testSentence = "This but not this";
                    const string eraseWord = "This";
                    const string expectedSentence = "     but not this";
                    IPencil pencil = MakePencil();
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
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseLetter);

                    Assert.Equal(expectedDurability, pencil.CurrentEraserDurability);
                }

                [Theory]
                [InlineData("Ah")]
                [InlineData("lsieLDOkd")]
                [InlineData("0?>9<8:6(*4&^2#1$%")]
                public void ShouldDegradeCorrectlyForManyCharacters(string eraseMatch)
                {
                    const int startEraserDurability = 50;
                    int expectedEraserDurability = startEraserDurability - eraseMatch.Length;
                    IPencil pencil = MakePencil(eraserDurability: startEraserDurability);
                    _paper.Text = eraseMatch;

                    pencil.Erase(_paper, eraseMatch);

                    Assert.Equal(expectedEraserDurability, pencil.CurrentEraserDurability);
                }

                [Theory]
                [InlineData("A h", 2)]
                [InlineData("L\nEM\tDa\nlsjL\r\nEKD", 12)]
                [InlineData("  \v0?>9\n<8:6 (*4&\t^2# 1$%  ", 18)]
                public void ShouldDegradeCorrectlyForMixedCharactersAndWhitespace(string mixedString, int degradeAmount)
                {
                    const int startEraserDurability = 50;
                    int expectedEraserDurability = startEraserDurability - degradeAmount;
                    IPencil pencil = MakePencil(eraserDurability: startEraserDurability);
                    _paper.Text = mixedString;

                    pencil.Erase(_paper, mixedString);

                    Assert.Equal(expectedEraserDurability, pencil.CurrentEraserDurability);
                }
            }

            public class WithoutEnoughDurability : Erasing
            {
                [Fact]
                public void ShouldDoNothingWithNoDurability()
                {
                    const string testSentence = "  \v0?>9\n<8:6 (*4&\t^2# 1$%  ";
                    const int eraserDurability = 0;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, testSentence);

                    Assert.Equal(testSentence, _paper.Text);
                }

                [Fact]
                public void ShouldErasePartOfMatchingWord()
                {
                    const string testWord = "word";
                    const string expectedWord = "wo  ";
                    const int eraserDurability = 2;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testWord;

                    pencil.Erase(_paper, testWord);

                    Assert.Equal(expectedWord, _paper.Text);
                }

                [Fact]
                public void ShouldErasePartOfMatchingWordInSentence()
                {
                    const string testWord = "word";
                    const string testSentence = "This is a word in a sentence.";
                    const string expectedSentence = "This is a wo   in a sentence.";
                    const int eraserDurability = 2;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, testWord);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldEraseCorrectPartOfMatchThatIncludesWhitespace()
                {
                    const string testSentence = "This is a test sentence.";
                    const string eraseSection = "is a test";
                    const string expectedSentence = "This i         sentence.";
                    const int eraserDurability = 6;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseSection);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldEraseCorrectPartialMatch()
                {
                    const string eraseWord = "thing";
                    const string testSentence = "things running";
                    const string expectedSentence = "th   s running";
                    const int eraserDurability = 3;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseWord);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Fact]
                public void ShouldNotEraseMatchingWhitespaceWhenDurabilityRunsOut()
                {
                    const string eraseMatch = "\n\n\nis a";
                    const string testSentence = "This\n\n\nis a sentence.";
                    const string expectedSentence = "This\n\n\n     sentence.";
                    const int eraserDurability = 3;
                    IPencil pencil = MakePencil(eraserDurability: eraserDurability);
                    _paper.Text = testSentence;

                    pencil.Erase(_paper, eraseMatch);

                    Assert.Equal(expectedSentence, _paper.Text);
                }

                [Theory]
                [InlineData("jT", 1)]
                [InlineData("t", 0)]
                [InlineData("TTs dfkD SFg ewe", 10)]
                public void ShouldNeverBecomeNegative(string sentence, int startDurability)
                {
                    const int expectedDurability = 0;
                    IPencil pencil = MakePencil(eraserDurability: startDurability);
                    _paper.Text = sentence;

                    pencil.Erase(_paper, sentence);

                    Assert.Equal(expectedDurability, pencil.CurrentEraserDurability);
                }
            }
        }

        public class Editing : PencilTests
        {
            public class Text : Editing
            {
                [Fact]
                public void ShouldOverwriteWhitespaceAtGivenIndex()
                {
                    const string testWhitespace = "  \t\r\n\f\v  ";
                    const string editText = "editing";
                    const string expectedText = " editing ";
                    const int startIndex = 1;
                    IPencil pencil = MakePencil();
                    _paper.Text = testWhitespace;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldOverwriteExistingTextWithConflictCharacter()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = "Added";
                    const string expectedText = "This is a se@@@@@e.";
                    const int startIndex = 12;
                    IPencil pencil = MakePencil();
                    _paper.Text = paperText;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldNotOverwriteTextWithWhitespace()
                {
                    const string testSentence = "This is a sentence.";
                    const string editText = " \t\r\n\f\v ";
                    const string expectedText = "This is a sentence.";
                    const int startIndex = 11;
                    IPencil pencil = MakePencil();
                    _paper.Text = testSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldOverwriteWhitespaceWithOtherWhitespace()
                {
                    const string testWhitespace = "               ";
                    const string editText = " \t\r\n\f\v ";
                    const string expectedText = "    \t\r\n\f\v      ";
                    const int startIndex = 3;
                    IPencil pencil = MakePencil();
                    _paper.Text = testWhitespace;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldAppendToPaperWhenStartIndexIsTooLarge()
                {
                    const string testSentence = "This is a sentence.";
                    const string editText = " This is another sentence.";
                    const string expectedText = "This is a sentence. This is another sentence.";
                    const int startIndex = 19;
                    IPencil pencil = MakePencil();
                    _paper.Text = testSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldOverwriteBeyondExistingTextLength()
                {
                    const string paperText = "          ";
                    const string editText = "This is a sentence.";
                    const string expectedText = "     This is a sentence.";
                    const int startIndex = 5;
                    IPencil pencil = MakePencil();
                    _paper.Text = paperText;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldCorrectlyConflictWithTextAndOverwriteWhitespace()
                {
                    const string testWhitespace = "This is a sentence.";
                    const string editText = "overwrite";
                    const string expectedText = "Thi@v@@w@i@@ntence.";
                    const int startIndex = 3;
                    IPencil pencil = MakePencil();
                    _paper.Text = testWhitespace;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldSetNegativeStartIndexToZero()
                {
                    const string textSentence = "          ";
                    const string editText = "Negative";
                    const string expectedText = "Negative  ";
                    const int startIndex = -20;
                    IPencil pencil = MakePencil();
                    _paper.Text = textSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }
            }

            public class WithDurability : Editing
            {
                [Fact]
                public void ShouldCorrectlyDegradeWhenAppendedToTheEnd()
                {
                    const string testSentence = "This is a sentence.";
                    const string editText = " This is another sentence.";
                    const int startIndex = 19;
                    const int pointDurability = 123;
                    const int expectedDurability = 100;
                    IPencil pencil = MakePencil(pointDurability: pointDurability);
                    _paper.Text = testSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldCorrectlyDegradeWhenOverwritingText()
                {
                    const string testSentence = "This is a sentence.";
                    const string editText = "overwrite";
                    const int startIndex = 3;
                    const int pointDurability = 129;
                    const int expectedDurability = 120;
                    IPencil pencil = MakePencil(pointDurability: pointDurability);
                    _paper.Text = testSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldWritePartialEditWhenDurabilityRunsOut()
                {
                    const string testSentence = "This is a sentence.";
                    const string editText = "overwrite";
                    const string expectedText = "Thi@v@@wa sentence.";
                    const int startIndex = 3;
                    const int pointDurability = 5;
                    IPencil pencil = MakePencil(pointDurability: pointDurability);
                    _paper.Text = testSentence;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldNotWriteUppercaseIfNotEnoughDurability()
                {
                    const string testText = "Word     Word";
                    const string editText = "tTt";
                    const string expectedText = "Word t t Word";
                    const int startindex = 5;
                    const int pointDurability = 2;
                    IPencil pencil = MakePencil(pointDurability: pointDurability);
                    _paper.Text = testText;

                    pencil.Edit(_paper, editText, startindex);

                    Assert.Equal(expectedText, _paper.Text);
                }

                [Fact]
                public void ShouldAddSpacesBeyondExistingTextLengthIfBecomesDull()
                {
                    const string paperText = "          ";
                    const string editText = "This is a sentence.";
                    const string expectedText = "     This is a          ";
                    const int startIndex = 5;
                    const int pointDurability = 8;
                    IPencil pencil = MakePencil(pointDurability: pointDurability);
                    _paper.Text = paperText;

                    pencil.Edit(_paper, editText, startIndex);

                    Assert.Equal(expectedText, _paper.Text);
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
                IPencil pencil = MakePencil(pointDurability: startingDurability);

                pencil.Write(_paper, testSentence);
                pencil.Sharpen();

                Assert.Equal(startingDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldReducePencilLengthByOne()
            {
                const int startingLength = 5;
                const int expectedLength = 4;
                IPencil pencil = MakePencil(length: startingLength);

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
                IPencil pencil = MakePencil(pointDurability: startingDurability, length: startingLength);

                pencil.Write(_paper, testSentence);
                pencil.Sharpen();

                Assert.Equal(expectedDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldNotAllowLengthToBecomeNegative()
            {
                const int startingLength = 0;
                const int expectedLength = 0;
                IPencil pencil = MakePencil(length: startingLength);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }
        }
    }
}
