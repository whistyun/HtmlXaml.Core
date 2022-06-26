using Html2Markdown.MdElements;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class TagIgnoreParser : ITagParser
    {
        private HashSet<string> _targets;

        public TagIgnoreParser()
        {
            _targets = new()
            {
                "title",
                "meta",
                "link",
                "script",
                "style"
            };
        }


        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();

            if (node.NodeType != HtmlNodeType.Element)
                return false;

            return _targets.Contains(node.Name.ToLower());
        }
    }
}
