using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TagIgnoreParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "title", "meta", "link", "script", "style", "datalist" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            generated = Array.Empty<TextElement>();
            return true;
        }
    }
}
