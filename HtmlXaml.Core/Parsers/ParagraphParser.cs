using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class ParagraphParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "p", "div" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            generated = manager.ParseAndGroup(node.ChildNodes);
            return true;
        }
    }
}
