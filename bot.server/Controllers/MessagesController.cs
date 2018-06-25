using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace bot.server.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        public IConfiguration Configuration { get; }

        public MessagesController(IConfiguration configuration)
        {
            this.Configuration = configuration 
                ?? throw new ArgumentNullException(nameof(configuration));
        }


        [Authorize(Roles="Bot")]
        [HttpPost]
        public OkResult Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var appCredentials = new MicrosoftAppCredentials(Configuration);
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl),appCredentials);
                var userMessage = activity.Text;
                Activity reply = null;

                var client = new RestClient("http://192.168.0.102:5000/api");
                var request = new RestRequest("Led", Method.GET);
                request.AddParameter("id", 1);

                //todo: feldolgozni a kérést: felkapcsolás, vagy lekapcsolás?
                //"led fel": felkapcsoljuk a ledet
                //"led le": lekapcsoljuk a ledet
                switch (userMessage.ToLower())
                {
                    case "led fel":
                        request.AddParameter("isOn", true);
                        var responseFel = client.Execute<List<string>>(request);
                        reply = activity.CreateReply($"led fel: {string.Join(",", responseFel.Data)}");
                        break;
                    case "led le":
                        request.AddParameter("isOn", false);
                        var responseLe = client.Execute<List<string>>(request);
                        reply = activity.CreateReply($"led fel: {string.Join(",", responseLe.Data)}");
                        break;
                    default:
                        reply = activity.CreateReply($"Visszhang: {userMessage}");
                        break;
                }

                //todo az rpi választ visszaküldeni
                //ehhez értelmezni kell az rpi választ és betenni a válaszba

                connector.Conversations.ReplyToActivity(reply);
            }
            return Ok();
        }

    }
}