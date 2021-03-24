using ChatBot.ExtensionMethods;
using ChatBot.Models;
using System;
using System.Collections.Generic;

namespace ChatBot.Services
{
    public class BotService
    {
        private BotRepository botRepository;
        private TimeAtApi timeAtApi;

        private IDictionary<string, Func<string, string>> commands;

        public BotService()
        {
            this.commands = new Dictionary<string, Func<string, string>>
            {
                { "!timeat", this.TimeAt },
                { "!timepopularity", this.TimePopularity }
            };

            this.botRepository = new BotRepository();
            this.timeAtApi = new TimeAtApi(this.botRepository);
        }

        public string Process(string input)
        {
            try
            {
                var data = input.Split(':');
                var user = data[0];
                var commandInfo = data[1].Trim().Split(' ');

                var command = commandInfo[0];
                var parameter = commandInfo[1];

                if (this.commands.TryGetValue(command, out Func<string, string> toExecute))
                {
                    return toExecute.Invoke(parameter);
                }

                this.botRepository.InsertLog(ErrorMessages.WRONG_COMMAND, null);
            }
            catch (IndexOutOfRangeException ex)
            {
                this.botRepository.InsertLog(ErrorMessages.WRONG_INPUT, ex);
            }

            return null;
        }

        private string TimeAt(string timezone)
        {
            this.PopulateZones();

            if (this.botRepository.IsKnownZone(timezone.GetLastRegion()))
            {
                this.botRepository.InsertRequest(timezone);

                var validPath = this.botRepository.GetValidRegionPath(timezone);

                var datetime = this.timeAtApi.GetTimeBy(validPath);

                var date = DateTime.Parse(datetime);

                return date.ToString("d MMM yyy HH:mm");
            }

            return ErrorMessages.UNKNOWN_TIMEZONE;
        }

        /// <summary>
        /// Timezone / Preffix
        /// </summary>
        /// <param name="parameter"></param>
        private string TimePopularity(string parameter)
        {
            this.PopulateZones();

            return this.botRepository.GetCount(parameter).ToString();
        }

        private void PopulateZones()
        {
            if (!this.botRepository.IsZonesTablePopulated())
            {
                var timezones = this.timeAtApi.GetAll();

                this.botRepository.TryInsert(timezones);
            }
        }
    }
}