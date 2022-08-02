using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Paragraph = System.Windows.Documents.Paragraph;
using System.Windows.Input;
using HtmlXaml.Core.Parsers;
using HtmlXaml.Core.Utils;
using HtmlXaml.Core.Parsers.MarkdigExtensions;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Linq;

namespace HtmlXaml.Core
{
    public class ReplaceManager : IUriContext
    {
        private Dictionary<string, List<IInlineTagParser>> _inlineBindParsers;
        private Dictionary<string, List<IBlockTagParser>> _blockBindParsers;
        private Dictionary<string, List<ITagParser>> _bindParsers;

        public ReplaceManager()
        {
            _inlineBindParsers = new();
            _blockBindParsers = new();
            _bindParsers = new();

            UnknownTags = UnknownTagsOption.Drop;

            Register(new TagIgnoreParser());
            Register(new CommentParsre());
            Register(new ImageParser());
            Register(new CodeBlockParser());
            Register(new CodeSpanParser());
            Register(new OrderListParser());
            Register(new UnorderListParser());
            Register(new TextNodeParser());
            Register(new HorizontalRuleParser());
            Register(new FigureParser());
            Register(new GridTableParser());
            Register(new InputParser());
            Register(new ButtonParser());
            Register(new TextAreaParser());
            Register(new ProgressParser());

            foreach (var parser in TypicalBlockParser.Load())
                Register(parser);

            foreach (var parser in TypicalInlineParser.Load())
                Register(parser);
        }

        public IEnumerable<string> InlineTags => _inlineBindParsers.Keys;
        public IEnumerable<string> BlockTags => _blockBindParsers.Keys;

        public bool MaybeSupportBodyTag(string tagName)
            => _blockBindParsers.ContainsKey(tagName.ToLower());

        public bool MaybeSupportInlineTag(string tagName)
            => _inlineBindParsers.ContainsKey(tagName.ToLower());

        public UnknownTagsOption UnknownTags { get; set; }
        public ICommand HyperlinkCommand { get; set; }
        public Uri BaseUri { get; set; }
        public string AssetPathRoot { get; set; }

        public void Register(ITagParser parser)
        {

            if (parser is IInlineTagParser inlineParser)
            {
                PrivateRegister(inlineParser, _inlineBindParsers);
            }
            if (parser is IBlockTagParser blockParser)
            {
                PrivateRegister(blockParser, _blockBindParsers);
            }

            PrivateRegister(parser, _bindParsers);

        }

        private void PrivateRegister<T>(T parser, Dictionary<string, List<T>> bindParsers) where T : ITagParser
        {
            foreach (var tag in parser.SupportTag)
            {
                if (!bindParsers.TryGetValue(tag.ToLower(), out var list))
                {
                    list = new();
                    bindParsers.Add(tag.ToLower(), list);
                }

                InsertWithPriority(list, parser);
            }
        }

        private void InsertWithPriority<T>(List<T> list, T parser)
        {
            static int GetPriority(object? p)
                => p is IHasPriority prop ? prop.Priority : HasPriority.DefaultPriority;

            int parserPriority = GetPriority(parser);

            int count = list.Count;
            for (int i = 0; i < count; ++i)
            {
                var elmnt = list[i];

                if (parserPriority <= GetPriority(elmnt))
                {
                    list.Insert(i, parser);
                    return;
                }
            }
            list.Add(parser);
        }

        public string GetTag(Tags tag)
        {
            return tag.ToString().Substring(3);
        }

        public BitmapImage? LoadImage(string url)
        {
            // check embedded resoruce
            try
            {
                Uri packUri;
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && BaseUri != null)
                {
                    packUri = new Uri(BaseUri, url);
                }
                else
                {
                    packUri = new Uri(url);
                }

                var img = MakeImage(packUri);
                if (img is not null) return img;
            }
            catch { }

            // check filesystem
            try
            {
                Uri imgUri;

                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && !System.IO.Path.IsPathRooted(url) && AssetPathRoot != null)
                {
                    if (Uri.IsWellFormedUriString(AssetPathRoot, UriKind.Absolute))
                    {
                        imgUri = new Uri(new Uri(AssetPathRoot), url);
                    }
                    else
                    {
                        url = System.IO.Path.Combine(AssetPathRoot ?? string.Empty, url);
                        imgUri = new Uri(url, UriKind.RelativeOrAbsolute);
                    }
                }
                else imgUri = new Uri(url, UriKind.RelativeOrAbsolute);

                var img = MakeImage(imgUri);
                if (img is not null) return img;
            }
            catch { }

            return null;

            static BitmapImage MakeImage(Uri url)
            {
                var imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.CacheOption = BitmapCacheOption.OnLoad;
                imgSource.CreateOptions = BitmapCreateOptions.None;
                imgSource.UriSource = url;
                imgSource.EndInit();

                return imgSource;
            }
        }


        /// <summary>
        /// Convert a html tag list to an element of markdown.
        /// </summary>
        public IEnumerable<Block> Parse(string htmldoc)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmldoc);

            return Parse(doc);
        }

        /// <summary>
        /// Convert a html tag list to an element of markdown.
        /// </summary>
        public IEnumerable<Block> Parse(HtmlDocument doc)
        {
            var contents = new List<HtmlNode>();

            var head = PickBodyOrHead(doc.DocumentNode, "head");
            if (head is not null)
                contents.AddRange(head.ChildNodes.SkipComment());

            var body = PickBodyOrHead(doc.DocumentNode, "body");
            if (body is not null)
                contents.AddRange(body.ChildNodes.SkipComment());

            if (contents.Count == 0)
            {
                var root = doc.DocumentNode.ChildNodes.SkipComment();

                if (root.Count == 1 && string.Equals(root[0].Name, "html", StringComparison.OrdinalIgnoreCase))
                    contents.AddRange(root[0].ChildNodes.SkipComment());
                else
                    contents.AddRange(root);
            }

            var jaggingResult = ParseJagging(contents);

            return Grouping(jaggingResult);
        }

        /// <summary>
        /// Convert a html tag list to an element of markdown.
        /// Inline elements are aggreated into paragraph.
        /// </summary>
        public IEnumerable<Block> ParseAndGroup(HtmlNodeCollection nodes)
        {
            var jaggingResult = ParseJagging(nodes);

            return Grouping(jaggingResult);
        }

        /// <summary>
        /// Convert a html tag to an element of markdown.
        /// this result contains a block element and an inline element.
        /// </summary>
        public IEnumerable<TextElement> ParseJagging(IEnumerable<HtmlNode> nodes)
        {
            bool isPrevBlock = true;
            TextElement? lastElement = null;

            foreach (var node in nodes)
            {
                if (node.IsComment())
                    continue;

                // remove blank text between the blocks.
                if (isPrevBlock
                    && node is HtmlTextNode txt
                    && String.IsNullOrWhiteSpace(txt.Text))
                    continue;

                foreach (var element in ParseBlockAndInline(node))
                {
                    lastElement = element;
                    yield return element;
                }

                isPrevBlock = lastElement is Block;
            }
        }

        /// <summary>
        /// Convert a html tag to an element of markdown.
        /// Only tag node and text node are accepted.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<TextElement> ParseBlockAndInline(HtmlNode node)
        {
            if (_bindParsers.TryGetValue(node.Name.ToLower(), out var binds))
            {
                foreach (var bind in binds)
                {
                    if (bind.TryReplace(node, this, out var parsed))
                    {
                        return parsed;
                    }
                }
            }

            switch (UnknownTags)
            {
                case UnknownTagsOption.PassThrough:
                    return HtmlUtils.IsBlockTag(node.Name) ?
                        new[] { new Paragraph(new Run() { Text = node.OuterHtml }) } :
                        new[] { new Run(node.OuterHtml) };

                case UnknownTagsOption.Drop:
                    return Array.Empty<TextElement>();

                case UnknownTagsOption.Bypass:
                    return ParseJagging(node.ChildNodes);

                case UnknownTagsOption.Raise:
                default:
                    throw new UnknownTagException(node);
            }
        }

        public IEnumerable<Block> ParseBlock(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (var node in doc.DocumentNode.ChildNodes)
                foreach (var block in ParseBlock(node))
                    yield return block;
        }

        public IEnumerable<Inline> ParseInline(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (var node in doc.DocumentNode.ChildNodes)
                foreach (var inline in ParseInline(node))
                    yield return inline;
        }

        public IEnumerable<Block> ParseBlock(HtmlNode node)
        {
            if (_blockBindParsers.TryGetValue(node.Name.ToLower(), out var binds))
            {
                foreach (var bind in binds)
                {
                    if (bind.TryReplace(node, this, out var parsed))
                    {
                        return parsed;
                    }
                }
            }

            switch (UnknownTags)
            {
                case UnknownTagsOption.PassThrough:
                    return new[] {
                        new Paragraph(
                            HtmlUtils.IsBlockTag(node.Name) ?
                                new Run() { Text = node.OuterHtml }:
                                new Run(node.OuterHtml)
                        )
                    };

                case UnknownTagsOption.Drop:
                    return Array.Empty<Block>();

                case UnknownTagsOption.Bypass:
                    return node.ChildNodes
                               .SkipComment()
                               .SelectMany(nd => ParseBlock(nd));

                case UnknownTagsOption.Raise:
                default:
                    throw new UnknownTagException(node);
            }
        }

        public IEnumerable<Inline> ParseInline(HtmlNode node)
        {
            if (_inlineBindParsers.TryGetValue(node.Name.ToLower(), out var binds))
            {
                foreach (var bind in binds)
                {
                    if (bind.TryReplace(node, this, out var parsed))
                    {
                        return parsed;
                    }
                }
            }

            switch (UnknownTags)
            {
                case UnknownTagsOption.PassThrough:
                    return HtmlUtils.IsBlockTag(node.Name) ?
                        new[] { new Run() { Text = node.OuterHtml } } :
                        new[] { new Run(node.OuterHtml) };

                case UnknownTagsOption.Drop:
                    return Array.Empty<Inline>();

                case UnknownTagsOption.Bypass:
                    return node.ChildNodes
                               .SkipComment()
                               .SelectMany(nd => ParseInline(nd));

                case UnknownTagsOption.Raise:
                default:
                    throw new UnknownTagException(node);
            }
        }

        /// <summary>
        /// Convert IMdElement to IMdBlock.
        /// Inline elements are aggreated into paragraph.
        /// </summary>
        public IEnumerable<Block> Grouping(IEnumerable<TextElement> elements)
        {
            Paragraph? Group(IList<Inline> inlines)
            {
                // trim whiltepace plain

                while (inlines.Count > 0)
                {
                    if (inlines[0] is Run run
                        && String.IsNullOrWhiteSpace(run.Text))
                    {
                        inlines.RemoveAt(0);
                    }
                    else break;
                }

                while (inlines.Count > 0)
                {
                    if (inlines[inlines.Count - 1] is Run run
                        && String.IsNullOrWhiteSpace(run.Text))
                    {
                        inlines.RemoveAt(inlines.Count - 1);
                    }
                    else break;
                }

                using (var list = inlines.GetEnumerator())
                {
                    Inline? prev = null;

                    if (list.MoveNext())
                    {
                        prev = list.Current;
                        DocUtils.TrimStart(prev);

                        while (list.MoveNext())
                        {
                            var now = list.Current;

                            if (now is LineBreak)
                            {
                                DocUtils.TrimEnd(prev);

                                if (list.MoveNext())
                                {
                                    now = list.Current;
                                    DocUtils.TrimStart(now);
                                }
                            }

                            prev = now;
                        }
                    }

                    if (prev is not null)
                        DocUtils.TrimEnd(prev);
                }

                if (inlines.Count > 0)
                {
                    var para = new Paragraph();
                    para.Inlines.AddRange(inlines);
                    return para;
                }
                return null;
            }

            List<Inline> stored = new();
            foreach (var e in elements)
            {
                if (e is Inline inline)
                {
                    stored.Add(inline);
                    continue;
                }

                // grouping inlines
                if (stored.Count != 0)
                {
                    var para = Group(stored);
                    if (para is not null) yield return para;
                    stored.Clear();
                }

                yield return (Block)e;
            }

            if (stored.Count != 0)
            {
                var para = Group(stored);
                if (para is not null) yield return para;
                stored.Clear();
            }
        }

        private HtmlNode? PickBodyOrHead(HtmlNode documentNode, string headOrBody)
        {
            // html?
            foreach (var child in documentNode.ChildNodes)
            {
                if (child.Name == HtmlTextNode.HtmlNodeTypeNameText
                    || child.Name == HtmlTextNode.HtmlNodeTypeNameComment)
                    continue;

                switch (child.Name.ToLower())
                {
                    case "html":
                        // body? head?
                        foreach (var descendants in child.ChildNodes)
                        {
                            if (descendants.Name == HtmlTextNode.HtmlNodeTypeNameText
                                || descendants.Name == HtmlTextNode.HtmlNodeTypeNameComment)
                                continue;
                            switch (descendants.Name.ToLower())
                            {
                                case "head":
                                    if (headOrBody == "head")
                                        return descendants;
                                    break;

                                case "body":
                                    if (headOrBody == "body")
                                        return descendants;
                                    break;

                                default:
                                    return null;
                            }
                        }
                        break;

                    case "head":
                        if (headOrBody == "head")
                            return child;
                        break;

                    case "body":
                        if (headOrBody == "body")
                            return child;
                        break;

                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
