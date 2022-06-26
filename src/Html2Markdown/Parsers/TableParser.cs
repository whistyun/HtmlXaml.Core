using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Blocks;
using Html2Markdown.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class PipeTableParser : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();

            if (node.NodeType != HtmlNodeType.Element)
                return false;

            if (node.Name.ToLower() != "table")
                return false;

            var theadRows = node.SelectNodes("./thead/tr");
            if (theadRows is null)
                return false;

            var tbodyRows = node.SelectNodes("./tbody/tr");
            if (tbodyRows is null)
                return false;


            var headGrp = TableRows2Block(theadRows, manager);
            if (headGrp is null)
                return false;

            var bodyGrp = TableRows2Block(tbodyRows, manager);
            if (bodyGrp is null)
                return false;

            List<IMdBlock>[]? footGrp = null;
            var tfootRows = node.SelectNodes("./tfoot/tr");
            if (tfootRows is not null)
            {
                footGrp = TableRows2Block(tfootRows, manager);
                if (footGrp is null)
                    return false;
            }

            var headStyle = ParseColumnStyle(theadRows.First());
            var details = headGrp.Skip(1).Concat(bodyGrp);
            if (footGrp is not null)
            {
                details = details.Concat(footGrp);
            }

            generated = new[] {
                new PipeTableBlock(headStyle ,headGrp.First(), details)
            };
            return false;
        }

        private List<IMdBlock>[]? TableRows2Block(IEnumerable<HtmlNode> rows, ReplaceManager manager)
        {
            var list = new List<List<IMdBlock>>();

            foreach (var row in rows)
            {
                List<IMdBlock> cells = new();

                foreach (var cell in row.ChildNodes.CollectTag())
                {
                    if (!IsNullOr1(cell.Attributes["colspan"]?.Value)) return null;
                    if (!IsNullOr1(cell.Attributes["rowspan"]?.Value)) return null;

                    var parsed = manager.ParseAndGroup(cell.ChildNodes);
                    if (parsed.Count() > 1) return null;

                    cells.Add(parsed.First());
                }

                list.Add(cells);
            }

            return list.ToArray();
        }

        private List<string?> ParseColumnStyle(HtmlNode row)
        {
            var styles = new List<string?>();

            foreach (var cell in row.ChildNodes.CollectTag())
            {
                var style = cell.Attributes["style"]?.Value;
                if (style is null)
                {
                    styles.Add(null);
                    continue;
                }

                var match = Regex.Match(style, @"text-align[ \t]*:[ \t]*([a-z]+)[ \t]*;?");
                styles.Add(match.Success ? match.Groups[1].Value : null);
            }

            return styles;
        }

        private bool IsNullOr1(string? text)
        {
            if (String.IsNullOrEmpty(text))
                return true;

            if (Int32.TryParse(text, out var num))
                return num == 1;

            return false;
        }
    }
}
