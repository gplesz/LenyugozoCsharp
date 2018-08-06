using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Bot.Builder.Dialogs;
using RestSharp;

namespace bot.server.Controllers
{
    internal class Reminder
    {
        private long seconds;
        private string text;
        private IDialogContext context;

        public Reminder(long seconds, string text, IDialogContext context)
        {
            this.seconds = seconds;
            this.text = text;
            this.context = context;

            var timer = new Timer((int)seconds * 1000);
            timer.Elapsed += OnTimerElapsed;
            timer.AutoReset = false;
            timer.Start();
            //rpi led be
            var client = new RestClient("http://10.168.1.148:5000/api");
            var request = new RestRequest("Led", Method.GET);
            request.AddParameter("id", 1);
            request.AddParameter("isOn", true);
            var responseFel = client.Execute<List<string>>(request);            
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            context.PostAsync($"Ertesites: {text}").GetAwaiter().GetResult();
            //todo: a listából törölni a lejárt értesítést
            //Reminder.Remove()
            //rpi led ki
            var client = new RestClient("http://10.168.1.148:5000/api");
            var request = new RestRequest("Led", Method.GET);
            request.AddParameter("id", 1);
            request.AddParameter("isOn", false);
            var responseFel = client.Execute<List<string>>(request);            

        }
    }
}