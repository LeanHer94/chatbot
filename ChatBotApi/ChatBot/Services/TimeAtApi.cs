using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ChatBot.Services
{
    public class TimeAtApi
    {
        private const string TIME_API = "TIME_API";

        private string BASE_URL { get; }

        private BotRepository botRepository;

        public TimeAtApi(BotRepository botRepository)
        {
            this.BASE_URL = ConfigurationManager.AppSettings[TIME_API];
            this.botRepository = botRepository;
        }

        public string GetTimeBy(string timezone)
        {
            HttpClient client = this.GetHttpClient();
            client.BaseAddress = new Uri(this.BASE_URL);

            // List data response.
            HttpResponseMessage response = client.GetAsync(timezone).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsAsync<WebApiResultDTO>().Result;

                if (this.IsISO8601Compliant(result.datetime))
                {
                    // So Datetime.Parse don't use it to convert to local datetime.
                    var offset = 6;
                    return result.datetime.Remove(result.datetime.Length - offset, offset);
                }

                this.botRepository.InsertLog(ErrorMessages.API_ISO_COMPLIANT, null);
                return ErrorMessages.API_ISO_COMPLIANT;
            } 
            else
            {
                this.botRepository.InsertLog(ErrorMessages.API_TIMEOUT, null);
                return ErrorMessages.API_TIMEOUT;
            }
        }

        public IEnumerable<string> GetAll()
        {
            HttpClient client = this.GetHttpClient();

            // List data response.
            HttpResponseMessage response = client.GetAsync(this.BASE_URL).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<IEnumerable<string>>().Result;
            }

            return Enumerable.Empty<string>();
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        } 

        private bool IsISO8601Compliant(string datetime)
        {
            try
            {
                return DateTime.TryParseExact(datetime, "yyyy-MM-ddTHH:mm:ss.ffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            }
            catch(ArgumentException ex)
            {
                this.botRepository.InsertLog(ErrorMessages.API_DATETIME_INVALID, ex);
                return false;
            }
        }
    }
}