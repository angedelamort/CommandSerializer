using CommandSerializer.Utils;
using NUnit.Framework;

namespace ConsoleArgsTests
{
    public class ConsoleStringBuilderTest
    {
        [Test]
        public void TestEmpty()
        {
            var builder = new ConsoleStringBuilder(8);
            Assert.IsEmpty(builder.ToString());
        }

        [Test]
        public void TestOneCharacter()
        {
            var builder = new ConsoleStringBuilder(8);
            builder.Append("1");
            Assert.AreEqual("1", builder.ToString());
        }

        [Test]
        public void TestWordWrap()
        {
            var builder = new ConsoleStringBuilder(8);
            builder.Append("This is a super long text on 2 lines.");

            Assert.AreEqual(@"This is
a super
long
text on
2 lines.", builder.ToString());
        }

        [Test]
        public void TestWordWrapWithOverflow()
        {
            var builder = new ConsoleStringBuilder(10);
            builder.Append("This is a super long text on 2 lines.", 2);

            Assert.AreEqual(@"This is a
  super
  long
  text on
  2 lines.", builder.ToString());
        }

        [Test]
        public void TestWordWrapWithMultipleSpaces()
        {
            var builder = new ConsoleStringBuilder(10);
            builder.Append("This is a super     long text on 2 lines.", 2);

            Assert.AreEqual(@"This is a
  super
  long
  text on
  2 lines.", builder.ToString());
        }

        [Test]
        public void TestWithTabsAndReturns()
        {
            var builder = new ConsoleStringBuilder(10);
            builder.Append("\tHello\n\tWorld!");

            Assert.AreEqual(@"    Hello
    World!", builder.ToString());
        }

        [Test]
        public void TestWithSuperLongStringThatCannotWrap()
        {
            var builder = new ConsoleStringBuilder(8);
            builder.Append("ThisIsTheSuperLongSentenceThatCannotBeSplitInMultiline.", 2);

            Assert.AreEqual(@"ThisIsTh
  eSuper
  LongSe
  ntence
  ThatCa
  nnotBe
  SplitI
  nMulti
  line.", builder.ToString());
        }
    }
}
