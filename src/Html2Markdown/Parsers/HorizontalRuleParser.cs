using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Blocks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.Parsers
{
    internal class HorizontalRuleParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "hr" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = new[] { new HorizontalRuleBlock() };
            return true;
        }
    }
}
