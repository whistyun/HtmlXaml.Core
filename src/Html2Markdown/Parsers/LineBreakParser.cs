using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    public class LineBreakParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "br" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = new[] { new Linebreak() };
            return true;
        }
    }
}
