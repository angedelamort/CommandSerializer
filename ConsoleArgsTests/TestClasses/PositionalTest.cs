using System.Collections.Generic;
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class PositionalTest
    {
        [PositionalParameter(Name = "COLOR")]
        public string Color { get; set; }

        [Parameter(Alias = 'a')]
        public bool IsAutomatic { get; set; }

        [PositionalParameter(Name = "FEATURE")]
        public List<string> Features { get; set; }
    }
}
