using System;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Gundem_TelegramBot
{
    public class Parsing
    {
        public HtmlAgilityPack.HtmlDocument hookSite(string link)
        {
            Uri url = new Uri(link);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string html = client.DownloadString(url);

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        public dynamic tagData(HtmlAgilityPack.HtmlDocument document, string xpath) // var tipini dinamik olarak dönderiyorum
        {
            var selectedHtml = xpath;
            var selectedH_list = document.DocumentNode.SelectNodes(selectedHtml);
            return selectedH_list;
        }
    }
}