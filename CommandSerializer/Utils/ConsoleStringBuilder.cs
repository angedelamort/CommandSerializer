using System.Text;

namespace CommandSerializer.Utils
{
    public class ConsoleStringBuilder
    {
        private readonly StringBuilder builder = new StringBuilder();

        public int LineWidth { get; }

        public ConsoleStringBuilder(int lineWidth)
        {
            LineWidth = lineWidth;
        }

        public void AppendLine()
        {
            builder.AppendLine();
        }

        public void AppendLine(string text, int overflowIndentation = 0)
        {
            Append(text, overflowIndentation);
            builder.AppendLine();
        }

        public void Append(string text, int overflowIndentation = 0)
        {
            // TODO -> split and indent properly.
            builder.Append(text);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
