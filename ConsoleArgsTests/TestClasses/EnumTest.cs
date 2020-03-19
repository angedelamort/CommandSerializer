using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class EnumTest
    {
        public enum Color
        {
            Red,
            Green, 
            Blue
        }

        [Parameter(Action = "color")]
        public Color Col { get; set; }
    }
}
