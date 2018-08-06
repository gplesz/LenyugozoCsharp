using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace bot.server.Controllers
{
    [Serializable]
    internal class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(OnMessageRecieved);

            return Task.CompletedTask;
        }

        private Task OnMessageRecieved(IDialogContext context, IAwaitable<object> result)
        {
            var message = (Activity)result.GetAwaiter().GetResult();

            context.PostAsync(message.CreateReply($"Ezt üzented:{ message.Text}"));

            PromptDialog.Choice(context, OnChoiceRecieved,
                new[] { "Emlekezteto", "Kepfelismero", "Arcazonositas", "Idojaras" },
                "Menupontok:");

            return Task.CompletedTask;
        }

        private Task OnChoiceRecieved(IDialogContext context, IAwaitable<string> result)
        {
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

            return Task.CompletedTask;
        }
    }
}