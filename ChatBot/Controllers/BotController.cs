using System.Web.Http;
using ChatBot.Models;
using ChatBot.Services;

namespace ChatBot.Controllers
{
    public class BotController : ApiController
    {
        public string Post(InputDTO input)
        {
            var bot = new BotService();

            return bot.Process(input.Input);
        }
    }
}