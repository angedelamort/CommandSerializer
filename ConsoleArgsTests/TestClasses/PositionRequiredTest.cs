using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class PositionalRequiredTest
    {
        [PositionalParameter(Name = "COLOR", Required = true)]
        public string Color { get; set; }
    }
}
