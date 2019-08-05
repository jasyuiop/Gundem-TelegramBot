using System;
using Telegram.Bot.Args;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace Gundem_TelegramBot
{
    public class Message
    {
        public static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Alınan Mesajın ChatId'si = {e.Message.Chat.Id}.");

                if (e.Message.Text == "/start")
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Merhaba, Ekşi sözlük gündemini, dünün en beğenilen entry'lerini, bana verilen entry numarasından entry'i sana gösterebilirim\nKullanabileceğin komutlar işte burada\n/gundem\n/debe\n/entry\n/yardim\n\n[Açık kaynak bir proje olduğumu biliyormusun? tıkla ve github üzerinde bana gözat](https://github.com/jasyuiop/Gundem-TelegramBot)",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );

                else if (e.Message.Text == "/yardim")
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Merhaba, Ekşi sözlük gündemini, dünün en beğenilen entry'lerini, bana verilen entry numarasından entry'i sana gösterebilirim\nKullanabileceğin komutlar işte burada\n/gundem\n/debe\n/entry\n\n[Açık kaynak bir proje olduğumu biliyormusun? tıkla ve github üzerinde bana gözat](https://github.com/jasyuiop/Gundem-TelegramBot)",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );

                else if (e.Message.Text == "/gundem")
                {
                    Parsing parsed = new Parsing();
                    HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    var returnedS_hList = parsed.TagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();

                    int sequence = 1;

                    foreach (var item in returnedS_hList)
                    {
                        foreach (var Inner_li in item.SelectNodes("li//a"))
                        {
                            /* ekşi'de reklam hizmetinden dolayı böyle bir satır düşüyor olabilir. onu devredışı 
                            bırakmak için o satırı stringimin içerisine eklemiyorum.*/
                            // if (!Inner_li.InnerText.ToString().Contains("NativeAdPub.push"))

                            // IsNullOrEmpty ile'de o reklam hizmetinden düşen satırı engelleyebiliyoruz
                            // ve böylesi daha sağlıklı.
                            if (!string.IsNullOrEmpty(Inner_li.ToString()))
                            {
                                HtmlAttribute href_att = Inner_li.Attributes["href"];
                                HtmlAttribute text_att = Inner_li.Attributes["text()"];
                                sbuilder.AppendLine(sequence + ". " + "[" + Inner_li.InnerText + "]" + "(" + "https://eksisozluk.com" + href_att.Value + ")");
                                sequence++;
                            }
                        }
                    }
                    string Data = sbuilder.ToString();
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    // gelen veride boş satırlar olabiliyor bu yüzden onları Regex kullanarak temizliyorum
                    text: Regex.Replace(Data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
                }

                else if (e.Message.Text == "/debe")
                {
                    Parsing parsed = new Parsing();
                    HtmlDocument hookedDocument = parsed.HookSite("https://sozlock.com/");
                    string xpath = @"/html/body/main/div/div[2]/ul";
                    var returnedS_hList = parsed.TagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();

                    foreach (var item in returnedS_hList)
                    {
                        foreach (var Inner_li in item.SelectNodes("li"))
                        {
                            Queue<string> links = new Queue<string>();
                            foreach (var Inner_divA in Inner_li.SelectNodes("div[2]//a"))
                            {
                                HtmlAttribute href_att = Inner_divA.Attributes["href"];
                                links.Enqueue(href_att.Value);
                            }
                            foreach (var Inner_h3 in Inner_li.SelectNodes("h3"))
                            {
                                if (!string.IsNullOrEmpty(Inner_h3.ToString()))
                                {
                                    /* h3'den gelen text'in içerisinde kesme işaretleri decode edilmemiş olarak dönüyor.
                                    bu yüzden kesme işareti içeren cümlelerde kesme işareti değil de "&#39" yazısı görünüyor.
                                    bunu önlemek için HtmlDecode kullanıyorum
                                    */
                                    sbuilder.AppendLine("[" + System.Web.HttpUtility.HtmlDecode(Inner_h3.InnerText) + "]" + "(" + links.Dequeue() + ")");
                                }
                            }
                        }
                    }
                    string Data = sbuilder.ToString();
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: Regex.Replace(Data, @"^\s+$[\r\s]*", string.Empty, RegexOptions.Multiline),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );
                }

                else if (e.Message.Text.Contains("/entry"))
                {
                    string entryId = e.Message.Text.Substring(6);
                    Parsing parsed = new Parsing();
                    try
                    {
                        HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com/entry/" + entryId.Trim());

                        string xpathTitle = @"//*[@id='topic']/h1";
                        var returnedS_hListTitle = parsed.TagData(hookedDocument, xpathTitle);

                        string xpathContent = @"//*[@id='entry-item-list']/li";
                        var returnedS_hListContent = parsed.TagData(hookedDocument, xpathContent);

                        StringBuilder sbuilder = new StringBuilder();


                        foreach (var item in returnedS_hListTitle)
                        {
                            foreach (var Inner_a in item.SelectNodes("a"))
                            {
                                if (!string.IsNullOrEmpty(Inner_a.ToString()))
                                {
                                    HtmlAttribute href_att = Inner_a.Attributes["href"];
                                    sbuilder.AppendLine("[" + Inner_a.InnerText + "]" + "(" + "https://eksisozluk.com" + href_att.Value + ")");
                                }
                            }
                        }

                        foreach (var item in returnedS_hListContent)
                        {
                            foreach (var Inner_div in item.SelectNodes("div"))
                            {
                                if (!string.IsNullOrEmpty(Inner_div.ToString()))
                                {
                                    sbuilder.AppendLine(Inner_div.InnerText);
                                }
                            }
                        }
                        string Data = sbuilder.ToString();
                        await Program.botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: Regex.Replace(Data, @"^\s+$[\r\s]*", string.Empty, RegexOptions.Multiline),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                        );
                    }
                    catch
                    {
                        await Program.botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: "Üzgünüm, geçerli bir entry numarası girmedin!\nEntry numarasını kontrol edip tekrar denersen sana yardımcı olmaya çalışacağım.\nUnutma entry numaraları sadece rakamlardan oluşur, harf ve karakter içermez."
                        );
                    }
                }

                else
                {
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Üzgünüm, geçerli bir komut girmedin!\nGeçerli komutların listesini sana gösterebilirim /yardim"
                    );
                }
            }
        }
    }
}