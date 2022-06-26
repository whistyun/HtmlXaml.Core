using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html2Markdown.MdElements.Inlines
{
    internal class Plain : IMdInline
    {
        public string Content { private set; get; }

        public Plain(string text)
        {
            bool alreadySpaced = false;

            var buff = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; ++i)
            {
                var c = text[i];

                if (Char.IsWhiteSpace(c))
                {
                    if (alreadySpaced) continue;

                    buff.Append(' ');

                    alreadySpaced = true;
                    continue;
                }

                alreadySpaced = false;

                if (c == '&' && TryDecode(text, ref i, out var decoded))
                {
                    buff.Append(decoded);
                    continue;
                }

                buff.Append(c);
            }

            Content = buff.ToString();
        }

        public void TrimStart() => Content = Content.TrimStart();

        public void TrimEnd() => Content = Content.TrimEnd();

        public string ToMarkdown() => Content;

        private bool TryDecode(string text, ref int start, out string decoded)
        {
            //  max length of entity is 33 (&CounterClockwiseContourIntegral;)
            var hit = text.IndexOf(';', start, Math.Min(text.Length - start, 40));

            if (hit == -1)
            {
                decoded = string.Empty;
                return false;
            }

            var entity = text.Substring(start, hit - start + 1);
            decoded = WebUtility.HtmlDecode(entity);
            start = hit;

            if (decoded == "<" && start + 1 < text.Length)
            {
                var c = text[start + 1];
                if ('a' <= c && c <= 'z' && 'A' <= c && c <= 'Z')
                {
                    // '<[a-zA-Z]' may be treated as tag
                    decoded = entity;
                }
            }


            return true;
        }

    }
}
