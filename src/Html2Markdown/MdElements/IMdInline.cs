using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements
{
    public interface IMdInline : IMdElement
    {
        void TrimStart();
        void TrimEnd();

        string ToMarkdown();
    }
}
