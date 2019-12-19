using System;
using Xunit;
using PencilDurability;

namespace PencilDurabilityTests
{
    public class PencilTests
    {
        [Fact]
        public void PencilShouldWriteToPaper()
        {
            var pencil = new Pencil();
            var paper = new Paper();
            const string testSentence = "This is a sentence.";

            pencil.Write(paper, testSentence);

            Assert.Equal(testSentence, paper.Text);
        }

        [Fact]
        public void WritingShouldAppendToPreExistingText()
        {
            var pencil = new Pencil();
            var paper = new Paper();
            const string testSentence1 = "This is a sentence.";
            const string testSentence2 = " This is another sentence.";
            paper.Text = testSentence1;

            pencil.Write(paper, testSentence2);

            Assert.Equal(testSentence1 + testSentence2, paper.Text);
        }

        [Fact]
        public void PencilWithNoDurabilityWritesCorrectNumberOfSpaces()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();
            const string testSentence = "This should not be written.";
            const string expectedSpaces = "                           ";

            pencil.Write(paper, testSentence);

            Assert.Equal(expectedSpaces, paper.Text);
        }

        [Fact]
        public void PencilWithNoDurabilityAppendsSpacesToText()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();

            const string preexistingText = "This already exists.";
            const string testSentence = " This should not be written.";
            const string expectedSpaces = "                            ";

            paper.Text = preexistingText;

            pencil.Write(paper, testSentence);

            Assert.Equal(preexistingText + expectedSpaces, paper.Text);
        }

        [Fact]
        public void PencilWithNoDurabilityStillWritesWhitespaceCharacters()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();
            const string testWhitespace = "  \t\r\n\f\v  ";

            pencil.Write(paper, testWhitespace);

            Assert.Equal(testWhitespace, paper.Text);
        }

        [Fact]
        public void PencilWithNoDurabilityKeepsWhitespaceInMiddleOfText()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();
            const string testSentence = "\vT\this is a\t sentence\n with whi\r\ntespace chara\ncters.\f";
            const string expectedText = "\v \t        \t         \n         \r\n             \n      \f";

            pencil.Write(paper, testSentence);

            Assert.Equal(expectedText, paper.Text);
        }

        [Fact]
        public void DurabilityShouldDegradeByOneWritingLowercase()
        {
            const string lowercaseLetter = "a";
            const int lowercaseDegradeValue = 1;
            const int startDurability = 5;
            var pencil = new Pencil(startDurability);
            var paper = new Paper();

            pencil.Write(paper, lowercaseLetter);

            Assert.Equal(startDurability - lowercaseDegradeValue, pencil.CurrentDurability);
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("cdefg")]
        [InlineData("hijklmn")]
        [InlineData("opqrstuvwxyz")]
        public void DurabilityShouldDegradeCorrectlyForManyLowercase(string lowercaseLetters)
        {
            const int startDurability = 20;
            int expectedDurability = startDurability - lowercaseLetters.Length;
            var pencil = new Pencil(startDurability);
            var paper = new Paper();

            pencil.Write(paper, lowercaseLetters);

            Assert.Equal(expectedDurability, pencil.CurrentDurability);
        }

        [Fact]
        public void DurabilityShouldDegradeByTwoWritingUppercase()
        {
            const string uppercaseLetter = "A";
            const int uppercaseDegradeValue = 2;
            const int startDurability = 5;
            var pencil = new Pencil(startDurability);
            var paper = new Paper();

            pencil.Write(paper, uppercaseLetter);

            Assert.Equal(startDurability - uppercaseDegradeValue, pencil.CurrentDurability);
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("CDEFG")]
        [InlineData("HIJKLMN")]
        [InlineData("OPQRSTUVWXYZ")]
        public void DurabilityShouldDegradeCorrectlyForManyUppercase(string uppercaseLetters)
        {
            const int startDurability = 40;
            const int uppercaseDegradeValue = 2;
            int expectedDurability = startDurability - (uppercaseLetters.Length * uppercaseDegradeValue);
            var pencil = new Pencil(startDurability);
            var paper = new Paper();

            pencil.Write(paper, uppercaseLetters);

            Assert.Equal(expectedDurability, pencil.CurrentDurability);
        }

        [Theory]
        [InlineData("Ah", 3)]
        [InlineData("rD", 3)]
        [InlineData("LEMDalsjLEKD", 20)]
        [InlineData("lsieLDOIlskd", 16)]
        [InlineData("LeLoFlEo", 12)]
        [InlineData("eLfOrG", 9)]
        public void DurabilityShouldDegradeCorrectlyForMixedCase(string mixedCase, int degradeAmount)
        {
            const int startDurability = 40;
            int expectedDurability = startDurability - degradeAmount;
            var pencil = new Pencil(startDurability);
            var paper = new Paper();

            pencil.Write(paper, mixedCase);

            Assert.Equal(expectedDurability, pencil.CurrentDurability);
        }
        
        // whitespace degrades by 0 (\t\r\n\f\v) & ' '
        // lowercase degrade by 1
        // capitals degrade by 2

        // what about punctuation and numbers?
        //  (1p?)  qwertyuiopasdfghjklzxcvbnm 1234567890  |:"<>?\;',./*_+^`~![]{}()
        //  (2p?)  QWERTYUIOPASDFGHJKLZXCVBNM &@#$%

        // what happens when you have 1 point and try to write a capital?
        //    writes a space and doesn't degrade?
    }
}
