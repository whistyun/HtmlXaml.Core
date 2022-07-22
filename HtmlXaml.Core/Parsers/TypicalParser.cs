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

        public TypicalParser()
        {
            _infos = new();

            using var stream = Assembly.GetExecutingAssembly()
                                        .GetManifestResourceStream("HtmlXaml.Core.Parsers.TypicalParser.tsv");
            using var reader = new StreamReader(stream!);


            var ptn = new Regex(" +");
            reader.ReadLine();
            while (reader.ReadLine() is string line)
            {
                var info = new ParseInfo(ptn.Split(line));
                _infos[info.HtmlTag] = info;
            }
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            if (_infos.TryGetValue(node.Name.ToLower(), out ParseInfo? info))
            {
                var tag = (TextElement)Activator.CreateInstance(info.FlowDocumentTag)!;

                if (info.ChildType != ChildType.Undefined)
                {

                    var addable = (IAddChild)tag;

                    switch (info.ChildType)
                    {
                        case ChildType.String:
                            addable.AddChild(node.InnerText);
                            break;

                        case ChildType.Inline:
                            if (manager.ParseJagging(node.ChildNodes).TryCast<Inline>(out var inlines))
                            {
                                foreach (var inline in inlines)
                                    addable.AddChild(inline);
                            }
                            else
                            {
                                generated = Array.Empty<TextElement>();
                                return false;
                            }
                            break;

                        case ChildType.Block:
                            foreach (var block in manager.ParseAndGroup(node.ChildNodes))
                                addable.AddChild(block);

                            break;
                    }
                }


                if (info.TagNameReference is not null)
                {
                    tag.Tag = manager.GetTag((Tags)Enum.Parse(typeof(Tags), info.TagNameReference));
                }


                if (info.ExtraModifyName is not null)
                {
                    switch (info.ExtraModifyName)
                    {
                        case "Hyperlink":
                            ExtraModifyHyperlink(node, (Hyperlink)tag, manager);
                            break;

                        case "Strikethrough":
                            ExtraModifyStrikethrough((Span)tag);
                            break;

                        case "Subscript":
                            ExtraModifySubscript((Run)tag);
                            break;

                        case "Superscript":
                            ExtraModifySuperscript((Run)tag);
                            break;
                    }
                }

                generated = new[] { tag };
                return true;
            }

            generated = Array.Empty<TextElement>();
            return false;
        }


        private void ExtraModifyHyperlink(HtmlNode node, Hyperlink link, ReplaceManager manager)
        {
            var href = node.Attributes["href"]?.Value;

            if (href is not null)
            {
                link.CommandParameter = href;
                link.Command = manager.HyperlinkCommand;
            }
        }

        private void ExtraModifyStrikethrough(Span span)
        {
            span.TextDecorations = TextDecorations.Strikethrough;
        }

        private void ExtraModifySubscript(Run run)
        {
            Typography.SetVariants(run, FontVariants.Subscript);
        }

        private void ExtraModifySuperscript(Run run)
        {
            Typography.SetVariants(run, FontVariants.Superscript);
        }


        class ParseInfo
        {
            public string HtmlTag { get; }
            public Type FlowDocumentTag { get; }
            public ChildType ChildType { get; }
            public string? TagNameReference { get; }
            public string? ExtraModifyName { get; }

            public ParseInfo(string[] line)
            {

                Type? elementType = AppDomain.CurrentDomain
                                             .GetAssemblies()
                                             .Select(asm => asm.GetType(line[1]))
                                             .OfType<Type>()
                                             .FirstOrDefault() ;

                if (elementType is null)
                    throw new ArgumentException($"Failed to load type '{line[1]}'");


                HtmlTag = line[0];
                FlowDocumentTag = elementType;
                ChildType = ChildType.Undefined;
                TagNameReference = null;
                ExtraModifyName = null;


                switch (line.Length)
                {
                    case 5:
                        ExtraModifyName = String.IsNullOrWhiteSpace(line[4]) ? null : line[4];
                        goto case 4;

                    case 4:
                        TagNameReference = String.IsNullOrWhiteSpace(line[3]) ? null : line[3];
                        goto case 3;

                    case 3:
                        ChildType = (ChildType)Enum.Parse(typeof(ChildType), line[2]);
                        goto case 2;

                    case 2:
                        return;

                    default:
                        throw new ArgumentException("line.Length");
                }
            }
        }

        enum ChildType
        {
            Undefined,
            Block,
            Inline,
            String,
        }
    }
}
