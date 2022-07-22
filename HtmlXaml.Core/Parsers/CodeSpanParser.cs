using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using HtmlXaml.Core.Utils;

namespace HtmlXaml.Core.Parsers
{
    public class CodeSpanParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "code" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            // inline code support only Plain
            if (node.ChildNodes.All(e => e is HtmlCommentNode or HtmlTextNode))
            {
                var span = new Run(node.InnerText);
                span.Tag = manager.GetTag(Tags.TagCode);

                generated = new[] { span };
                return true;
            }

            // block code support only Plain and Linebreak
            if (HasOnlyPlain(node))
            {
                generated = new[] { DocUtils.CreateCodeBlock(null, node.InnerText, manager) };
                return true;
            }

            generated = Array.Empty<TextElement>();
            return false;
        }

        private bool HasOnlyPlain(HtmlNode node)
            => node.ChildNodes.All(e => e.NodeType switch
            {
                HtmlNodeType.Comment => true,
                HtmlNodeType.Text => true,
                HtmlNodeType.Element => String.Equals(e.Name, "br", StringComparison.OrdinalIgnoreCase),
                _ => false
            });
    }
}
