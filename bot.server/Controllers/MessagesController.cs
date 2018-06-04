using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;

namespace bot.server.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        [HttpPost]
        public OkResult Post([FromBody]Activity activity)
        {

            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var userMessage = activity.Text;
            var reply = activity.CreateReply($"Visszhang: {userMessage}");
            connector.Conversations.ReplyToActivity(reply);

            return Ok();
        }
        
    }
}