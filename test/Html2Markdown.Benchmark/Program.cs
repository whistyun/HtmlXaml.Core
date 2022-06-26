using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace Html2Markdown.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }

    public class Executor
    {
        private Converter _converter;
        private string _markdown;

        public Executor()
        {
            _converter = new Converter();

            using (var stream = typeof(Program).Assembly.GetManifestResourceStream("Html2Markdown.Benchmark.TargetText.html"))
            using (var reader = new StreamReader(stream))
                _markdown = reader.ReadToEnd();
        }

        [Benchmark]
        public string Convert()
        {
            return _converter.Convert(_markdown);
        }
    }
}
