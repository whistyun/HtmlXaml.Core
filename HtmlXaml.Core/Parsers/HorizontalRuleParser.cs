using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class HorizontalRuleParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "hr" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var sep = new Separator();
            var container = new BlockUIContainer(sep);
            container.Tag = manager.GetTag(Tags.TagRuleSingle);

            generated = new[] { container };
            return true;
        }
    }
}
