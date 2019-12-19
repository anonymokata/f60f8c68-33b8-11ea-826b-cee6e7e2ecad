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
        public void PencilWithNoDurabilityStillWritesWhitespaceCharacters()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();
            const string testWhitespace = "  \t\r\n\f\v  ";

            pencil.Write(paper, testWhitespace);

            Assert.Equal(testWhitespace, paper.Text);
        }

        // pencil with no durability
        //    writes correct number of spaces
        //    does not exclude whitespace characters in the middle of text
        //    appends whitespace to existing text    

        // whitespace degrades by 0 (\t\r\n\f\v) & ' '
        // lowercase degrade by 1
        // capitals degrade by 2
    }
}
