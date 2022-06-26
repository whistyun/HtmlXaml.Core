using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // `text`  => <code>text</code>
    //
    internal class Code : AccessoryInline
    {
        public Code(string tag, string code) : base(tag, new[] { new Plain(code) })
        {
        }
    }
}