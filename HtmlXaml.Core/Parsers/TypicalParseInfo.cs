﻿using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class TypicalParseInfo
    {
        public string HtmlTag { get; }
        public string FlowDocumentTagText { get; }
        public Type? FlowDocumentTag { get; }
        public string? TagNameReference { get; }
        public Tags TagName { get; }
        public string? ExtraModifyName { get; }

        private MethodInfo? _method;

        public TypicalParseInfo(string[] line)
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

            if (TagNameReference is not null)
            {
                TagName = (Tags)Enum.Parse(typeof(Tags), TagNameReference);
            }

            if (ExtraModifyName is not null)
            {
                _method = this.GetType().GetMethod("ExtraModify" + ExtraModifyName);

                if (_method is null)
                    throw new InvalidOperationException("unknown method ExtraModify" + ExtraModifyName);
            }

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

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            // create instance

            if (FlowDocumentTag is null)
            {
                switch (FlowDocumentTagText)
                {
                    case "#blocks":
                        generated = manager.ParseAndGroup(node.ChildNodes).ToArray();
                        break;

                    case "#jagging":
                        generated = manager.ParseJagging(node.ChildNodes).ToArray();
                        break;

                    case "#inlines":
                        if (manager.ParseJagging(node.ChildNodes).TryCast<Inline>(out var inlines))
                        {
                            generated = inlines.ToArray();
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
                var tag = (TextElement)Activator.CreateInstance(FlowDocumentTag)!;

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

            if (TagNameReference is not null)
            {
                foreach (var tag in generated)
                {
                    tag.Tag = manager.GetTag(TagName);
                }
            }

            // extra modify
            if (_method is not null)
            {
                foreach (var tag in generated)
                {
                    _method.Invoke(this, new object[] { tag, node, manager });
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

        public void ExtraModifyCenter(Section center, HtmlNode node, ReplaceManager manager)
        {
            center.TextAlignment = TextAlignment.Center;
        }

        public static IEnumerable<TypicalParseInfo> Load(string resourcePath)
        {
            using var stream = Assembly.GetExecutingAssembly()
                                       .GetManifestResourceStream(resourcePath);

            if (stream is null)
                throw new ArgumentException($"resource not found: '{resourcePath}'");

            using var reader = new StreamReader(stream!);
            while (reader.ReadLine() is string line)
            {
                if (line.StartsWith("#")) continue;

                var elements = line.Split('|').Select(t => t.Trim()).ToArray();
                yield return new TypicalParseInfo(elements);
            }
        }
    }
}
