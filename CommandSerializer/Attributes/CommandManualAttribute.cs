using System;

namespace CommandSerializer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandManualAttribute : Attribute
    {
        public string Description { get; set; }
    }
}
