using Html2Markdown.MdElements;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    public class ParagraphParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "p", "div" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = manager.ParseAndGroup(node.ChildNodes);
            return true;
        }
    }
}
