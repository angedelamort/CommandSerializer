
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    public class ActionsTestWithInputString
    {
        [Parameter(Action = "name")]
        public string Name{ get; set; }

        [Parameter(Action = "password")]
        public string Password { get; set; }
    }
}
