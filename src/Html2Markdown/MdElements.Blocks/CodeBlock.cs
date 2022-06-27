using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Html2Markdown.Utils;

namespace Html2Markdown.MdElements.Blocks
{
    public class CodeBlock : IMdBlock
    {
        public string Lang { get; }
        public string Code { get; }

        public CodeBlock(string lang, string code)
        {
            Lang = lang;

            var headMch = Regex.Match(code, "^\r?\n");
            if (headMch.Success)
            {
                code = code.Substring(headMch.Value.Length);
            }

            var tailMch = Regex.Match(code, "\r?\n$");
            if (tailMch.Success)
            {
                code = code.Substring(0, code.Length - tailMch.Value.Length);
            }

            Code = WebUtility.HtmlDecode(code);
        }


        public IEnumerable<string> ToMarkdown()
        {
            yield return "```" + Lang;

            foreach (var codeline in Code.SplitLine())
                yield return codeline;

            yield return "```";
        }
    }
}
