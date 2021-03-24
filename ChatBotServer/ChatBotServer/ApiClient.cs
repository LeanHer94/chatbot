using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace ChatBotServer
{
    public class ApiClient
    {
        private string apiServer { get; }

        private HttpClient client { get; }

        public ApiClient()
        {
            this.apiServer = "http://localhost:53109/api/";

            this.client = new HttpClient();
            this.client.BaseAddress = new Uri(apiServer);
        }

        public string Call(string endpoint, string timezone)
        {
            var parameter = new { Timezone = timezone };
            var content = new StringContent(JsonConvert.SerializeObject(parameter), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(endpoint, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}