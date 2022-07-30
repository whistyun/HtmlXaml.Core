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
    public class TypicalBlockParser : IBlockTagParser
    {
        private const string _resource = "HtmlXaml.Core.Parsers.TypicalBlockParser.tsv";
        private Dictionary<string, TypicalParseInfo> _infos;

        public TypicalBlockParser()
        {
            _infos = TypicalParseInfo.Load(_resource);
        }

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Block> generated)
        {
            if (_infos.TryGetValue(node.Name.ToLower(), out TypicalParseInfo? info)
                && info.TryReplace(node, manager, out var parsed))
            {
                generated = parsed.Cast<Block>();
                return true;
            }
            else
            {
                generated = Array.Empty<Block>();
                return false;
            }
        }
    }
}
