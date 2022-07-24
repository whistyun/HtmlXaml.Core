using HtmlAgilityPack;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HtmlXaml.Core.Parsers
{
    public class ButtonParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "button" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.ParseAndGroup(node.ChildNodes));

            var box = new FlowDocumentScrollViewer();
            box.Margin = new Thickness(0);
            box.Padding = new Thickness(0);
            box.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            box.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            box.Document = doc;

            box.Loaded += (s, e) =>
            {
                var desiredWidth = box.DesiredSize.Width;
                var desiredHeight = box.DesiredSize.Height;


                for (int i = 0; i < 10; ++i)
                {
                    desiredWidth /= 2;
                    var size = new Size(desiredWidth, double.PositiveInfinity);

                    box.Measure(size);

                    if (desiredHeight != box.DesiredSize.Height) break;

                    // Give up because it will not be wrapped back.
                    if (i == 9) return;
                }

                var preferedWidth = desiredWidth * 2;

                for (int i = 0; i < 10; ++i)
                {
                    var width = (desiredWidth + preferedWidth) / 2;

                    var size = new Size(width, double.PositiveInfinity);
                    box.Measure(size);

                    if (desiredHeight == box.DesiredSize.Height)
                    {
                        preferedWidth = width;
                    }
                    else
                    {
                        desiredWidth = width;
                    }
                }

                box.Width = preferedWidth;
            };


            var btn = new Button();
            btn.Content = box;
            btn.IsEnabled = false;

            generated = new[] { new InlineUIContainer(btn) };
            return true;
        }
    }
}
