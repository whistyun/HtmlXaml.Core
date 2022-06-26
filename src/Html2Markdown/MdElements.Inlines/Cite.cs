using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // ""text""  => <cite>text</cite>
    //
    internal class Cite : AccessoryInline
    {
        public Cite(IEnumerable<IMdInline> content) : base("\"\"", content)
        {
        }
    }
}