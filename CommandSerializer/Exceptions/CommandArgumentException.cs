using System;

namespace CommandSerializer.Exceptions
{
    public class CommandArgumentException : Exception
    {
        public CommandArgumentException(string message) : base(message) { }
    }
}
