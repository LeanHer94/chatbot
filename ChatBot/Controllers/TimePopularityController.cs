using ChatBot.Services;
using System.Web.Http;

namespace ChatBot.Controllers
{
    public class TimePopularityController : ApiController
    {
        public string Post(string input)
        {
            var bot = new BotService();

            return bot.TimePopularity(input);
        }
    }
}