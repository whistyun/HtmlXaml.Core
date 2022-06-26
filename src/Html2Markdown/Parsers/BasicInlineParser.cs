using Html2Markdown.MdElements;
using Html2Markdown.MdElements.Inlines;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Html2Markdown.Utils;
using Html2Markdown.MdElements.Blocks;

namespace Html2Markdown.Parsers
{
    internal class BasicInlineParser : ITagParser
    {
        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<IMdElement> generated)
        {
            generated = Array.Empty<IMdElement>();

            switch (node.Name.ToLower())
            {

                case "b":
                case "strong":
                    return Parse(node, manager, nds => new Bold(nds), ref generated);


                case "i":
                case "em":
                    return Parse(node, manager, nds => new Italic(nds), ref generated);

                case "a":
                    {
                        var link = node.Attributes["href"]?.Value;
                        var content = manager.ParseJagging(node.ChildNodes);
                        var title = node.Attributes["title"]?.Value;
                        if (link is null) return false;
                        if (content.All(nd => nd is IMdInline))
                        {
                            generated = new[] { new Hyperlink(content.Cast<IMdInline>(), link, title) };
                            return true;
                        }

                        return false;
                    }

                case "img":
                case "image":
                    {
                        var link = node.Attributes["src"]?.Value;
                        var alt = node.Attributes["alt"]?.Value;
                        if (link is null) return false;
                        var title = node.Attributes["title"]?.Value;

                        generated = new[] { new Image(alt, link, title) };
                        return true;
                    }

                case "code":
                    {
                        // inline code support only Plain
                        if (node.ChildNodes.All(e => e is HtmlCommentNode or HtmlTextNode))
                        {
                            var span = node.InnerText;

                            int tagCnt = 1;
                            var mch = Regex.Match(span, "`+");
                            if (mch.Success)
                            {
                                tagCnt = 1 + mch.Value.Length;
                            }

                            generated = new[] { new Code(new String('`', tagCnt), span) };
                            return true;
                        }

                        // block code support only Plain and Linebreak
                        if (HasOnlyPlain(node))
                        {
                            generated = new[] { new IndendBlock(node.InnerText) };
                            return true;
                        }

                        return false;
                    }
            }

            return false;
        }

        private bool HasOnlyPlain(HtmlNode node)
            => node.ChildNodes.All(e => e.NodeType switch
            {
                HtmlNodeType.Comment => true,
                HtmlNodeType.Text => true,
                HtmlNodeType.Element => String.Equals(e.Name, "br", StringComparison.OrdinalIgnoreCase),
                _ => false
            });

        private bool Parse(HtmlNode node, ReplaceManager manager, Func<IEnumerable<IMdInline>, IMdInline> converter, ref IEnumerable<IMdElement> generated)
        {
            var children = manager.ParseJagging(node.ChildNodes);

            if (children.TryCast<IMdInline>(out var inlines))
            {
                generated = new[] { converter(inlines) };
                return true;
            }
            return false;
        }


    }
}
