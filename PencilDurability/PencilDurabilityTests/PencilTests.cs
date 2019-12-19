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
    }
}
