﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Markup;
using System.Xml;

namespace HtmlXaml.Test
{
    public static class Utils
    {
        public static string ReadHtml([CallerMemberName] string fileBaseName = null)
        {
            if (fileBaseName is null)
                throw new ArgumentNullException(nameof(fileBaseName));

            var asm = Assembly.GetCallingAssembly();
            var asmDir = Path.GetDirectoryName(asm.Location);
            var path = Path.Combine(asmDir, "Input", fileBaseName + ".html");
            return File.ReadAllText(path);
        }

        public static string AsXaml(object instance)
        {
            using (var writer = new StringWriter())
            {
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    XamlWriter.Save(instance, xmlWriter);
                }

                writer.WriteLine();
                return writer.ToString();
            }
        }
    }
}
