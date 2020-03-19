using System.Collections.Generic;
using CommandSerializer.Attributes;

namespace ConsoleArgsTests.TestClasses
{
    [CommandManual(Description = @"Initialize a car using command line arguments

We can set multiple parameters for the car. Check the options.

The NAME is the name of the car.

Example:
  Set a car in automatic mode:
      test -automatic

Author:
  AngeDeLaMort
")]
    public class HelpTest
    {
        [Parameter(Action = "automatic", HelpText = "Set the car to automatic")] 
        public bool IsAutomatic { get; set; }

        [Parameter(Action = "colors", Alias = 'c', HelpText = "Set a list of colors (-c red blue ...)")]
        public List<string> Colors { get; set; }

        [Parameter(Alias = 'b', Required = true, HelpText = "Set the brand name")]
        public string Brand { get; set; }

        [PositionalParameter(Name = "NAME", Required = true)]
        public string Name { get; set; }
    }
}
