using HtmlXaml.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace HtmlXaml.Demo
{
    public class TextToFlowDocumentConverter : IValueConverter
    {
        ReplaceManager manager = new ReplaceManager();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                var blocks=manager.Parse(text);

                var doc = new FlowDocument();
                doc.Blocks.AddRange(blocks);

                return doc;
            }
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
