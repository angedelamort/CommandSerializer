using System;
using System.Collections.Generic;
using System.Text;

namespace CommandSerializer.Utils
{
    public class ConsoleStringBuilder
    {
        private readonly List<string> lines = new List<string>();
        private string currentLine = "";

        public int LineWidth { get; }

        public int TabIndentation { get; set; }

        public ConsoleStringBuilder(int lineWidth, int tabIndentation = 4)
        {
            LineWidth = lineWidth;
            TabIndentation = tabIndentation;
        }

        public void AppendLine()
        {
            lines.Add("");
        }

        public void AppendLine(string text, int overflowIndentation = 0)
        {
            Append(text);
            currentLine = "";
        }

        public void Append(string text, int overflowIndentation = 0)
        {
            foreach (var letter in text)
            {
                switch (letter)
                {
                    case '\t':
                        currentLine += new string(' ', (currentLine.Length / TabIndentation) + TabIndentation);
                        break;

                    case '\r':
                        // do nothing
                        break;

                    case '\n':
                        lines.Add(currentLine);
                        currentLine = new string(' ', overflowIndentation);
                        break;

                    default:
                        if (!(string.IsNullOrWhiteSpace(currentLine) && char.IsWhiteSpace(letter))) // NOTE: super slow - find an alternative.
                            currentLine += letter;
                        break;
                }

                if (currentLine.Length > LineWidth)
                {
                    int i;
                    for (i = currentLine.Length - 1; i >= 0; i--)
                    {
                        if (currentLine[i] == ' ' && i <= currentLine.Length)
                        {
                            lines.Add(currentLine.Substring(0, i).TrimEnd());
                            currentLine = new string(' ', overflowIndentation) + currentLine.Substring(i).TrimStart();
                            break;
                        }
                    }

                    // NOTE: Add it anyway - we did our best!
                    if (i == 0)
                    {
                        lines.Add(currentLine.Substring(0, LineWidth).TrimEnd());
                        currentLine = new string(' ', overflowIndentation) + currentLine.Substring(LineWidth);
                    }
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.AppendLine(line);

            if (!string.IsNullOrWhiteSpace(currentLine))
                builder.Append(currentLine);

            return builder.ToString();
        }
    }
}
