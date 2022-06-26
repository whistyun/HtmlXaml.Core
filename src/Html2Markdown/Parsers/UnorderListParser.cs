using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Blocks;
using Html2Markdown.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class UnorderListParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "ul" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            var block = new UnorderListBlock();

            foreach (var listItem in node.ChildNodes.CollectTag("li"))
            {
                var items = manager.ParseAndGroup(listItem.ChildNodes);
                block.ListItems.Add(items);
            }

            generated = block.ListItems.Count > 0 ?
                            new[] { block } :
                            Array.Empty<IMdElement>();

            return true;
        }
    }
}
