using HtmlAgilityPack;
using HtmlXaml.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HtmlXaml.Core.Parsers
{
    public class ImageParser : IInlineTagParser
    {
        public IEnumerable<string> SupportTag => new[] { "img", "image" };

        bool ITagParser.TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<TextElement> generated)
        {
            var rtn = TryReplace(node, manager, out var list);
            generated = list;
            return rtn;
        }

        public bool TryReplace(HtmlNode node, ReplaceManager manager, out IEnumerable<Inline> generated)
        {
            var link = node.Attributes["src"]?.Value;
            var alt = node.Attributes["alt"]?.Value;
            if (link is null)
            {
                generated = EnumerableExt.Empty<Inline>();
                return false;
            }
            var title = node.Attributes["title"]?.Value;

            var imgSource = manager.LoadImage(link);

            if (imgSource is null)
            {
                generated = new[] { new Run("!" + (alt ?? link)) { Foreground = Brushes.Red } };
            }
            else
            {
                var widthTxt = node.Attributes["width"]?.Value;
                var heightTxt = node.Attributes["height"]?.Value;

                var image = new Image
                {
                    Source = imgSource,
                    ToolTip = title
                };

                if (!string.IsNullOrEmpty(heightTxt)
                    && Length.TryParse(heightTxt, out var heightLen))
                {
                    if (heightLen.Unit == Unit.Percentage)
                    {
                        image.SetBinding(
                            Image.HeightProperty,
                            new Binding(nameof(Image.Width))
                            {
                                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                Converter = new MultiplyConverter(heightLen.Value / 100)
                            });
                    }
                    else
                    {
                        image.Height = heightLen.ToPoint();
                    }
                }

                // Bind size so document is updated when image is downloaded
                if (!String.IsNullOrEmpty(widthTxt)
                    && Length.TryParse(widthTxt, out var widthLen))
                {
                    if (widthLen.Unit == Unit.Percentage)
                    {
                        image.Loaded += (s, e) =>
                        {
                            var owner = (Image)s;

                            var parent = owner.Parent;

                            for (; ; )
                            {
                                if (parent is FrameworkElement element)
                                {
                                    parent = element;
                                    break;
                                }
                                else if (parent is FrameworkContentElement content)
                                {
                                    parent = content.Parent;
                                }
                                else break;
                            }

                            if (parent is FlowDocumentScrollViewer)
                            {
                                var binding = CreateMultiBindingForFlowDocumentScrollViewer();
                                binding.Converter = new MultiMultiplyConverter2(widthLen.Value / 100);
                                owner.SetBinding(Image.WidthProperty, binding);
                            }
                            else
                            {
                                var binding = CreateBinding(nameof(FrameworkElement.ActualWidth), typeof(FrameworkElement));
                                binding.Converter = new MultiplyConverter(widthLen.Value / 100);
                                owner.SetBinding(Image.WidthProperty, binding);
                            }
                        };
                    }
                    else
                    {
                        image.Width = widthLen.ToPoint();
                    }
                }
                else if (imgSource.IsDownloading)
                {
                    Binding binding = new(nameof(BitmapImage.Width));
                    binding.Source = imgSource;
                    binding.Mode = BindingMode.OneWay;

                    BindingExpressionBase bindingExpression = BindingOperations.SetBinding(image, Image.WidthProperty, binding);
                    void downloadCompletedHandler(object? sender, EventArgs e)
                    {
                        imgSource.DownloadCompleted -= downloadCompletedHandler;
                        imgSource.Freeze();
                        bindingExpression.UpdateTarget();
                    }

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
        private MultiBinding CreateMultiBindingForFlowDocumentScrollViewer()
        {
            var binding = new MultiBinding();

            var totalWidth = CreateBinding(nameof(FlowDocumentScrollViewer.ActualWidth), typeof(FlowDocumentScrollViewer));
            var verticalBarVis = CreateBinding(nameof(FlowDocumentScrollViewer.VerticalScrollBarVisibility), typeof(FlowDocumentScrollViewer));

            binding.Bindings.Add(totalWidth);
            binding.Bindings.Add(verticalBarVis);

            return binding;
        }

        private static Binding CreateBinding(string propName, Type ancestorType)
        {
            return new Binding(propName)
            {
                RelativeSource = new RelativeSource()
                {
                    Mode = RelativeSourceMode.FindAncestor,
                    AncestorType = ancestorType,
                }
            };
        }

        class MultiplyConverter : IValueConverter
        {
            public double Value { get; }

            public MultiplyConverter(double v)
            {
                Value = v;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return Value * (Double)value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ((Double)value) / Value;
            }
        }
        class MultiMultiplyConverter2 : IMultiValueConverter
        {
            public double Value { get; }

            public MultiMultiplyConverter2(double v)
            {
                Value = v;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var value = (double)values[0];
                var visibility = (ScrollBarVisibility)values[1];

                if (visibility == ScrollBarVisibility.Visible)
                {
                    return Value * (value - SystemParameters.VerticalScrollBarWidth);
                }
                else
                {
                    return Value * (Double)value;
                }
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
