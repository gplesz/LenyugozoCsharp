using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace bot.server.Controllers
{
    internal class ImageAnalyzer
    {
        internal static bool IsImageUpladed(Activity message)
        {
            return (message.Attachments != null
                    && message.Attachments.Any(x => x.ContentType.Contains("image")))
                    || Uri.IsWellFormedUriString(message.Text, UriKind.Absolute);
        }

        internal static string GetCaption(Activity message, string key, string url)
        {
            var visionAPiClient = new VisionServiceClient(key, url);

            var image = message.Attachments?
                                .FirstOrDefault(x => x.ContentType.Contains("image"));
            if (image != null)
            {
                //emulator
                using (var stream = GetImageStream(image))
                {
                    var result = visionAPiClient.AnalyzeImageAsync(stream,
                     new string[] { VisualFeature.Description.ToString() })
                     .GetAwaiter()
                     .GetResult();

                    return result.Description.Captions.FirstOrDefault().Text;
                }
            }
            //Facebook messenger
            var messengerResult = visionAPiClient.AnalyzeImageAsync(message.Text,
             new string[] { VisualFeature.Description.ToString() }).GetAwaiter().GetResult();

            return messengerResult.Description.Captions.FirstOrDefault().Text;
        }

        private static Stream GetImageStream(Attachment image)
        {
            using (var client = new HttpClient())
            {
                var uri = new Uri(image.ContentUrl);

                return client.GetStreamAsync(uri).GetAwaiter().GetResult();
            }
        }
    }
}