﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HtmlXaml.Core.Parsers
{
    public class ProgressParser : IInlineTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "progress", "meter" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            var bar = new ProgressBar();
            bar.Value = TryParse(node.Attributes["value"]?.Value, 1);
            bar.Minimum = TryParse(node.Attributes["min"]?.Value, 0);
            bar.Maximum = TryParse(node.Attributes["max"]?.Value, 1);

            bar.Width = 50;
            bar.Height = 12;

            generated = new[] { new InlineUIContainer(bar) };
            return true;
        }

        private int TryParse(string? txt, int def)
        {
            if (txt is null) return def;
            return int.TryParse(txt, out var v) ? v : def;
        }
    }
}