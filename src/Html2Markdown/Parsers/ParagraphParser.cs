using Html2Markdown.MdElements;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    class ParagraphParser : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            if (node.NodeType != HtmlNodeType.Element)
            {
                generated = Array.Empty<IMdElement>();
                return false;
            }

            var tagNm = node.Name.ToLower();
            if (tagNm != "div" && tagNm != "p")
            {
                generated = Array.Empty<IMdElement>();
                return false;
            }

            generated = manager.ParseAndGroup(node.ChildNodes);
            return true;
        }
    }
}
