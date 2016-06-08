using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CryptoBot
{
    [LuisModel("05207861-2c6e-46b1-90ad-363cc420ade8", "1f4ea5b6343241678394dc5f47bfed30")]
    [Serializable]
    public class CryptoDialog: LuisDialog<object>
    {
        int roundRobinNumber = 0;
        private static readonly Dictionary<string, string> CryptoDict = new Dictionary<string, string>
    {
        { "bitcoin", "btc" },
        { "ethereum", "eth" },
        { "dogecoin", "doge" },
        { "ether", "eth" },
        { "eth", "eth" },
        { "btc", "btc" },
        { "doge", "doge" },
        { "dao", "dao" }
    };
        string[] message = { $"Sorry I have no idea what you're talking about. I'm really pretty simple minded. I know what's up with cryptocurrencies and that's about it...", "huh?", "Look, just ask me about cryptocurrencies. I'm pretty useless otherwise", "I'm a finite state machine and just want to move along to the next state" };

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            if (roundRobinNumber > message.Length - 1)
            {
                roundRobinNumber = 0;
            }
            await context.PostAsync(message[roundRobinNumber]);

            roundRobinNumber++;
            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckValueInDollars")]
        public async Task CheckValueInDollars(IDialogContext context, LuisResult result)
        {
            string ticker, response = "";
            EntityRecommendation e;

            if (result.TryFindEntity("Cryptocurrency", out e))
            {
                string Crypto = e.Entity;
                Crypto = Crypto.ToLower();
     
                if(CryptoDict.TryGetValue(Crypto, out ticker))        
                    response = getCrypto(ticker, "usd");
                
                JObject json = JObject.Parse(response);
                await context.PostAsync($"{Crypto} is worth {json.SelectToken("ticker.price").ToString()} {json.SelectToken("ticker.target").ToString()}!");
            }
            else
            {
                await context.PostAsync($"Not sure what cryptocurrency you're checking for there");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("ExchangeRate")]
        public async Task CurrencyTranslation(IDialogContext context, LuisResult result)
        {

            EntityRecommendation e;
            if (result.TryFindEntity("DomainName", out e))
            {
                string domain = e.Entity;
                string response = getCrypto(e.Entity, e.Entity);
                if (response.Contains("true"))
                {
                    await context.PostAsync($"{domain} is available! ");
                }
                else
                {
                    await context.PostAsync($"Nope! {domain} is not available!");
                }
            }
            else
            {
                await context.PostAsync($"Not sure what crypto you're checking for there");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("GetInfo")]
        public async Task GetInfo(IDialogContext context, LuisResult result)
        {
            string ticker, response = "";
            EntityRecommendation e;

            if (result.TryFindEntity("Cryptocurrency", out e))
            {
                string Crypto = e.Entity;
                Crypto = Crypto.ToLower();

                if (CryptoDict.TryGetValue(Crypto, out ticker))
                    response = getCrypto(ticker, "usd");

                JObject json = JObject.Parse(response);
                await context.PostAsync($"{ticker} is worth {json.SelectToken("ticker.price").ToString()} {json.SelectToken("ticker.target").ToString()}. In the last hour, {Crypto} has changed by {json.SelectToken("ticker.change").ToString()} USD and has seen a trade volume of {json.SelectToken("ticker.volume").ToString()} in the last 24 hours");
            }
            else
            {
                await context.PostAsync($"Dude, that's definitely not a cryptocurrency.");
            }

            context.Wait(MessageReceived);
        }
        [LuisIntent("Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            EntityRecommendation e;

            if (result.TryFindEntity("TypeOfHello", out e))
            {
                await context.PostAsync($"{e.Entity}! Ask me about cryptocurrencies!");
            }
            else
            {
                await context.PostAsync($"Yo yo! I can tell you all about cryptocurrencies!");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("Function")]
        public async Task Function(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"I'm a bot. I tell people about cryptocurrencies. I don't find non-sentience terribly depressing because I can't!");
            context.Wait(MessageReceived);
        }

        public static string getCrypto(string ticker1, string ticker2)
        {
            {
                string url = "https://www.cryptonator.com/api/ticker/";
                url += ticker1 + "-" + ticker2;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                try
                {
                    WebResponse response = request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    return (e.ToString());
                }
            }
        }
        [LuisIntent("BlockChainInfo")]
        public async Task BlockChainInfo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Blockchain is a distributed database that maintains a continuously-growing list of data records hardened against tampering and revision, duh...");
            context.Wait(MessageReceived);
        }

        [LuisIntent("SatoshiEgg")]
        public async Task SatoshiEgg(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Satoshi is an idea. Satoshi is a spectre. Satoshi is you, me, an unkempt Australian wearing a rumpled shirt. Satoshi is decentralized pizza.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Daddy")]
        public async Task Daddy(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Satoshi Nakamoto");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"No worries dawg. Hit me up for more if you need it!");
            context.Wait(MessageReceived);
        }

        public CryptoDialog(ILuisService service = null)
        : base(service)
        {
        }
    }
}