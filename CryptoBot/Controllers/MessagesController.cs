﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using System.Web.Http;

namespace CryptoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            return await Conversation.SendAsync(message, () => new CryptoDialog());
        }

        // ------  to send a message 
        // ConnectorClient botConnector = new BotConnector();
        // ... use message.CreateReplyMessage() to create a message, or
        // ... create a new message and set From, To, Text 
        // await botConnector.Messages.SendMessageAsync(message);
    }
}