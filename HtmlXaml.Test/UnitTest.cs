
using ApprovalTests;
using ApprovalTests.Reporters;
using HtmlXaml.Core;
using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Windows.Documents;

namespace HtmlXaml.Test
{
    [UseReporter(typeof(DiffReporter))]
    public class UnitTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Button()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CodeBlock()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void InlineCode()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Input()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void List()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Progres()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TypicalBlock()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TypicalInline()
        {
            ReplaceManager manager = new();
            var html = Utils.ReadHtml();

            var doc = new FlowDocument();
            doc.Blocks.AddRange(manager.Parse(html));

            var xaml = Utils.AsXaml(doc);

            Approvals.Verify(xaml);
        }
    }
}
