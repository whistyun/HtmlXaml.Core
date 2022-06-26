using Markdig;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DocAsCode.MarkdigEngine.Extensions;
using System.Reflection;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Html2Markdown.Test
{
    public class ReReConvertTests
    {
        private string _testPath;
        private MarkdownPipeline _pipe;

        public ReReConvertTests()
        {
            var builder = new MarkdownPipelineBuilder();
            builder.UseDocfxExtensions(new MarkdownContext());

            _pipe = builder.Build();

            _testPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        }


        [Test]
        public void ListTest()
        {
            var mdtxt = ReadText("List.md");

            var htmltxt = Markdown.ToHtml(mdtxt);

            var converter = new Converter();
            var reMdtxt = converter.Convert(htmltxt);

            WriteText("List.temp.md", reMdtxt);

            var reHtmltxt = Markdown.ToHtml(reMdtxt);

            Assert.AreEqual(Normalize(htmltxt), Normalize(reHtmltxt));
        }

        [Test]
        public void CodeTest()
        {
            var mdtxt = ReadText("Code.md");

            var htmltxt = Markdown.ToHtml(mdtxt);

            var converter = new Converter();
            var reMdtxt = converter.Convert(htmltxt);

            WriteText("Code.temp.md", reMdtxt);

            var reHtmltxt = Markdown.ToHtml(reMdtxt);

            Assert.AreEqual(Normalize(htmltxt), Normalize(reHtmltxt));
        }

        private string ReadText(string fileName)
        {
            var fullpath = Path.Combine(_testPath, @"..\..\..\ReReConvertTests", fileName);
            return File.ReadAllText(fullpath);
        }

        private void WriteText(string fileName, string content)
        {
            var fullpath = Path.Combine(_testPath, @"..\..\..\ReReConvertTests", fileName);
            File.WriteAllText(fullpath, content);
        }


        private string Normalize(string text)
        {
            var buff = new StringBuilder();

            var spcPtn = new Regex(@"[ \r\n\t]+", RegexOptions.Compiled);
            var prePtn = new Regex(@"<pre([ \t][^>]+)?>.*?</pre>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.RightToLeft);

            int idx = 0;
            for (; ; )
            {
                var match = prePtn.Match(text, idx);
                if (!match.Success)
                {
                    var preTxt = text.Substring(idx);
                    buff.Append(spcPtn.Replace(preTxt, " "));

                    break;
                }
                else
                {
                    var preTxt = text.Substring(idx, match.Index - idx);
                    buff.Append(spcPtn.Replace(preTxt, " "));

                    buff.Append(match.Value);

                    idx = match.Index + match.Length;
                }
            }

            return buff.ToString();
        }


    }
}
