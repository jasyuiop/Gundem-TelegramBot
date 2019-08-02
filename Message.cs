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
                            if (!string.IsNullOrEmpty(Inner_li.ToString()))
                            {

                                sbuilder.AppendLine(Inner_li.InnerText);
                            }
                        }
                    }
                    // gelen veride boş satırlar olabiliyor bu yüzden onları işin içinden temizliyoruz
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: Regex.Replace(Data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline) + "Linkleri görmek için tıklayınız:\n/eksigundemlink"
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
                            HtmlAttribute href_att = Inner_a.Attributes["href"];
                            sbuilder.AppendLine(sequence + "-" + "https://eksisozluk.com" + href_att.Value);
                            sequence++;
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