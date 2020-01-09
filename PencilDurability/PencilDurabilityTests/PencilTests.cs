using Xunit;
using PencilDurability;
using System;

namespace PencilDurabilityTests
{
    public class PencilTests
    {
        private const int _arbitraryDurability = 999999;
        private const int _arbitraryLength = 99999;
        private const int _arbitraryEraserDurability = 999999;

        private IPencil GetPencil(int pointDurability = _arbitraryDurability,
                                   int length = _arbitraryLength,
                                   int eraserDurability = _arbitraryEraserDurability)
        {
            return new Pencil(pointDurability, length, eraserDurability);
        }

        private IPaper GetPaper(string paperText = "")
        {
            return new Paper() { Text = paperText };
        }

        public class Initialization : PencilTests
        {
            [Fact]
            public void ShouldMakeValuePositiveForNegativeDurability()
            {
                const int startPointDurability = -10;
                const int expectedPointDurability = 10;

                IPencil pencil = GetPencil(pointDurability: startPointDurability);

                Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldSharpenToThePositiveValueForNegativeDurability()
            {
                const int startPointDurability = -10;
                const int expectedPointDurability = 10;
                IPencil pencil = GetPencil(pointDurability: startPointDurability);

                pencil.Sharpen();

                Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldMakeValuePositiveForNegativeLength()
            {
                const int startLength = -10;
                const int expectedLength = 10;

                IPencil pencil = GetPencil(length: startLength);

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }

            [Fact]
            public void ShouldMakeValuePositiveForNegativeEraserDurability()
            {
                const int startEraserDurability = -10;
                const int expectedEraserDurability = 10;

                IPencil pencil = GetPencil(eraserDurability: startEraserDurability);

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
                    const string testSentence = "This should not be written.";
                    const string expectedString = "                           ";
                    const int pointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testSentence);

                    Assert.Equal(expectedString, paper.Text);
                }

                [Fact]
                public void ShouldAppendSpacesToText()
                {
                    const string paperText = "This already exists.";
                    const string testSentence = " This should not be written.";
                    const string expectedText = "This already exists.                            ";
                    const int pointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Write(paper, testSentence);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldWriteWhitespaceCharacters()
                {
                    const string testWhitespace = "  \t\r\n\f\v  ";
                    const int pointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testWhitespace);

                    Assert.Equal(testWhitespace, paper.Text);
                }

                [Fact]
                public void ShouldKeepWhitespaceInMiddleOfText()
                {
                    const string testSentence = "\vT\this is a\t sentence\n with whi\r\ntespace chara\ncters.\f";
                    const string expectedText = "\v \t        \t         \n         \r\n             \n      \f";
                    const int pointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testSentence);

                    Assert.Equal(expectedText, paper.Text);
                }
            }

            public class WithDurability : Writing
            {
                [Fact]
                public void ShouldAddTextToPaper()
                {
                    const string testSentence = "This is a sentence";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testSentence);

                    Assert.Equal(testSentence, paper.Text);
                }

                [Fact]
                public void ShouldAppendToExistingText()
                {
                    const string paperText = "This is a sentence";
                    const string testSentence = " This is another sentence";
                    const string expectedText = paperText + testSentence;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Write(paper, testSentence);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldWritePartOfTheSentenceWithoutEnoughDurability()
                {
                    const string testSentence = "This should not be written and This should not be written";
                    const string expectedSentence = "This should not be written and                           ";
                    const int pointDurability = 26;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testSentence);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldNotWriteUppercaseIfNotEnoughDurability()
                {
                    const string testString = "aTat";
                    const string expectedText = "a a ";
                    const int pointDurability = 2;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper();

                    pencil.Write(paper, testString);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldDegradeByOneWhenWritingInLowercase()
                {
                    const string lowercaseLetter = "a";
                    const int startPointDurability = 5;
                    const int expectedPointDurability = 4;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), lowercaseLetter);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("ab")]
                [InlineData("cdefg")]
                [InlineData("hijklmn")]
                [InlineData("opqrstuvwxyz")]
                public void ShouldDegradeCorrectlyForManyLowercaseLetters(string lowercaseLetters)
                {
                    const int startPointDurability = 20;
                    int expectedPointDurability = startPointDurability - lowercaseLetters.Length;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), lowercaseLetters);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldDegradeByTwoWhenWritingInUppercase()
                {
                    const string uppercaseLetter = "A";
                    const int startPointDurability = 5;
                    const int expectedPointDurability = 3;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), uppercaseLetter);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("AB")]
                [InlineData("CDEFG")]
                [InlineData("HIJKLMN")]
                [InlineData("OPQRSTUVWXYZ")]
                public void ShouldDegradeCorrectlyForManyUppercaseLetters(string uppercaseLetters)
                {
                    const int uppercaseDegradeValue = 2;
                    const int startPointDurability = 40;
                    int expectedPointDurability = startPointDurability - (uppercaseLetters.Length * uppercaseDegradeValue);
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), uppercaseLetters);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldDefaultDegradingByOneIfNotWhitespaceOrUppercase()
                {
                    const string otherCharecter = "?";
                    const int startPointDurability = 5;
                    const int expectedPointDurability = 4;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), otherCharecter);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("321654987")]
                [InlineData(")(*&^%%#$@#$!~`_+-=")]
                [InlineData("{}:\">?<[];',.|\\")]
                public void ShouldDegradCorrectlyforManyRandomCharacters(string miscCharacters)
                {
                    const int startPointDurability = 20;
                    int expectedPointDurability = startPointDurability - miscCharacters.Length;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), miscCharacters);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
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
                    const int startPointDurability = 40;
                    int expectedPointDurability = startPointDurability - degradeAmount;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), mixedCase);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
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
                    const int startPointDurability = 40;
                    int expectedPointDurability = startPointDurability - degradeAmount;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), mixedString);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Theory]
                [InlineData("jT", 1)]
                [InlineData("t", 0)]
                [InlineData("TTs dfkD SFg ewe", 10)]
                public void ShouldNeverBecomeNegative(string sentence, int startPointDurability)
                {
                    const int expectedPointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);

                    pencil.Write(GetPaper(), sentence);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
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
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(testWord);

                    pencil.Erase(paper, testWord);

                    Assert.DoesNotContain(testWord, paper.Text);
                }

                [Theory]
                [InlineData("sometext", "        ")]
                [InlineData("sometextvaryinglength", "                     ")]
                public void ShouldReplaceMatchingTextWithCorrectAmountOfSpaces(string testWord, string expected)
                {
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(testWord);

                    pencil.Erase(paper, testWord);

                    Assert.Equal(expected, paper.Text);
                }

                [Fact]
                public void ShouldReplaceOnlyMatchingText()
                {
                    const string paperText = "this is a test sentence";
                    const string eraseWord = "test";
                    const string expectedSentence = "this is a      sentence";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseWord);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldOnlyReplaceTheLastMatch()
                {
                    const string paperText = "Not this but this";
                    const string eraseWord = "this";
                    const string expectedSentence = "Not this but     ";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseWord);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldDoNothingIfNoMatchIsFound()
                {
                    const string testSentence = "Not this or this";
                    const string eraseWord = "test";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(testSentence);

                    pencil.Erase(paper, eraseWord);

                    Assert.Equal(testSentence, paper.Text);
                }

                [Fact]
                public void ShouldReplaceWhitespaceWithSpaces()
                {
                    const string testWhitespace = "  \t\r\n\f\v  ";
                    const string expected = "         ";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(testWhitespace);

                    pencil.Erase(paper, testWhitespace);

                    Assert.Equal(expected, paper.Text);
                }

                [Fact]
                public void ShouldMatchOnCaseWhenMatching()
                {
                    const string paperText = "This but not this";
                    const string eraseWord = "This";
                    const string expectedSentence = "     but not this";
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseWord);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldDegradeByOneWhenErasingOneCharacter()
                {
                    const string paperText = "This is a sentence";
                    const string eraseLetter = "a";
                    const int startEraserDurability = 5;
                    const int expectedEraserDurability = 4;
                    IPencil pencil = GetPencil(eraserDurability: startEraserDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseLetter);

                    Assert.Equal(expectedEraserDurability, pencil.CurrentEraserDurability);
                }

                [Theory]
                [InlineData("Ah")]
                [InlineData("lsieLDOkd")]
                [InlineData("0?>9<8:6(*4&^2#1$%")]
                public void ShouldDegradeCorrectlyForManyCharacters(string eraseMatch)
                {
                    const int startEraserDurability = 50;
                    int expectedEraserDurability = startEraserDurability - eraseMatch.Length;
                    IPencil pencil = GetPencil(eraserDurability: startEraserDurability);
                    IPaper paper = GetPaper(eraseMatch);

                    pencil.Erase(paper, eraseMatch);

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
                    IPencil pencil = GetPencil(eraserDurability: startEraserDurability);
                    IPaper paper = GetPaper(mixedString);

                    pencil.Erase(paper, mixedString);

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
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(testSentence);

                    pencil.Erase(paper, testSentence);

                    Assert.Equal(testSentence, paper.Text);
                }

                [Fact]
                public void ShouldErasePartOfMatchingWord()
                {
                    const string testWord = "word";
                    const string expectedWord = "wo  ";
                    const int eraserDurability = 2;
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(testWord);

                    pencil.Erase(paper, testWord);

                    Assert.Equal(expectedWord, paper.Text);
                }

                [Fact]
                public void ShouldErasePartOfMatchingWordInSentence()
                {
                    const string paperText = "This is a word in a sentence.";
                    const string testWord = "word";
                    const string expectedSentence = "This is a wo   in a sentence.";
                    const int eraserDurability = 2;
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, testWord);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldEraseCorrectPartOfMatchThatIncludesWhitespace()
                {
                    const string paperText = "This is a test sentence.";
                    const string eraseSection = "is a test";
                    const string expectedSentence = "This i         sentence.";
                    const int eraserDurability = 6;
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseSection);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldEraseCorrectPartialMatch()
                {
                    const string paperText = "things running";
                    const string eraseWord = "thing";
                    const string expectedSentence = "th   s running";
                    const int eraserDurability = 3;
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseWord);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Fact]
                public void ShouldNotEraseMatchingWhitespaceWhenDurabilityRunsOut()
                {
                    const string paperText = "This\n\n\nis a sentence.";
                    const string eraseMatch = "\n\n\nis a";
                    const string expectedSentence = "This\n\n\n     sentence.";
                    const int eraserDurability = 3;
                    IPencil pencil = GetPencil(eraserDurability: eraserDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Erase(paper, eraseMatch);

                    Assert.Equal(expectedSentence, paper.Text);
                }

                [Theory]
                [InlineData("jT", 1)]
                [InlineData("t", 0)]
                [InlineData("TTs dfkD SFg ewe", 10)]
                public void ShouldNeverBecomeNegative(string sentence, int startEraserDurability)
                {
                    const int expectedEraserDurability = 0;
                    IPencil pencil = GetPencil(eraserDurability: startEraserDurability);
                    IPaper paper = GetPaper(sentence);

                    pencil.Erase(paper, sentence);

                    Assert.Equal(expectedEraserDurability, pencil.CurrentEraserDurability);
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
                    const string paperText = "  \t\r\n\f\v  ";
                    const string editText = "editing";
                    const string expectedText = " editing ";
                    const int startIndex = 1;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldOverwriteExistingTextWithConflictCharacter()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = "Added";
                    const string expectedText = "This is a se@@@@@e.";
                    const int startIndex = 12;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldNotOverwriteTextWithWhitespace()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = " \t\r\n\f\v ";
                    const string expectedText = "This is a sentence.";
                    const int startIndex = 11;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldOverwriteWhitespaceWithOtherWhitespace()
                {
                    const string paperText = "               ";
                    const string editText = " \t\r\n\f\v ";
                    const string expectedText = "    \t\r\n\f\v      ";
                    const int startIndex = 3;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldAppendToPaperWhenStartIndexIsTooLarge()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = " This is another sentence.";
                    const string expectedText = "This is a sentence. This is another sentence.";
                    const int startIndex = 19;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldOverwriteBeyondExistingTextLength()
                {
                    const string paperText = "          ";
                    const string editText = "This is a sentence.";
                    const string expectedText = "     This is a sentence.";
                    const int startIndex = 5;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldCorrectlyConflictWithTextAndOverwriteWhitespace()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = "overwrite";
                    const string expectedText = "Thi@v@@w@i@@ntence.";
                    const int startIndex = 3;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldSetNegativeStartIndexToZero()
                {
                    const string paperText = "          ";
                    const string editText = "Negative";
                    const string expectedText = "Negative  ";
                    const int startIndex = -20;
                    IPencil pencil = GetPencil();
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }
            }

            public class WithDurability : Editing
            {
                [Fact]
                public void ShouldCorrectlyDegradeWhenAppendedToTheEnd()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = " This is another sentence.";
                    const int startIndex = 19;
                    const int startPointDurability = 123;
                    const int expectedPointDurability = 100;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldCorrectlyDegradeWhenOverwritingText()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = "overwrite";
                    const int startIndex = 3;
                    const int startPointDurability = 129;
                    const int expectedPointDurability = 120;
                    IPencil pencil = GetPencil(pointDurability: startPointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
                }

                [Fact]
                public void ShouldWritePartialEditWhenDurabilityRunsOut()
                {
                    const string paperText = "This is a sentence.";
                    const string editText = "overwrite";
                    const string expectedText = "Thi@v@@wa sentence.";
                    const int startIndex = 3;
                    const int pointDurability = 5;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldNotWriteUppercaseIfNotEnoughDurability()
                {
                    const string paperText = "Word     Word";
                    const string editText = "tTt";
                    const string expectedText = "Word t t Word";
                    const int startindex = 5;
                    const int pointDurability = 2;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startindex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldAddSpacesBeyondExistingTextLengthIfBecomesDull()
                {
                    const string paperText = "          ";
                    const string editText = "This is a sentence.";
                    const string expectedText = "     This is a          ";
                    const int startIndex = 5;
                    const int pointDurability = 8;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }

                [Fact]
                public void ShouldWriteWhitespaceIfNoDurability()
                {
                    const string paperText = "Word       Word";
                    const string editText = "a\t\r\n\f\va";
                    const string expectedText = "Word \t\r\n\f\v Word";
                    const int startIndex = 4;
                    const int pointDurability = 0;
                    IPencil pencil = GetPencil(pointDurability: pointDurability);
                    IPaper paper = GetPaper(paperText);

                    pencil.Edit(paper, editText, startIndex);

                    Assert.Equal(expectedText, paper.Text);
                }
            }
        }

        public class Sharpening : PencilTests
        {
            [Fact]
            public void ShouldResetDurabilityToOriginalValue()
            {
                const string testSentence = "Text to waste pencil durability for testing sharpening abilities";
                const int pointDurability = 100;
                IPencil pencil = GetPencil(pointDurability: pointDurability);

                pencil.Write(GetPaper(), testSentence);
                pencil.Sharpen();

                Assert.Equal(pointDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldReducePencilLengthByOne()
            {
                const int startLength = 5;
                const int expectedLength = 4;
                IPencil pencil = GetPencil(length: startLength);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }

            [Fact]
            public void ShouldNotResetDurabilityWhenZeroLength()
            {
                const string testSentence = "Keep it simple";
                const int length = 0;
                const int startPointDurability = 100;
                const int expectedPointDurability = 87;
                IPencil pencil = GetPencil(pointDurability: startPointDurability, length: length);

                pencil.Write(GetPaper(), testSentence);
                pencil.Sharpen();

                Assert.Equal(expectedPointDurability, pencil.CurrentPointDurability);
            }

            [Fact]
            public void ShouldNotAllowLengthToBecomeNegative()
            {
                const int startLength = 0;
                const int expectedLength = 0;
                IPencil pencil = GetPencil(length: startLength);

                pencil.Sharpen();

                Assert.Equal(expectedLength, pencil.CurrentLength);
            }
        }
    }
}
