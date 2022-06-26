using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // 2^10^ => 2<sup>10</sup>
    //
    internal class Superscript : AccessoryInline
    {
        public Superscript(IEnumerable<IMdInline> content) : base("^", content)
        {
        }
    }
}