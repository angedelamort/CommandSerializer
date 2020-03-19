using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommandSerializer.Attributes;
using CommandSerializer.Exceptions;

namespace CommandSerializer.Utils
{
    internal class CommandLineParser<T> where T : new()
    {
        private readonly Dictionary<string, ArgumentItem> actions = new Dictionary<string, ArgumentItem>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<char, ArgumentItem> aliases = new Dictionary<char, ArgumentItem>();
        private readonly List<ArgumentItem> explicitArguments = new List<ArgumentItem>();
        private readonly List<ArgumentItem> positionalArguments = new List<ArgumentItem>();

        public CommandLineParser()
        {
            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var positionalAttribute = propertyInfo.GetCustomAttribute<PositionalParameterAttribute>();
                if (positionalAttribute != null)
                {
                    // NOTE: kind of cheating and we should have some kind of interface - I don't want to do 2 ReadInput.
                    var item = new ArgumentItem(propertyInfo, positionalAttribute.ToParameterAttribute());
                    positionalArguments.Add(item);
                }
                else
                {
                    var item = new ArgumentItem(propertyInfo, propertyInfo.GetCustomAttribute<ParameterAttribute>());

                    if (item.GetAlias() != 0)
                        aliases.Add(item.GetAlias(), item);
                    if (!string.IsNullOrEmpty(item.GetAction()))
                        actions.Add(item.GetAction(), item);

                    explicitArguments.Add(item);
                }
            }
        }

        public T ParseArgs(string[] args)
        {
            var parsedItem = new T();
            var required = GetRequired();
            var aliasRegex = new Regex("^-([a-zA-Z0-9]+)$");
            var actionRegex = new Regex("^--([a-zA-Z0-9][a-zA-Z0-9_-]+)$"); // 2 and more characters
            var positionalIndex = 0;

            for (var i = 0; i < args.Length; i++)
            {
                var flag = args[i];
                var matchResult = aliasRegex.Match(flag);
                if (matchResult.Success)
                {
                    var characters = matchResult.Groups[1].Value.ToCharArray();
                    foreach (var character in characters)
                    {
                        if (aliases.TryGetValue(character, out var aliasItem))
                        {
                            if (characters.Length > 1 && aliasItem.PropertyInfo.PropertyType != typeof(bool))
                                throw new CommandArgumentException($"multiple flags should be boolean only. Flag -{character} is not.");

                            required.Remove(aliasItem);
                            ReadInput(parsedItem, aliasItem, args, ref i);
                        }
                        else
                        {
                            throw new CommandArgumentException($"Flag -{characters} doesn't exits.'");
                        }
                    }
                }
                else
                {
                    matchResult = actionRegex.Match(flag);
                    if (matchResult.Success)
                    {
                        var action = matchResult.Groups[1].Value;
                        if (actions.TryGetValue(action, out var actionItem))
                        {
                            required.Remove(actionItem);
                            ReadInput(parsedItem, actionItem, args, ref i);
                        }
                        else
                        {
                            throw new CommandArgumentException($"Flag --{action} doesn't exits.'");
                        }
                    }
                    else
                    {
                        if (positionalIndex < positionalArguments.Count)
                        {
                            var posItem = positionalArguments[positionalIndex];
                            ReadInput(parsedItem, posItem, args, ref i, true);
                            positionalIndex++;
                        }
                        else
                        {
                            throw new CommandArgumentException($"An unexpected arguments was found: {flag}");
                        }
                    }
                }

            }

            if (required.Count > 0)
            {
                var flags = string.Join(", ", required.Select(x => x.GetAction()));
                throw new CommandArgumentException($"Missing required parameters [{flags}] doesn't exits.'");
            }

            for (var i = positionalIndex; i < positionalArguments.Count; i++)
            {
                if (positionalArguments[positionalIndex].IsRequired())
                    throw new CommandArgumentException($"Missing required positional parameters [{positionalArguments[positionalIndex].GetAction()}] doesn't exits.'");
            }

            return parsedItem;
        }

        private void ReadInput(T newItem, ArgumentItem argumentItem, string[] args, ref int i, bool isPositional = false)
        {
            if (argumentItem.PropertyInfo.PropertyType == typeof(bool))
            {
                argumentItem.PropertyInfo.SetValue(newItem, true);
            }
            else if (typeof(IList).IsAssignableFrom(argumentItem.PropertyInfo.PropertyType) && argumentItem.PropertyInfo.PropertyType.IsGenericType)
            {
                var listType = typeof(List<>);
                var listItemType = argumentItem.PropertyInfo.PropertyType.GenericTypeArguments[0];
                var constructedListType = listType.MakeGenericType(listItemType);
                var list = (IList)Activator.CreateInstance(constructedListType);

                if (list == null)
                    throw new NullReferenceException($"The property of type list hasn't been initialized. See {newItem.GetType().Name}.{argumentItem.PropertyInfo.Name}'");

                var increment = isPositional ? 0 : 1;
                for (; i + increment < args.Length; i++)
                {
                    var input = args[i + increment];
                    if (input.StartsWith('-'))
                        break;

                    list.Add(ConvertValue(input, listItemType, argumentItem.GetConverter()));
                }

                argumentItem.PropertyInfo.SetValue(newItem, list);
            }
            else if (argumentItem.PropertyInfo.PropertyType.IsArray)
            {
                var listItemType = argumentItem.PropertyInfo.PropertyType.GetElementType();
                if (listItemType == null)
                    throw new Exception("Since we check the array type, this shouldn't happen.'");

                var listType = typeof(List<>).MakeGenericType(listItemType);
                var list = (IList)Activator.CreateInstance(listType);

                var increment = isPositional ? 0 : 1;
                for (; i + increment < args.Length; i++)
                {
                    var input = args[i + increment];
                    if (input.StartsWith('-'))
                        break;

                    list.Add(ConvertValue(input, listItemType, argumentItem.GetConverter()));
                }

                var array = Array.CreateInstance(listItemType, list.Count);
                list.CopyTo(array, 0);
                argumentItem.PropertyInfo.SetValue(newItem, array);
            }
            else if (argumentItem.PropertyInfo.PropertyType.IsEnum)
            {
                if (Enum.TryParse(argumentItem.PropertyInfo.PropertyType,
                    ReadParameter(args, ref i, argumentItem.GetAction(), isPositional), true, out var result))
                {
                    argumentItem.PropertyInfo.SetValue(newItem, result);
                }
                else
                {
                    throw new CommandArgumentException($"Error parsing the enum for property {argumentItem.PropertyInfo.Name}.");
                }
            }
            else if (argumentItem.PropertyInfo.PropertyType == typeof(FileInfo))
            {
                argumentItem.PropertyInfo.SetValue(newItem,
                    new FileInfo(ReadParameter(args, ref i, argumentItem.GetAction(), isPositional)));
            }
            else if (argumentItem.PropertyInfo.PropertyType == typeof(DirectoryInfo))
            {
                argumentItem.PropertyInfo.SetValue(newItem,
                    new DirectoryInfo(ReadParameter(args, ref i, argumentItem.GetAction(), isPositional)));
            }
            else
            {
                argumentItem.PropertyInfo.SetValue(newItem,
                    ConvertValue(ReadParameter(args, ref i, argumentItem.GetAction(), isPositional),
                        argumentItem.PropertyInfo.PropertyType, argumentItem.GetConverter()));
            }
        }

        private object ConvertValue(string input, Type conversionType, TypeConverter converter)
        {
            if (converter == null)
                converter = TypeDescriptor.GetConverter(conversionType);

            if (converter.CanConvertFrom(input.GetType()))
                return converter.ConvertFrom(input);

            throw new InvalidCastException($"No string converter were found for type {conversionType}");
        }

        private static string ReadParameter(string[] args, ref int i, string parameterName, bool isPositional)
        {
            if (!isPositional)
            {
                if (i + 1 >= args.Length)
                    throw new CommandArgumentException($"Missing a parameter argument for flag {parameterName}");
                i++;

                if (args[i].StartsWith('-'))
                    throw new CommandArgumentException($"Missing a parameter argument for flag {parameterName}");
            }

            return args[i];
        }

        private List<ArgumentItem> GetRequired() => // NOTE: probably do a static list and a new List<ArgumentItem>(requiredArguments)
            (from item in explicitArguments where item.IsRequired() select item).ToList();

        public string ToString(int lineWidth = 80)
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var sb = new ConsoleStringBuilder(lineWidth);

            sb.Append($"Usage: {appName} ");

            if (actions.Count > 0 || actions.Count > 0)
                sb.Append("[OPTION]... ");

            if (positionalArguments.Count > 0)
                sb.Append(PositionalToString());

            sb.AppendLine();
            sb.AppendLine();

            if (explicitArguments.Count > 0)
            {
                sb.AppendLine("Mandatory arguments to long options are mandatory for short options too."); // NOTE: might be optional?
                AppendArguments(sb);
                sb.AppendLine();
                sb.AppendLine();
            }

            var manualAttribute = typeof(T).GetCustomAttribute<CommandManualAttribute>();
            if (manualAttribute != null)
            {
                sb.AppendLine(manualAttribute.Description);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string PositionalToString()
        {
            var result = "";
            foreach (var item in positionalArguments)
            {
                var option = "";
                if (result.Length > 0)
                    option += " ";

                option += $"[{item.GetAction()}]";

                var isArray = item.PropertyInfo.PropertyType.IsArray;
                var isList = typeof(IList).IsAssignableFrom(item.PropertyInfo.PropertyType) &&
                             item.PropertyInfo.PropertyType.IsGenericType;
                if (isArray || isList)
                    option += "...";

                if (!item.IsRequired())
                    option = $"({option})";

                result += option;
            }

            return result;
        }

        private void AppendArguments(ConsoleStringBuilder sb)
        {
            const int optionBaseIndentation = 2;
            const int textIndentation = 32;
            const int aliasLength = 2; //-A
            const string aliasSeparator = ", ";
            // TODO: add [required] keyword
            foreach (var argument in explicitArguments)
            {
                var line = new string(' ', optionBaseIndentation);

                if (argument.GetAlias() != 0)
                    line += $"-{argument.GetAlias()}";

                if (!string.IsNullOrEmpty(argument.GetAction()))
                {
                    line += argument.GetAlias() == 0 ? new string(' ', aliasLength + aliasSeparator.Length) : aliasSeparator;
                    line += argument.GetAction() + " ";
                }

                line += new string(' ', textIndentation - line.Length); // TODO might be too long.

                if (argument.IsRequired())
                    line += "[required] ";
                if (!string.IsNullOrEmpty(argument.GetHelp()))
                    line += argument.GetHelp();

                sb.AppendLine(line, textIndentation);
            }
        }

        private class ArgumentItem
        {
            private readonly TypeConverter converter;
            private ParameterAttribute ArgumentAttribute { get; }

            public ArgumentItem(PropertyInfo propertyInfo, ParameterAttribute argumentAttribute = null)
            {
                PropertyInfo = propertyInfo;
                ArgumentAttribute = argumentAttribute;

                if (ArgumentAttribute?.Converter != null)
                    converter = (TypeConverter)Activator.CreateInstance(ArgumentAttribute.Converter);
            }

            public string GetAction() => ArgumentAttribute == null ? PropertyInfo.Name : ArgumentAttribute.Action;

            public char GetAlias() => ArgumentAttribute?.Alias ?? (char)0;

            public string GetHelp() => ArgumentAttribute?.HelpText;

            public bool IsRequired() => ArgumentAttribute?.Required ?? false;

            public PropertyInfo PropertyInfo { get; }

            public TypeConverter GetConverter() => converter;
        }
    }
}
