using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using HtmlAgilityPack;

namespace telegram_bot
{
    class Program
    {
        static ITelegramBotClient botClient;

        static void Main()
        {
            botClient = new TelegramBotClient("***REMOVED***");

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Id = {me.Id} | Name = {me.FirstName}. \n"
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);

        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null) // gelen mesaj null değilse
            {
                Console.WriteLine($"Alınan Mesajın ChatId'si = {e.Message.Chat.Id}.");

                if (e.Message.Text == "/help")
                    await botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var 
                    text: "Merhaba, bu bot belirli platformlardaki gündemi, olayları ve trend başlıkları gösterir.\nKullanabileceğiniz komutlar :\n/reddit\n/ekşi"
                    );

                else if (e.Message.Text == "/eksi")
                {
                    Scraping.Parsing parsed = new Scraping.Parsing();
                    HtmlAgilityPack.HtmlDocument hookedDocument = parsed.hookSite("https://eksisozluk.com");
                    string xpath = @"//*[@id='partial-index']/ul";
                    string returnedData = parsed.tagData(hookedDocument, xpath);

                    await botClient.SendTextMessageAsync( // mesajı göndermeyi bekliyoruz.
                    chatId: e.Message.Chat, // her mesaj atan kişiyle oluşan bir unique Id var
                    text: returnedData
                    );

                }
            }
        }
    }
}