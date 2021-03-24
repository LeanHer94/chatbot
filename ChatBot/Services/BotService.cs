﻿using ChatBot.Models;
using System;
using System.Collections.Generic;

namespace ChatBot.Services
{
    public class BotService
    {
        private BotRepository botRepository = new BotRepository();
        private TimeAtApi timeAtApi = new TimeAtApi();

        private IDictionary<string, Func<string, string>> commands;

        public BotService()
        {
            this.commands = new Dictionary<string, Func<string, string>>
            {
                { "!timeat", this.TimeAt },
                { "!timepopularity", this.TimePopularity }
            };
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

                return ErrorMessages.WRONG_COMMAND;
            } 
            catch (IndexOutOfRangeException ex)
            {
                return ErrorMessages.WRONG_INPUT;
            }
        }

        private string TimeAt(string timezone)
        {
            this.PopulateZones();

            this.botRepository.InsertRequest(timezone);

            var time = this.timeAtApi.GetTimeBy(timezone);

            var date = DateTime.Parse((string)time.datetime);

            return date.ToString("d MMM yyy HH:mm");
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