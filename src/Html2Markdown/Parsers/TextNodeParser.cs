using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    internal class TextNodeParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { HtmlNode.HtmlNodeTypeNameText };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            if (node is HtmlTextNode textNode)
            {
                generated = new[] { new Plain(textNode.Text) };
                return true;
            }

            generated = Array.Empty<IMdElement>();
            return false;
        }
    }
}
