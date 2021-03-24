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

        public string TimeAt(string timezone)
        {
            this.PopulateZones();

            if (timezone == null)
            {
                return null;
            }

            if (this.botRepository.IsKnownZone(timezone.GetLastRegion()))
            {
                this.botRepository.InsertRequest(timezone);

                var validPath = this.botRepository.GetValidRegionPath(timezone);

                var result = this.timeAtApi.GetTimeBy(validPath);

                if (DateTime.TryParse(result, out DateTime date))
                {
                    return date.ToString("d MMM yyy HH:mm");
                }

                return result;
            }

            return ErrorMessages.UNKNOWN_TIMEZONE;
        }

        /// <summary>
        /// Timezone / Preffix
        /// </summary>
        /// <param name="parameter"></param>
        public string TimePopularity(string parameter)
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