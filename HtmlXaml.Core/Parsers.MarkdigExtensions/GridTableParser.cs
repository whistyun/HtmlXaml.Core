using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows;
using HtmlXaml.Core.Utils;

namespace HtmlXaml.Core.Parsers.MarkdigExtensions
{
    public class GridTableParser : ISimpleTagParser, IHasPriority
    {
        public int Priority => TagParserExt.DefaultPriority + 1000;

        public IEnumerable<string> SupportTag => new[] { "table" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var table = new Table();

            ParseColumnStyle(node, table);

            int totalColCount = 0;

            var theadRows = node.SelectNodes("./thead/tr");
            if (theadRows is not null)
            {
                var group = CreateRowGroup(theadRows, manager, out int colCount);
                group.Tag = manager.GetTag(Tags.TagTableHeader);
                table.RowGroups.Add(group);

                totalColCount = Math.Max(totalColCount, colCount);
            }

            var tbodyRows = new List<HtmlNode>();
            foreach (var child in node.ChildNodes)
            {
                if (string.Equals(child.Name, "tr", StringComparison.OrdinalIgnoreCase))
                    tbodyRows.Add(child);

                if (string.Equals(child.Name, "tbody", StringComparison.OrdinalIgnoreCase))
                    tbodyRows.AddRange(child.ChildNodes.CollectTag("tr"));
            }
            if (tbodyRows.Count > 0)
            {
                var group = CreateRowGroup(tbodyRows, manager, out int colCount);
                group.Tag = manager.GetTag(Tags.TagTableBody);
                table.RowGroups.Add(group);

                int idx = 0;
                foreach (var row in group.Rows)
                {
                    var useTag = (++idx & 1) == 0 ? Tags.TagEvenTableRow : Tags.TagOddTableRow;
                    row.Tag = manager.GetTag(useTag);
                }

                totalColCount = Math.Max(totalColCount, colCount);
            }

            var tfootRows = node.SelectNodes("./tfoot/tr");
            if (tfootRows is not null)
            {
                var group = CreateRowGroup(tfootRows, manager, out int colCount);
                group.Tag = manager.GetTag(Tags.TagTableFooter);
                table.RowGroups.Add(group);

                totalColCount = Math.Max(totalColCount, colCount);
            }

            while (totalColCount >= table.Columns.Count)
            {
                table.Columns.Add(new TableColumn());
            }

            var captions = node.SelectNodes("./caption");
            if (captions is not null)
            {
                var tableSec = new Section();
                foreach (var cap in captions)
                {
                    tableSec.Blocks.AddRange(manager.ParseAndGroup(captions[0].ChildNodes));
                }

                tableSec.Blocks.Add(table);
                tableSec.Tag = manager.GetTag(Tags.TagTableCaption);

                generated = new[] { tableSec };
            }
            else
            {
                generated = new[] { table };
            }

            return true;
        }

        private void ParseColumnStyle(HtmlNode tableTag, Table table)
        {
            var colHolder = tableTag.ChildNodes.HasOneTag("colgroup", out var colgroup) ? colgroup : tableTag;

            foreach (var col in colHolder.ChildNodes.CollectTag("col"))
            {
                var coldef = new TableColumn();
                table.Columns.Add(coldef);

                var spanAttr = col.Attributes["span"];
                if (spanAttr is not null)
                {
                    if (int.TryParse(spanAttr.Value, out var spanCnt))
                    {
                        foreach (var _ in Enumerable.Range(0, spanCnt - 1))
                            table.Columns.Add(coldef);
                    }
                }

                var styleAttr = col.Attributes["style"];
                if (styleAttr is null) continue;

                var mch = Regex.Match(styleAttr.Value, "width:([^;\"]+?)(%|em|ex|mm|cm|in|pt|pc)");
                if (!mch.Success) continue;

                var numTxt = mch.Groups[1].Value.Trim();
                if (!Double.TryParse(numTxt, out var numVal)) continue;

                coldef.Width = mch.Groups[2].Value.Trim() switch
                {
                    "%" => new GridLength(numVal, GridUnitType.Star),
                    "em" => new GridLength(numVal * 11),
                    "ex" => new GridLength(numVal * 11 / 2),
                    "mm" => new GridLength(numVal * 3.77952755905512),
                    "cm" => new GridLength(numVal * 37.7952755905512),
                    "in" => new GridLength(numVal * 96.0),
                    "pt" => new GridLength(numVal * 1.33333333333333),
                    "pc" => new GridLength(numVal * 16),
                    _ => new GridLength(numVal),
                };
            }
        }


        private TableRowGroup CreateRowGroup(
            IEnumerable<HtmlNode> rows,
            ReplaceManager manager,
            out int maxColCount)
        {
            var group = new TableRowGroup();
            var list = new List<ColspanCounter>();

            maxColCount = 0;

            foreach (var rowTag in rows)
            {
                var row = new TableRow();

                int colCount = list.Sum(e => e.ColSpan);

                foreach (var cellTag in rowTag.ChildNodes.CollectTag("td", "th"))
                {
                    var cell = new TableCell();
                    cell.Blocks.AddRange(manager.ParseAndGroup(cellTag.ChildNodes));

                    int colspan = TryParse(cellTag.Attributes["colspan"]?.Value);
                    int rowspan = TryParse(cellTag.Attributes["rowspan"]?.Value);

                    cell.RowSpan = rowspan;
                    cell.ColumnSpan = colspan;

                    row.Cells.Add(cell);

                    colCount += colspan;

                    if (rowspan > 1)
                    {
                        list.Add(new ColspanCounter(rowspan, colspan));
                    }
                }

                group.Rows.Add(row);

                maxColCount = Math.Max(maxColCount, colCount);

                for (int idx = list.Count - 1; idx >= 0; --idx)
                    if (list[idx].Detent())
                        list.RemoveAt(idx);
            }

            return group;

            static int TryParse(string? txt) => int.TryParse(txt, out var v) ? v : 1;
        }


        class ColspanCounter
        {
            public int Remain { get; set; }
            public int ColSpan { get; }

            public ColspanCounter(int rowspan, int colspan)
            {
                Remain = rowspan;
                ColSpan = colspan;
            }

            public bool Detent()
            {
                return --Remain == 0;
            }
        }
    }
}
