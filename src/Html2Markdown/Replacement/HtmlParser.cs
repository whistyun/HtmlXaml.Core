﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqExtensions;

namespace Html2Markdown.Replacement
{
	internal static class HtmlParser
	{
		private static readonly Regex NoChildren = new Regex(@"<(ul|ol)\b[^>]*>(?:(?!<ul|<ol)[\s\S])*?<\/\1>");

		internal static string ReplaceLists(string html)
		{
			while (HasNoChildLists(html))
			{
				var listToReplace = NoChildren.Match(html).Value;
				var formattedList = ReplaceList(listToReplace);
				html = html.Replace(listToReplace, formattedList);
			}

			return html;
		}

		private static string ReplaceList(string html)
		{
			var list = Regex.Match(html, @"<(ul|ol)\b[^>]*>([\s\S]*?)<\/\1>");
			var listType = list.Groups[1].Value;
			var listItems = list.Groups[2].Value.Split(new[] { "</li>" }, StringSplitOptions.None);

			var counter = 0;
			var markdownList = new List<string>();
			listItems.Each(listItem =>
				{
					var listPrefix = (listType.Equals("ol")) ? string.Format("{0}.  ", ++counter) : "*   ";
					var finalList = Regex.Replace(listItem, @"<li[^>]*>", string.Empty);

					if (finalList.Trim().Length == 0) return;

					finalList = Regex.Replace(finalList, @"^\s+", string.Empty);
					finalList = Regex.Replace(finalList, @"\n{2}", string.Format("{0}{1}    ", Environment.NewLine, Environment.NewLine));
					// indent nested lists
					finalList = Regex.Replace(finalList, @"\n([ ]*)+(\*|\d+\.)", string.Format("{0}$1    $2", "\n"));
					markdownList.Add(string.Format("{0}{1}", listPrefix, finalList));
				});

			return Environment.NewLine + Environment.NewLine + markdownList.Aggregate((current, item) => current + Environment.NewLine + item);
		}

		private static bool HasNoChildLists(string html)
		{
			return NoChildren.Match(html).Success;
		}

		internal static string ReplacePre(string html)
		{
			var preTags = new Regex(@"<pre\b[^>]*>([\s\S]*)<\/pre>").Matches(html);

			return preTags.Cast<Match>().Aggregate(html, ConvertPre);
		}

		private static string ConvertPre(string html, Match preTag)
		{
			var tag = preTag.Groups[1].Value;
			tag = TabsToSpaces(tag);
			tag = IndentNewLines(tag);
			html = html.Replace(preTag.Value, Environment.NewLine + Environment.NewLine + tag + Environment.NewLine);
			return html;
		}

		private static string IndentNewLines(string tag)
		{
			return tag.Replace(Environment.NewLine, Environment.NewLine + "    ");
		}

		private static string TabsToSpaces(string tag)
		{
			return tag.Replace("\t", "    ");
		}

		internal static string ReplaceImg(string html)
		{
			var originalImages = new Regex(@"<img([^>]+)>").Matches(html);
			originalImages.Cast<Match>().Each(image =>
				{
					var img = image.Value;
					var src = AttributeParser(img, "src");
					var alt = AttributeParser(img, "alt");
					var title = AttributeParser(img, "title");

					html = html.Replace(img, string.Format(@"![{0}]({1}{2})", alt, src, (title.Length > 0) ? string.Format(" \"{0}\"", title) : ""));
				});

			return html;
		}

		public static string ReplaceAnchor(string html)
		{
			var originalAnchors = new Regex(@"<a[^>]+>[^<]+</a>").Matches(html);
			originalAnchors.Cast<Match>().Each(anchor =>
				{
					var a = anchor.Value;
					var linkText = GetLinkText(a);
					var href = AttributeParser(a, "href");
					var title = AttributeParser(a, "title");

					html = html.Replace(a, string.Format(@"[{0}]({1}{2})", linkText, href, (title.Length > 0) ? string.Format(" \"{0}\"", title) : ""));
				});

			return html;
		}

		private static string GetLinkText(string link)
		{
			var match = Regex.Match(link, @"<a[^>]+>([^<]+)</a>");
			var groups = match.Groups;
			return groups[1].Value;
		}

		public static string ReplaceCode(string html)
		{
			var singleLineCodeBlocks = new Regex(@"<code>([^\n]*?)</code>").Matches(html);
			singleLineCodeBlocks.Cast<Match>().Each(block =>
				{
					var code = block.Value;
					var content = GetCodeContent(code);
					html = html.Replace(code, string.Format("`{0}`", content));
				});

			var multiLineCodeBlocks = new Regex(@"<code>([^>]*?)</code>").Matches(html);
			multiLineCodeBlocks.Cast<Match>().Each(block =>
				{
					var code = block.Value;
					var content = GetCodeContent(code);
					content = IndentLines(content).TrimEnd() + Environment.NewLine + Environment.NewLine;
					html = html.Replace(code, string.Format("{0}    {1}", Environment.NewLine, TabsToSpaces(content)));
				});

			return html;
		}

		private static string IndentLines(string content)
		{
			var lines = content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

			return lines.Aggregate("", (current, line) => current + IndentLine(line));
		}

		private static string IndentLine(string line)
		{
			if (line.Trim().Equals(string.Empty)) return "";
			return line + Environment.NewLine + "    ";
		}

		private static string GetCodeContent(string code)
		{
			var match = Regex.Match(code, @"<code[^>]*?>([^<]*?)</code>");
			var groups = match.Groups;
			return groups[1].Value;
		}

		private static string AttributeParser(string html, string attribute)
		{
			var match = Regex.Match(html, string.Format(@"{0}\s*=\s*[""\']?([^""\']*)[""\']?", attribute));
			var groups = match.Groups;
			return groups[1].Value;
		}
	}
}