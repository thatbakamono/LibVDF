using Xunit;
using System;

namespace LibVDF.Tests
{
    public class ParserTests
    {
        [Fact]
        public void TestFieldParsing()
        {
            var parser = new VDFParser();
            var result = parser.ParseContent($"\"name\" \"Kurisu\"{Environment.NewLine}\"surname\" \"Makise\"");

            Assert.Equal("Kurisu", result.GetString("name"));
            Assert.Equal("Makise", result.GetString("surname"));
        }

        [Fact]
        public void TestObjectParsing()
        {
            var parser = new VDFParser();
            var result = parser.ParseContent($"\"info\"{Environment.NewLine}{{{Environment.NewLine}\"name\" \"Kurisu\"{Environment.NewLine}\"surname\" \"Makise\"{Environment.NewLine}}}");
            var info = result.GetObject("info");

            Assert.Equal("Kurisu", info.GetString("name"));
            Assert.Equal("Makise", info.GetString("surname"));
        }
    }
}