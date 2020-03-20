using System.Collections.Generic;
using System.Text;

namespace CommandSerializer.Utils
{
    public class ConsoleStringBuilder
    {
        private readonly List<string> lines = new List<string>();
        private string currentLine = "";
        private bool isOverflowOnNewLineContainsChar;  // When we have an overflow, tells if there is an actual character or just the indentation spaces.
        private bool isOverflowMode;

        public int LineWidth { get; }

        public int TabIndentation { get; set; }

        public ConsoleStringBuilder(int lineWidth, int tabIndentation = 4)
        {
            LineWidth = lineWidth;
            TabIndentation = tabIndentation;
        }

        public void AppendLine(string text = null, int overflowIndentation = 0)
        {
            if (!string.IsNullOrEmpty(text))
                Append(text);
            lines.Add(currentLine);
            currentLine = "";
            isOverflowOnNewLineContainsChar = false;
            isOverflowMode = false;
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
                        isOverflowOnNewLineContainsChar = true;
                        isOverflowMode = true;
                        break;

                    default:
                        if (!isOverflowOnNewLineContainsChar || !char.IsWhiteSpace(letter))
                        {
                            currentLine += letter;
                            isOverflowOnNewLineContainsChar = false;
                        }
                        break;
                }

                if (currentLine.Length > LineWidth)
                {
                    var i = currentLine.Length - 1;
                    var max = isOverflowMode ? overflowIndentation : 0;
                    for (; i >= max; i--)
                    {
                        if (char.IsWhiteSpace(currentLine[i]) && i <= currentLine.Length)
                        {
                            lines.Add(currentLine.Substring(0, i).TrimEnd());
                            currentLine = new string(' ', overflowIndentation) + currentLine.Substring(i).TrimStart();
                            isOverflowOnNewLineContainsChar = true;
                            isOverflowMode = true;
                            break;
                        }
                    }

                    // NOTE: Add it anyway - we did our best!
                    if (i == max - 1)
                    {
                        lines.Add(currentLine.Substring(0, LineWidth).TrimEnd());
                        currentLine = new string(' ', overflowIndentation) + currentLine.Substring(LineWidth);
                        isOverflowOnNewLineContainsChar = false;
                        isOverflowMode = true;
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
