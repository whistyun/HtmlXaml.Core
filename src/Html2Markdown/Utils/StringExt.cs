using System.Text.RegularExpressions;

namespace Html2Markdown.Utils
{
    internal static class StringExt
    {
        public static string[] SplitLine(this string text) => text.Split('\n');
    }
}
