using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    internal class ItalicParser : SimpleInlineParser
    {
        public ItalicParser() : base("i", "em") { }

        public override bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
            => Parse(node, manager, nds => new Italic(nds), out generated);
    }
}
