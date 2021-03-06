using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Serilog;

namespace bot.server.Controllers
{
    [Serializable]
    internal class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            Log.Logger.Debug("RootDialog.StartAsync");
            context.Wait(OnMessageRecieved);

            return Task.CompletedTask;
        }

        private Task OnMessageRecieved(IDialogContext context, IAwaitable<object> result)
        {
            Log.Logger.Debug("RootDialog.OnMessageRecieved started");

            var message = (Activity)result.GetAwaiter().GetResult();

            context.PostAsync(message.CreateReply($"Ezt üzented:{ message.Text}"));

            PromptDialog.Choice(context, OnChoiceRecieved,
                new[] { "Emlekezteto", "Kepfelismero", "Arcazonositas", "Idojaras" },
                "Menupontok:");

            return Task.CompletedTask;
        }

        private Task OnChoiceRecieved(IDialogContext context, IAwaitable<string> result)
        {
            Log.Logger.Debug("RootDialog.OnChoiceRecieved started");

            var choice = result.GetAwaiter().GetResult();

            switch (choice)
            {
                case "Emlekezteto":
                    context.Call(new ReminderDialog(), OnDialogDone);
                    break;
                case "Kepfelismero":
                    context.Call(new ImageAnalyzerDialog(), OnDialogDone);
                    break;
                default:
                    context.PostAsync($"Hmm, erre meg nem vagyok felkeszitve, ezt valasztottad: {choice}")
                        .GetAwaiter()
                        .GetResult();
                    break;
            }

            return Task.CompletedTask;
        }

        private Task OnDialogDone(IDialogContext context, IAwaitable<object> result)
        {
            Log.Logger.Debug("RootDialog.OnDialogDone started");

            return Task.CompletedTask;
        }
    }
}