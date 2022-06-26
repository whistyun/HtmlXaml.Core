using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // ==text== => <mark>text</mark>
    //
    internal class Marked : AccessoryInline
    {
        public Marked(IEnumerable<IMdInline> content) : base("==", content)
        {
        }
    }
}