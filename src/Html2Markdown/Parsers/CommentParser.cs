using Html2Markdown.MdElements;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Html2Markdown.Parsers
{
    /// <summary>
    /// remove comment element
    /// </summary>
    class CommentParsre : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();
            return node.NodeType == HtmlNodeType.Comment;
        }
    }
}
