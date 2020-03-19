using System;
using System.Collections.Generic;
using CommandSerializer.Utils;

namespace CommandSerializer
{
    public static class CommandSerializer<T> where T : new()
    {
        private static readonly Dictionary<Type, CommandLineParser<T>> Parsers = new Dictionary<Type, CommandLineParser<T>>();

        public static T Parse(string[] args)
        {
            var parser = GetArgParser();
            return parser.ParseArgs(args);
        }

        public static string GetHelp(int lineWidth = 80)
        {
            var argParser = GetArgParser();
            return argParser.ToString(lineWidth);
        }

        private static CommandLineParser<T> GetArgParser()
        {
            if (Parsers.TryGetValue(typeof(T), out var parser))
                return parser;

            parser = new CommandLineParser<T>();
            Parsers.Add(typeof(T), parser);
            return parser;
        }
    }
}
