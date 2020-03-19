using System;
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class DateTest
    {
        [Parameter(Action = "date")]
        public DateTime Date { get; set; }
    }
}
