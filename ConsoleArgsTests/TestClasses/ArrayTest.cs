using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ArrayTest
    {
        [Parameter(Action = "array")]
        public int[] Numbers { get; set; }
    }
}
