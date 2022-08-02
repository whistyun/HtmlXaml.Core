using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TagIgnoreParser : IBlockTagParser, IInlineTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "title", "meta", "link", "script", "style", "datalist" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            generated = Array.Empty<TextElement>();
            return true;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Block> generated)
        {
            generated = Array.Empty<Block>();
            return true;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            generated = Array.Empty<Inline>();
            return true;
        }
    }
}
