using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements
{
    internal interface IMdInline : IMdElement
    {
        void TrimStart();
        void TrimEnd();

        string ToMarkdown();
    }
}
