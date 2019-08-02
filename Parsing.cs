using System;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Telegram.Bot;


namespace Gundem_TelegramBot
{
    public class Parsing
    {
        public HtmlDocument HookSite(string link)
        {
            Uri url = new Uri(link);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string html = client.DownloadString(url);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        public dynamic TagData(HtmlDocument document, string xpath) // var tipini dinamik olarak dönderiyorum
        {
            var selectedHtml = xpath;
            var selectedH_list = document.DocumentNode.SelectNodes(selectedHtml);
            return selectedH_list;
        }
    }
}