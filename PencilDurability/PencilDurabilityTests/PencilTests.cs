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
    }
}
