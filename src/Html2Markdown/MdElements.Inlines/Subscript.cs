using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // H~2~O => H<sub>2</sub>O
    //
    public class Subscript : AccessoryInline
    {
        public Subscript(IEnumerable<IMdInline> content) : base("~", content)
        {
        }

    }
}