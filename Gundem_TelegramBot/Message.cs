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
                    text: "Merhaba, Ekşi sözlük gündemini, bana verilen entry numarasından entry'i sana gösterebilirim\nKullanabileceğin komutlar işte burada\n/yardim\n/gundem\n/entry\n\n[Ben artık açık kaynak bir projeyim, tıkla ve github üzerinde bana gözat.](https://github.com/jasyuiop/Gundem-TelegramBot)\n\nGeliştiriciye destek olmak için;\nRipple XRP Adress =\nrDrwceWscNExnTmgxz51cRcrs24dhVEz3V\nXRP Tag = 0",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                    );


                else if (e.Message.Text == "/yardim")
                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, Ekşi sözlük gündemini, bana verilen entry numarasından entry'i sana gösterebilirim\nKullanabileceğin komutlar işte burada\n/gundem\n/entry\n\n[Ben artık açık kaynak bir projeyim, tıkla ve github üzerinde bana gözat.](https://github.com/jasyuiop/Gundem-TelegramBot)\n\nGeliştiriciye destek olmak için;\nRipple XRP Adress =\nrDrwceWscNExnTmgxz51cRcrs24dhVEz3V\nXRP Tag = 0",
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
                                /* text() node'unu çektiğim zaman small tag'ındaki veriler ile birlikte geliyor.
                                small tag'ından gelen verilerın last indexleri -1 olarak düşüyor.
                                bunu önlemek için bir anahtar kodu eklemem gerekiyor.*/
                                if (Inner_li.InnerText.LastIndexOf(' ') != -1)
                                {
                                    HtmlAttribute href_att = Inner_li.Attributes["href"];
                                    HtmlAttribute text_att = Inner_li.Attributes["text()"];
                                    sbuilder.AppendLine(sequence + "- " + "[" + Inner_li.InnerText + "]" + "(" + "https://eksisozluk.com" + href_att.Value + ")");
                                    sequence++;
                                }
                            }
                        }
                    }
                    // gelen veride boş satırlar olabiliyor bu yüzden onları işin içinden temizliyoruz
                    string Data = sbuilder.ToString();

                    await Program.botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: Regex.Replace(Data, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline),
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

                else if (e.Message.Text == "/eksigundemlink")
                {
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Linkler başlık içerisine gömüldü, artık /gundem yazarak başlıklara tıklayınca direkt link'e gidebileceksiniz :)"
                    );
                }

                else if (e.Message.Text == "/eksigundem")
                {
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Linkler ve başlık /gundem 'de birleşti, artık linkleri ve başlıkları tek bir komut'ta görebileceksiniz :)"
                    );
                }
                else if (e.Message.Text == "/help")
                {
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Yardım almak için /yardim komutunu kullanmanı tavsiye ediyorum, geliştiricim artık böylesini uygun görüyor."
                    );
                }
                else if (e.Message.Text == "/eksientry")
                {
                    await Program.botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Entry'leri görmek için /entry komutunu kullanmanı tavsiye ediyorum, artık başlık ismini ve başlık ismine gömülü entry link'ini de gösterebiliyorum, geliştiricim artık böylesini uygun görüyor."
                    );
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