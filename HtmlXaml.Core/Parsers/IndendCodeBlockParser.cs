using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class IndendCodeBlockParser : IBlockTagParser, ISimpleTag
    {
        public IEnumerable<string> SupportTag => new[] { "code", "kbd", "var" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Block> generated)
        {
            // block code support only Plain and Linebreak
            if (HasOnlyPlain(node))
            {
                generated = new[] { DocUtils.CreateCodeBlock(null, node.InnerText, manager) };
                return true;
            }

            generated = Array.Empty<Block>();
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
