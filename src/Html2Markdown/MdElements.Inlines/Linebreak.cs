using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html2Markdown.MdElements.Inlines
{
    internal class Linebreak : IMdInline
    {
        public void TrimStart() { }

        public void TrimEnd() { }

        public string ToMarkdown() => "  \n";
    }
}
