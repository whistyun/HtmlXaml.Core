using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace Html2Markdown.Parsers.MarkdigExtensions
{
    public class DeletedParser : SimpleInlineParser
    {
        public DeletedParser() : base("del") { }

        public override bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
            => Parse(node, manager, nds => new Deleted(nds), out generated);
    }
}
