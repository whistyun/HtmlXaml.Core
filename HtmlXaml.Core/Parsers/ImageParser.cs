using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HtmlXaml.Core.Parsers
{
    public class ImageParser : ISimpleTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "img", "image" };

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var link = node.Attributes["src"]?.Value;
            var alt = node.Attributes["alt"]?.Value;
            if (link is null)
            {
                generated = Array.Empty<TextElement>();
                return false;
            }
            var title = node.Attributes["title"]?.Value;


            var imgSource = manager.LoadImage(link);


            if (imgSource is null)
            {
                generated = new[] { new Run("!" + alt ?? link) { Foreground = Brushes.Red } };
            }
            else
            {
                Image image = new Image { Source = imgSource };
                image.ToolTip = title;

                // Bind size so document is updated when image is downloaded
                if (imgSource.IsDownloading)
                {
                    Binding binding = new Binding(nameof(BitmapImage.Width));
                    binding.Source = imgSource;
                    binding.Mode = BindingMode.OneWay;

                    BindingExpressionBase bindingExpression = BindingOperations.SetBinding(image, Image.WidthProperty, binding);
                    EventHandler downloadCompletedHandler = null;
                    downloadCompletedHandler = (sender, e) =>
                    {
                        imgSource.DownloadCompleted -= downloadCompletedHandler;
                        imgSource.Freeze();
                        bindingExpression.UpdateTarget();
                    };
                    imgSource.DownloadCompleted += downloadCompletedHandler;
                }
                else
                {
                    image.Width = imgSource.Width;
                }

                var container = new InlineUIContainer() { Child = image };
                imgSource.DownloadFailed += (s, e) =>
                {
                    var ext = e.ErrorException;

                    var label = new Label()
                    {
                        Foreground = Brushes.Red,
                        Content = "!" + link + "\r\n" + ext.GetType().Name + ":" + ext.Message
                    };

                    container.Child = label;
                };

                generated = new[] { container };
            }

            return true;
        }
    }
}
