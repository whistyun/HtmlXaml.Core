using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class CodeSpanParser : IInlineTagParser, ISimpleTag
    {
        public IEnumerable<string> SupportTag => new[] { "code", "kbd", "var" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            // inline code support only Plain
            if (node.ChildNodes.All(e => e is HtmlCommentNode or HtmlTextNode))
            {
                var span = new Run(node.InnerText);
                span.Tag = manager.GetTag(Tags.TagCode);

                generated = new[] { span };
                return true;
            }

            generated = Array.Empty<Inline>();
            return false;
        }
    }
}
