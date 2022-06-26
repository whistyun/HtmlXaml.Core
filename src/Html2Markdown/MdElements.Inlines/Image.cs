using System;
using System.Collections.Generic;
using System.Text;

namespace Html2Markdown.MdElements.Inlines
{
    //
    // ![text](link) => <img alt="text" src="link">
    //
    internal class Image : IMdInline
    {
        public string? Alt { get; }
        public string Src { get; }
        public string? Title { get; }

        public Image(string? alt, string src, string? title)
        {
            Alt = alt;
            Src = src;
            Title = title;
        }

        public void TrimStart() => Alt?.TrimStart();

        public void TrimEnd() => Alt?.TrimEnd();

        public string ToMarkdown() => $"![{(Alt is null ? "" : Alt)}]({Src}{(Title is null ? "" : @$" ""{Title}""")})";
    }
}