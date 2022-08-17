﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HtmlXaml.Core.Parsers
{
    public class InputParser : IInlineTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "input" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            var type = node.Attributes["type"]?.Value ?? "text";

            double? width = null;
            var widAttr = node.Attributes["width"];
            var sizAttr = node.Attributes["size"];

            if (widAttr is not null)
            {
                if (double.TryParse(widAttr.Value, out var v))
                    width = v;
            }
            if (sizAttr is not null)
            {
                if (int.TryParse(sizAttr.Value, out var v))
                    width = v * 7;
            }

            InlineUIContainer inline;
            switch (type)
            {
                default:
                case "text":
                    var txt = new TextBox()
                    {
                        Text = node.Attributes["value"]?.Value ?? "",
                        IsReadOnly = true,
                    };
                    if (width.HasValue) txt.Width = width.Value;


                    inline = new InlineUIContainer(txt);
                    break;


                case "button":
                case "reset":
                case "submit":
                    var btn = new Button()
                    {
                        Content = node.Attributes["value"]?.Value ?? "",
                        IsEnabled = false,
                    };
                    if (width.HasValue) btn.Width = width.Value;

                    inline = new InlineUIContainer(btn);
                    break;


                case "radio":
                    var radio = new RadioButton()
                    {
                        IsEnabled = false,
                    };
                    if (node.Attributes["checked"] != null) radio.IsChecked = true;

                    inline = new InlineUIContainer(radio);
                    break;


                case "checkbox":
                    var chk = new CheckBox()
                    {
                        IsEnabled = false
                    };
                    if (node.Attributes["checked"] != null)
                        chk.IsChecked = true;

                    inline = new InlineUIContainer(chk);
                    break;


                case "range":
                    var slider = new Slider()
                    {
                        IsEnabled = false,
                        Minimum = 0,
                        Value = 50,
                        Maximum = 100,
                    };

                    var minAttr = node.Attributes["min"];
                    if (minAttr is not null && double.TryParse(minAttr.Value, out var minVal))
                    {
                        slider.Minimum = minVal;
                    }

                    var maxAttr = node.Attributes["max"];
                    if (maxAttr is not null && double.TryParse(maxAttr.Value, out var maxVal))
                    {
                        slider.Maximum = maxVal;
                    }

                    var valAttr = node.Attributes["val"];
                    if (valAttr is not null && double.TryParse(valAttr.Value, out var val))
                    {
                        slider.Value = val;
                    }

                    slider.Value = (slider.Minimum + slider.Maximum) / 2;

                    inline = new InlineUIContainer(slider);
                    break;
            }

            generated = new[] { inline
    };
            return true;
        }
    }
}
