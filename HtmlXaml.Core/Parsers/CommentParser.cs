using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    /// <summary>
    /// remove comment element
    /// </summary>
    public class CommentParsre : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { HtmlNode.HtmlNodeTypeNameComment };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            generated = Array.Empty<TextElement>();
            return true;
        }
    }
}
