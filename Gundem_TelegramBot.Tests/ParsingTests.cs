using HtmlAgilityPack;
using System;
using System.Linq;
using Xunit;

namespace Gundem_TelegramBot.Tests
{
    public class ParsingTests
    {
        [Fact]
        public void ParserTest()
        {
            Parsing parsed = new Parsing();
            HtmlDocument hookedDocument = parsed.HookSite("http://eksisozluk.com");

            var title = hookedDocument.DocumentNode.SelectSingleNode("//title")?.InnerText;
            Assert.Equal("ekşi sözlük - kutsal bilgi kaynağı", title);
        }
    }
}
