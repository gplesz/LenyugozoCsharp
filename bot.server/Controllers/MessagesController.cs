using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace bot.server.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly ILogger<MessagesController> logger;

        public IConfiguration Configuration { get; }

        public MessagesController(IConfiguration configuration, ILogger<MessagesController> logger)
        {
            this.Configuration = configuration 
                ?? throw new ArgumentNullException(nameof(configuration));

            this.logger = logger 
                ?? throw new ArgumentNullException(nameof(logger));
        }


        [Authorize(Roles="Bot")]
        [HttpPost]
        public OkResult Post([FromBody]Activity activity)
        {
            try
            {
                logger.LogDebug("MessagesController.Post started: {ActivityType}, activity: {@Activity}", activity.Type, activity);

                var msAppIdKey = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
                var msAppPwd = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value;

                var connector = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials(msAppIdKey, msAppPwd));

                if (activity.Type == ActivityTypes.ConversationUpdate)
                {
                    var conversationUpdate = activity as IConversationUpdateActivity;

                    if (conversationUpdate != null
                        && conversationUpdate.MembersAdded != null
                        && conversationUpdate.MembersAdded.Any())
                    {

                        foreach (var member in conversationUpdate.MembersAdded)
                        {
                            Activity reply = activity.CreateReply($"Hello: {member.Name}");
                            connector.Conversations.ReplyToActivity(reply);
                        }
                    }
                }

                if (activity.Type == ActivityTypes.Message)
                {
                    Activity reply = activity.CreateReply($"Echo: {activity.Text}");
                    connector.Conversations.ReplyToActivity(reply);

                    Conversation.SendAsync(activity, () => new RootDialog());
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MessagesController.Post failed");
            }
            finally
            {
                logger.LogDebug("MessagesController.Post ended");
            }

            return Ok();
        }

    }
}