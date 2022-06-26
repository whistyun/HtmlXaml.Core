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
    internal class BlockquoteParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "blockquote" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            var blocks = manager.ParseAndGroup(node.ChildNodes);

            generated = new[] { new BlockquoteBlock(blocks) };
            return true;
        }
    }
}
