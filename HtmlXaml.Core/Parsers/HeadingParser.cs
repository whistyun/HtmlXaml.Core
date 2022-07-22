using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class HeadingParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "h1", "h2", "h3", "h4", "h5", "h6" };

        private Tags[] _TagList = { Tags.TagHeading1, Tags.TagHeading2, Tags.TagHeading3, Tags.TagHeading4, Tags.TagHeading5, Tags.TagHeading6 };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var level = int.Parse(node.Name.Substring(1));

            var blocks = manager.ParseAndGroup(node.ChildNodes);

            var tag = manager.GetTag(_TagList[level - 1]);

            foreach (var block in blocks)
                block.Tag = tag;

            generated = blocks;
            return true;
        }
    }
}
