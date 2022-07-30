using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TextNodeParser : IInlineTagParser, ISimpleTag
    {
        public IEnumerable<string> SupportTag => new[] { HtmlNode.HtmlNodeTypeNameText };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            if (node is HtmlTextNode textNode)
            {
                generated = new[] { new Run(textNode.Text) };
                return true;
            }

            generated = Array.Empty<Inline>();
            return false;
        }
    }
}
