using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace HtmlXaml.Core.Parsers
{
    public class TypicalParser : ITagParser
    {
        Dictionary<string, ParseInfo> _infos;
        Dictionary<string, MethodInfo> _methods;

        public TypicalParser()
        {
            _infos = new();
            _methods = new();

            using var stream = Assembly.GetExecutingAssembly()
                                       .GetManifestResourceStream("HtmlXaml.Core.Parsers.TypicalParser.tsv");
            using var reader = new StreamReader(stream!);


            reader.ReadLine();
            while (reader.ReadLine() is string line)
            {
                var elements = line.Split('|').Select(t => t.Trim()).ToArray();
                var info = new ParseInfo(elements);
                _infos[info.HtmlTag] = info;
            }
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            if (!_infos.TryGetValue(node.Name.ToLower(), out ParseInfo? info))
            {
                generated = Array.Empty<TextElement>();
                return false;
            }


            // create instance

            if (info.FlowDocumentTag is null)
            {
                switch (info.FlowDocumentTagText)
                {
                    case "#blocks":
                        generated = manager.ParseAndGroup(node.ChildNodes);
                        break;

                    case "#jagging":
                        generated = manager.ParseJagging(node.ChildNodes);
                        break;

                    case "#inlines":
                        if (manager.ParseJagging(node.ChildNodes).TryCast<Inline>(out var inlines))
                        {
                            generated = inlines;
                            break;
                        }
                        else
                        {
                            generated = Array.Empty<TextElement>();
                            return false;
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }
            else
            {
                var tag = (TextElement)Activator.CreateInstance(info.FlowDocumentTag)!;

                var cntInlines = (tag as Paragraph)?.Inlines ?? (tag as Span)?.Inlines;
                if (cntInlines is not null)
                {
                    if (manager.ParseJagging(node.ChildNodes).TryCast<Inline>(out var parsed))
                    {
                        cntInlines.AddRange(parsed);
                    }
                    else
                    {
                        generated = Array.Empty<TextElement>();
                        return false;
                    }

                }
                else if (tag is Run run)
                {
                    run.Text = node.InnerText;
                }
                else if (tag is Section section)
                {
                    section.Blocks.AddRange(manager.ParseAndGroup(node.ChildNodes));
                }
                else if (tag is not LineBreak)
                {
                    throw new InvalidOperationException();
                }

                generated = new[] { tag };
            }

            // apply tag

            if (info.TagNameReference is not null)
            {
                var tagVal = manager.GetTag((Tags)Enum.Parse(typeof(Tags), info.TagNameReference));

                foreach (var tag in generated)
                {
                    tag.Tag = tagVal;
                }
            }

            // extra modify

            if (info.ExtraModifyName is not null)
            {
                if (! _methods.TryGetValue(info.ExtraModifyName, out var method))
                {
                    method = this.GetType().GetMethod("ExtraModify" + info.ExtraModifyName);

                    if (method is null)
                        throw new InvalidOperationException("unknown method ExtraModify" + info.ExtraModifyName);

                    _methods[info.ExtraModifyName] = method;
                }

                foreach (var tag in generated)
                {
                    method.Invoke(this, new object[] { tag, node, manager });
                }
            }

            return true;
        }


        public void ExtraModifyHyperlink(Hyperlink link, HtmlNode node, ReplaceManager manager)
        {
            var href = node.Attributes["href"]?.Value;

            if (href is not null)
            {
                link.CommandParameter = href;
                link.Command = manager.HyperlinkCommand;
            }
        }

        public void ExtraModifyStrikethrough(Span span, HtmlNode node, ReplaceManager manager)
        {
            span.TextDecorations = TextDecorations.Strikethrough;
        }

        public void ExtraModifySubscript(Run run, HtmlNode node, ReplaceManager manager)
        {
            Typography.SetVariants(run, FontVariants.Subscript);
        }

        public void ExtraModifySuperscript(Run run, HtmlNode node, ReplaceManager manager)
        {
            Typography.SetVariants(run, FontVariants.Superscript);
        }

        public void ExtraModifyAcronym(Span span, HtmlNode node, ReplaceManager manager)
        {
            var title = node.Attributes["title"]?.Value;
            if (title is not null)
                span.ToolTip = title;
        }



        class ParseInfo
        {
            public string HtmlTag { get; }
            public string FlowDocumentTagText { get; }
            public Type? FlowDocumentTag { get; }
            public string? TagNameReference { get; }
            public string? ExtraModifyName { get; }

            public ParseInfo(string[] line)
            {
                FlowDocumentTagText = line[1];

                if (FlowDocumentTagText.StartsWith("#"))
                {
                    FlowDocumentTag = null;
                }
                else
                {
                    Type? elementType = AppDomain.CurrentDomain
                                                 .GetAssemblies()
                                                 .Select(asm => asm.GetType(FlowDocumentTagText))
                                                 .OfType<Type>()
                                                 .FirstOrDefault();

                    if (elementType is null)
                        throw new ArgumentException($"Failed to load type '{line[1]}'");

                    FlowDocumentTag = elementType;
                }


                HtmlTag = line[0];
                TagNameReference = GetArrayAt(line, 2);
                ExtraModifyName = GetArrayAt(line, 3);


                string? GetArrayAt(string[] array, int idx)
                {
                    if (idx < array.Length
                        && !string.IsNullOrWhiteSpace(array[idx]))
                    {
                        return array[idx];
                    }
                    return null;
                }
            }
        }
    }
}
