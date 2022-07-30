using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace HtmlXaml.Core.Parsers
{
    public class TypicalInlineParser : IInlineTagParser
    {
        private const string _resource = "HtmlXaml.Core.Parsers.TypicalInlineParser.tsv";
        private Dictionary<string, TypicalParseInfo> _infos;

        public TypicalInlineParser()
        {
            _infos = TypicalParseInfo.Load(_resource);
        }

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            if (_infos.TryGetValue(node.Name.ToLower(), out TypicalParseInfo? info)
                && info.TryReplace(node, manager, out var parsed))
            {
                generated = parsed.Cast<Inline>();
                return true;
            }
            else
            {
                generated = Array.Empty<Inline>();
                return false;
            }
        }
    }
}
