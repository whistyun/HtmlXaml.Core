using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TextNodeParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { HtmlNode.HtmlNodeTypeNameText };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            if (node is HtmlTextNode textNode)
            {
                generated = new[] { new Run(textNode.Text) };
                return true;
            }

            generated = Array.Empty<TextElement>();
            return false;
        }
    }
}
