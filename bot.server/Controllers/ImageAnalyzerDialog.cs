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
                var caption = ImageAnalyzer.GetCaption(message, "0902cbc4bf524361a961e16495e36fac", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");

                context.PostAsync(message.CreateReply(caption)).GetAwaiter().GetResult();

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