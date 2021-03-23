using System;
using System.Collections.Generic;

namespace ChatBot.Services
{
    public class BotService
    {
        private IDictionary<string, Func<string, string>> commands;

        public BotService()
        {
            this.commands = new Dictionary<string, Func<string, string>>
            {
                { "!timeat", this.TimeAt },
                { "!timepopularity", this.TimePopilarity }
            };
        }

        public void Process(string input)
        {
            //Add exception handlering
            var data = input.Split(':');
            var user = data[0];
            var commandInfo = data[1].Trim().Split(' ');

            var command = commandInfo[0];
            var parameter = commandInfo[1];

            if (this.commands.TryGetValue(command, out Func<string, string> toExecute))
            {
                toExecute.Invoke(parameter);
            }
        }

        private string TimeAt(string timezone)
        {
            var api = new TimeAtApi();

            api.GetTimeBy(timezone);

            return "time at";
        }

        /// <summary>
        /// Timezone / Preffix
        /// </summary>
        /// <param name="parameter"></param>
        private string TimePopilarity(string parameter)
        {
            return "time pop";
        }
    }
}