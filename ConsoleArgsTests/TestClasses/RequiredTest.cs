using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class RequiredTest
    {
        [Parameter(Alias = 'v', Required = true)]
        public bool IsValid { get; set; }

        [Parameter(Action = "name", Required = true)]
        public string Name { get; set; }
    }
}
