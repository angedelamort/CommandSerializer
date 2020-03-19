using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class AliasTest
    {
        [Parameter(Alias = 's')]
        public bool IsSet { get; set; }
    }
}
