using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    internal class BoldParser : SimpleInlineParser
    {
        public BoldParser() : base("b", "strong") { }

        public override bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
            => Parse(node, manager, nds => new Bold(nds), out generated);
    }
}
