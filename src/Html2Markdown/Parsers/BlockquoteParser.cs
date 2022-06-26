using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Blocks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class BlockquoteParser : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();

            if (node.NodeType != HtmlNodeType.Element)
                return false;

            if (node.Name.ToLower() != "blockquote")
                return false;

            var blocks = manager.ParseAndGroup(node.ChildNodes);

            generated = new[] { new BlockquoteBlock(blocks) };
            return true;
        }
    }
}
