using System.Drawing;
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ConverterTest
    {
        [Parameter(Alias = 'c', Converter = typeof(ColorConverter))]
        public Color Color { get; set; }
    }
}
