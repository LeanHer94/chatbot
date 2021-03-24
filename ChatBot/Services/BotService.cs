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