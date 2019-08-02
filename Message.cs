using System;
using Telegram.Bot.Args;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;

namespace Gundem_TelegramBot
{
    public class Message
    {
        public static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null) // gelen mesaj null değilse
            {
                Console.WriteLine($"Alınan Mesajın ChatId'si = {e.Message.Chat.Id}.");

                if (e.Message.Text == "/start")
                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, Ekşi sözlük gündem başlıklarını, gündem başlıklarının linklerini, bana verilen entry numarasından entry içeriğini sana gösterebilirim.\nKullanabileceğin komutlar işte burada\n/help\n/eksigundem\n/eksigundemlink\n/eksientry\n\nGeliştiriciye destek olmak için;\nRipple XRP Adress =\nrDrwceWscNExnTmgxz51cRcrs24dhVEz3V\nXRP Tag = 0"
                    );


                else if (e.Message.Text == "/help")
                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, Ekşi sözlük gündem başlıklarını, gündem başlıklarının linklerini, bana verilen entry numarasından entry içeriğini sana gösterebilirim.\nKullanabileceğin komutlar işte burada\n/eksigundem\n/eksigundemlink\n/eksientry\n\nGeliştiriciye destek olmak için;\nRipple XRP Adress =\nrDrwceWscNExnTmgxz51cRcrs24dhVEz3V\nXRP Tag = 0"
                    );

                else if (e.Message.Text == "/eksigundem")
                {
                    Parsing parsed = new Parsing();
                    HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    var returnedS_hList = parsed.TagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();

                    int sequence = 1;

                    foreach (var item in returnedS_hList)
                    {
                        foreach (var Inner_li in item.SelectNodes("li//a//text()"))
                        {
                            /* ekşi'de reklam hizmetinden dolayı böyle bir satır düşüyor olabilir. onu devredışı 
                            bırakmak için o satırı stringimin içerisine eklemiyorum.*/
                            // if (!Inner_li.InnerText.ToString().Contains("NativeAdPub.push"))

                            // IsNullOrEmpty ile'de o reklam hizmetinden düşen satırı engelleyebiliyoruz
                            // ve böylesi daha sağlıklı.
                            if (!string.IsNullOrEmpty(Inner_li.ToString()))
                            {
                                /* text() node'unu çektiğim zaman small tag'ındaki veriler ile birlikte geliyor.
                                small tag'ından gelen verilerın last indexleri -1 olarak düşüyor.
                                bunu önlemek için bir anahtar kodu eklemem gerekiyor.*/
                                if (Inner_li.InnerText.LastIndexOf(' ') != -1)
                                {
                                    sbuilder.AppendLine(sequence + "- " + Inner_li.InnerText);
                                    sequence++;
                                }
                            }
                        }
                    }
                    // gelen veride boş satırlar olabiliyor bu yüzden onları işin içinden temizliyoruz
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: Regex.Replace(Data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline) + "\nBu başlıkların linklerini sana gösterebilirim /eksigundemlink"
                    );
                }

                else if (e.Message.Text == "/eksigundemlink")
                {
                    Parsing parsed = new Parsing();
                    HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    var returnedS_hList = parsed.TagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();
                    int sequence = 1;
                    foreach (var item in returnedS_hList)
                    {
                        foreach (var Inner_a in item.SelectNodes("li//a"))
                        {
                            // Burada IsNullOrEmpty gereksiz gibi görünebilir fakat 
                            // her ihtimale karşı verilerin düzgün bir şekilde aktarılması için
                            // bu kod çok önemli!
                            if (!string.IsNullOrEmpty(Inner_a.ToString()))
                            {
                                HtmlAttribute href_att = Inner_a.Attributes["href"];
                                sbuilder.AppendLine(sequence + "-" + "https://eksisozluk.com" + href_att.Value);
                                sequence++;
                            }
                        }
                    }
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: Regex.Replace(Data, @"^\s+$[\r]*", string.Empty, RegexOptions.Multiline)
                    );
                }
                else if (e.Message.Text.Contains("/eksientry"))
                {
                    string entryId = e.Message.Text.Substring(10);
                    Parsing parsed = new Parsing();
                    try
                    {
                        HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com/entry/" + entryId.Trim());
                        string xpath = @"//*[@id='entry-item-list']/li";
                        var returnedS_hList = parsed.TagData(hookedDocument, xpath);
                        StringBuilder sbuilder = new StringBuilder();
                        foreach (var item in returnedS_hList)
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
                        text: Regex.Replace(Data, @"^\s+$[\r]*", string.Empty, RegexOptions.Multiline)
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
                    text: "Üzgünüm, geçerli bir komut girmedin!\nGeçerli komutların listesini sana gösterebilirim /help"
                    );
                }
            }
        }
    }
}