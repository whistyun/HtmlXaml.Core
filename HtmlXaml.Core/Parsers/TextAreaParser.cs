using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TextAreaParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "textarea" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var area = new TextBox();
            area.AcceptsReturn = true;
            area.AcceptsTab = true;
            area.Text = node.InnerText.Trim();
            area.TextWrapping = TextWrapping.Wrap;

            int? rows = null;
            int? cols = null;
            var rowsAttr = node.Attributes["rows"];
            var colsAttr = node.Attributes["cols"];

            if (rowsAttr is not null)
            {
                if (int.TryParse(rowsAttr.Value, out var v))
                    rows = v * 14;
            }
            if (colsAttr is not null)
            {
                if (int.TryParse(colsAttr.Value, out var v))
                    cols = v * 7;
            }

            if (rows.HasValue) area.Height = rows.Value;
            if (cols.HasValue) area.Width = cols.Value;

            generated = new[] { new InlineUIContainer(area) };
            return true;
        }
    }
}
