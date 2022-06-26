using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Blocks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class HeadingParser : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();

            if (node.NodeType != HtmlNodeType.Element)
                return false;

            if (!Regex.IsMatch(node.Name, "^[hH][1-6]$"))
                return false;

            var level = Int32.Parse(node.Name.Substring(1));

            var blocks = manager.ParseAndGroup(node.ChildNodes);

            if (blocks.Count() > 1)
                return false;

            var block = blocks.First();

            if (block is Paragraph paragraph)
            {
                generated = new[] { new HeadingBlock(level, paragraph) };
                return true;
            }

            return false;
        }
    }
}
