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
        public void PencilWithNoDurabilityWritesWhitespace()
        {
            const int noDurability = 0;
            var pencil = new Pencil(noDurability);
            var paper = new Paper();
            const string testSentence = "This should not be written.";

            pencil.Write(paper, testSentence);

            Assert.Empty(paper.Text);
        }
        // after pencil is dull, all chars appear as whitespace

        // whitespace degrades by 0
        // lowercase degrade by 1
        // capitals degrade by 2
    }
}
