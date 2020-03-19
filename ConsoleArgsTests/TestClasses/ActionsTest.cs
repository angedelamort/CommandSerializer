using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ActionsTest
    {
        [Parameter(Action = "set1")]
        public bool Set1 { get; set; }

        [Parameter(Action = "set2")]
        public bool Set2 { get; set; }

        [Parameter(Action = "set3")]
        public bool Set3 { get; set; }
    }
}
