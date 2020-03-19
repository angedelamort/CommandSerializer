using System;

namespace CommandSerializer.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public char Alias { get; set; }
        public string Action { get; set; }
        public string HelpText { get; set; }

        public bool Required { get; set; }

        public Type Converter { get; set; }
    }
}
