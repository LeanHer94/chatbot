using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ChatBotServer
{
    public class Server
    {
        public void Run()
        {
            var apiClient = new ApiClient();
            var ircClient = new IRCClient(apiClient);

            ircClient.Connect();
            ircClient.Run();
        }
    }
}