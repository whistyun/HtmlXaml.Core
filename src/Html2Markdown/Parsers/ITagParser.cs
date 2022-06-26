using Html2Markdown.MdElements;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Html2Markdown.Parsers
{
    internal interface ITagParser
    {
        bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated);
    }

    internal interface ISimpleTagParser : ITagParser
    {
        IEnumerable<string> SupportTag { get; }
    }
}
