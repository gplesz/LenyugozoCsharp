using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Dialogs;
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
                Conversation.SendAsync(activity, () => new RootDialog());
            }
            return Ok();
        }

    }
}