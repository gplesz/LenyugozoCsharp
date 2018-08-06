using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace bot.server.Controllers
{
    [Serializable]
    internal class ReminderDialog : IDialog<object>
    {
        const string ReminderTextKey = "Text";
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("Mire szeretnel emlekezni?").GetAwaiter().GetResult();

            context.Wait(OnMessageReceived);

            return Task.CompletedTask;
        }

        private Task OnMessageReceived(IDialogContext context, IAwaitable<object> result)
        {
            var message = (Activity)result.GetAwaiter().GetResult();

            context.ConversationData.SetValue(ReminderTextKey, message.Text);

            PromptDialog.Number(context, OnSecondReceived,
             "Hany masodperc mulva szeretned kapni az ertesitot?");

            return Task.CompletedTask;
        }

        private Task OnSecondReceived(IDialogContext context, IAwaitable<long> result)
        {
            var seconds = result.GetAwaiter().GetResult();
            var text = context.ConversationData.GetValue<string>(ReminderTextKey);

            context.PostAsync($"Ok, {seconds} masodperc mulva kuldunk ertesitot ezzel a szoveggel: {text} ")
                .GetAwaiter()
                .GetResult();

            var reminder = new Reminder(seconds, text, context);
            Reminders.Add(reminder);

            context.Done(true);
            return Task.CompletedTask;
        }
    }
}