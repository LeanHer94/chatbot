using System.Web.Http;
using ChatBot.Models;
using ChatBot.Services;

namespace ChatBot.Controllers
{
    public class TimeAtController : ApiController
    {
        public string Post(InputDTO input)
        {
            var bot = new BotService();

            return bot.TimeAt(input.Input);
        }
    }
}