using System;
using System.Threading;
using Telegram.Bot;
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
                    await Gundem_TelegramBot.Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, bu bot belirli platformlardaki gündemi, olayları ve trend başlıkları gösterir.\nKullanabileceğiniz komutlar :\n/reddit\n/ekşi"
                    );

                else if (e.Message.Text == "/eksi")
                {
                    Parsing parsed = new Parsing();
                    HtmlAgilityPack.HtmlDocument hookedDocument = parsed.hookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    var returnedS_hList = parsed.tagData(hookedDocument, xpath);

                    StringBuilder sbuilder = new StringBuilder();

                    foreach (var item in returnedS_hList)
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
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: Regex.Replace(Data, @"^\s+$[\r]*", string.Empty, RegexOptions.Multiline)
                    );

                }
            }
        }
    }
}