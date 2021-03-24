using ChatBot.Models;
using ChatBot.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace ChatBot.Controllers
{
    public class TimePopularityController : ApiController
    {
        public ResponseMessageResult Post(InputDTO input)
        {
            var bot = new BotService();

            var content = bot.TimePopularity(input?.Timezone);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                RequestMessage = Request,
                Content = new StringContent(content)
            };

            return ResponseMessage(httpResponseMessage);
        }
    }
}