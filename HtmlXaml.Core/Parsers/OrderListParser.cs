using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using HtmlXaml.Core.Utils;
using System.Windows;

namespace HtmlXaml.Core.Parsers
{
    public class OrderListParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "ol" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Decimal;

            var startAttr = node.Attributes["start"];
            if (startAttr is not null && Int32.TryParse(startAttr.Value, out var start))
            {
                list.StartIndex = start;
            }

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
