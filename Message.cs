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

                if (e.Message.Text == "/help")
                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, bu bot belirli platformlardaki gündemi, olayları ve trend başlıkları gösterir.\nKullanabileceğiniz komutlar :\n/reddit\n/ekşi"
                    );

                else if (e.Message.Text == "/eksigundem")
                {
                    Parsing parsed = new Parsing();
                    HtmlDocument hookedDocument = parsed.HookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    var returnedS_hList = parsed.TagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();

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
                                    sbuilder.AppendLine(Inner_li.InnerText);
                                }
                            }
                        }
                    }
                    // gelen veride boş satırlar olabiliyor bu yüzden onları işin içinden temizliyoruz
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: Regex.Replace(Data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline) + "\nLinkleri görmek için tıklayınız:\n/eksigundemlink"
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
            }
        }
    }
}