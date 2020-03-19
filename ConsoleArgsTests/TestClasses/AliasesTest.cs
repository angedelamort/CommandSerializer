using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class AliasesTest
    {
        [Parameter(Alias = 's')]
        public bool Set1 { get; set; }

        [Parameter(Alias = 'S')]
        public bool Set2 { get; set; }

        [Parameter(Alias = 't')]
        public bool Set3 { get; set; }

        [Parameter(Alias = 'v')]
        public string Val { get; set; }
    }
}
