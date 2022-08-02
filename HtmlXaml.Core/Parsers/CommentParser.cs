using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    /// <summary>
    /// remove comment element
    /// </summary>
    public class CommentParsre : IBlockTagParser, IInlineTagParser
    {
        public IEnumerable<string> SupportTag => new[] { HtmlNode.HtmlNodeTypeNameComment };

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
