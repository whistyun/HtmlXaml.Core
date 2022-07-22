using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public interface ITagParser
    {
        bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated);
    }

    public interface ISimpleTagParser : ITagParser
    {
        IEnumerable<string> SupportTag { get; }
    }

    public interface IHasPriority
    {
        int Priority { get; }
    }

    public static class TagParserExt
    {
        public const int DefaultPriority = 10000;

        public static int GetPriority(this ITagParser parser)
        {
            return parser is IHasPriority prop ? prop.Priority : DefaultPriority;
        }
    }
}
