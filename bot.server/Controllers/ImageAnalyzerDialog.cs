using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace bot.server.Controllers
{
    [Serializable]
    internal class ImageAnalyzerDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Tolts fel egy kepet! ");

            context.Wait(OnImageUploaded);
        }

        private Task OnImageUploaded(IDialogContext context, IAwaitable<object> result)
        {
            var message = (Activity)result.GetAwaiter().GetResult();

            if (ImageAnalyzer.IsImageUpladed(message))
            {
                //Ha elhagyjuk az alkalmazás határait, érdemes 
                //vigyázni, nehogy válasz nélkül hagyjunk egy kérést
                try
                {
                    var caption = ImageAnalyzer.GetCaption(message, "6a4a771ce05c475c8fac21308d279136", "https://northeurope.api.cognitive.microsoft.com/vision/v1.0");
                    context.PostAsync(message.CreateReply(caption)).GetAwaiter().GetResult();
                }
                catch (System.Exception)
                {
                    context.PostAsync(message.CreateReply("Hiba történt, értesültünk róla, az elhárítás folyamatban")).GetAwaiter().GetResult();
                }
                context.Done(true);
            }
            else
            {
                context.PostAsync("Nem jo filet toltottel fel, probald ujra!")
                    .GetAwaiter()
                    .GetResult();

                context.Wait(OnImageUploaded);
            }

            return Task.CompletedTask;
        }
    }
}