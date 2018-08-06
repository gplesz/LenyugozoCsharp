using System;
using System.Timers;
using Microsoft.Bot.Builder.Dialogs;

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
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            context.PostAsync($"Ertesites: {text}").GetAwaiter().GetResult();
            //todo: a listából törölni a lejárt értesítést
        }
    }
}