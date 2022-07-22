using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class CodeBlockParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "pre" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            generated = Array.Empty<TextElement>();

            var codeElements = node.ChildNodes.CollectTag("code");
            if (codeElements.Count != 0)
            {
                var rslt = new List<TextElement>();

                foreach (var codeElement in codeElements)
                {
                    // "language-**", "lang-**", "**" or "sourceCode **"
                    var classVal = codeElement.Attributes["class"]?.Value;

                    var langCode = ParseLangCode(classVal);
                    rslt.Add(DocUtils.CreateCodeBlock(langCode, codeElement.InnerText, manager));
                }

                generated = rslt;
                return rslt.Count > 0;
            }
            else if (node.ChildNodes.TryCastTextNode(out var textNodes))
            {
                var buff = new StringBuilder();
                foreach (var textNode in textNodes)
                    buff.Append(textNode.InnerText);

                generated = new[] { DocUtils.CreateCodeBlock(null, buff.ToString(), manager) };
                return true;
            }
            else return false;
        }

        private string ParseLangCode(string? classVal)
        {
            if (classVal is null) return "";

            // "language-**", "lang-**", "**" or "sourceCode **"
            var indics = Enumerable.Range(0, classVal.Length)
                                   .Reverse()
                                   .Where(i => !Char.IsLetterOrDigit(classVal[i]));

            return classVal.Substring(indics.Count() == 0 ? 0 : indics.First() + 1);
        }
    }
}
