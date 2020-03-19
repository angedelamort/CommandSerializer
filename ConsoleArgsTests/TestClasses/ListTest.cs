using System.Collections.Generic;
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ListTest
    {
        [Parameter(Alias = 'c')]
        public List<string> Color { get; set; }
    }
}
