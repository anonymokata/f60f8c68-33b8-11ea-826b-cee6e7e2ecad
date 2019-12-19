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

        // whitespace degrades by 0 (\t\r\n\f\v) & ' '
        // lowercase degrade by 1
        // capitals degrade by 2
    }
}
