using System;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Scraping
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

        public string tagData(HtmlAgilityPack.HtmlDocument document, string xpath) // var tipini dinamik olarak dönderiyorum
        {
            var selectedHtml = xpath;
            StringBuilder sbuilder = new StringBuilder();

            var selectedH_list = document.DocumentNode.SelectNodes(selectedHtml);

            foreach (var item in selectedH_list)
            {
                foreach (var Inneritem in item.SelectNodes("li"))
                {
                    /* ekşi'de reklam hizmetinden dolayı böyle bir satır düşüyor olabilir. onu devredışı 
                    bırakmak için o satırı stringimin içerisine eklemiyorum.*/
                    if (!Inneritem.InnerText.ToString().Contains("NativeAdPub.push"))
                    {
                        sbuilder.AppendLine(Inneritem.InnerText);
                    }
                }
            }

            // gelen veride boş satırlar olabiliyor bu yüzden onları işin içinden temizliyoruz
            string returnedData = sbuilder.ToString();
            return Regex.Replace(returnedData, @"^\s+$[\r]*", string.Empty, RegexOptions.Multiline);
        }
    }
}