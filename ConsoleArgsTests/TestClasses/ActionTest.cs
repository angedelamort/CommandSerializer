using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ActionTest
    {
        [Parameter(Action = "set-me")]
        public bool IsSet { get; set; }
    }
}
