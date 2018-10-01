using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Linq;
using Microsoft.Extensions.Logging;

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
                logger.LogDebug("MessagesController.Post started");
                if (activity.Type == ActivityTypes.ConversationUpdate)
                {
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels

                    var msAppIdKey = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value;
                    var msAppPwd = Configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value;

                    // Note: Add introduction here:
                    IConversationUpdateActivity update = activity;
                    var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials(msAppIdKey, msAppPwd));
                    if (update.MembersAdded != null && update.MembersAdded.Any())
                    {
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != activity.Recipient.Id)
                            {
                                var reply = activity.CreateReply();
                                reply.Text = $"Welcome {newMember.Name}!";
                                client.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }
                }

                if (activity.Type == ActivityTypes.Message)
                {
                    Conversation.SendAsync(activity, () => new RootDialog());
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, "Hiba történt");
            }
            finally
            {
                logger.LogDebug("MessagesController.Post ended");
            }
            return Ok();
        }

    }
}