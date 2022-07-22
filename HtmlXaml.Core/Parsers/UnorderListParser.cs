using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows.Documents;
using HtmlXaml.Core.Utils;
using System.Windows;

namespace HtmlXaml.Core.Parsers
{
    public class UnorderListParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "ul" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Disc;

            foreach (var listItemTag in node.ChildNodes.CollectTag("li"))
            {
                var itemContent = manager.ParseAndGroup(listItemTag.ChildNodes);

                var listItem = new ListItem();
                listItem.Blocks.AddRange(itemContent);

                list.ListItems.Add(listItem);
            }

            generated = new[] { list };
            return true;
        }
    }
}
