using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements
{
    internal interface IMdBlock : IMdElement
    {
        IEnumerable<string> ToMarkdown();
    }
}
