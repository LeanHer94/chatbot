using System.Web.Http;
using ChatBot.Services;

namespace ChatBot.Controllers
{
    public class BotController : ApiController
    {
        public string Post(string input)
        {
            var bot = new BotService();

            bot.Process(input);

            return "Hello world";
        }
    }
}