using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;

namespace ChatBotServer
{
    public class Server
    {
        private const string TIME_AT = "!timeat";
        private const string TIME_POPULARITY = "!timepopularity";

        private IDictionary<string, string> commands;

        public Server()
        {
            this.commands = new Dictionary<string, string>
            {
                { TIME_AT, "timeat" },
                { TIME_POPULARITY, "timepopularity" }
            };
        }

        public void Init()
        {
            //api configuration
            var apiServer = "http://localhost:53109/api/";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(apiServer);

            //irc configuration
            var ircServer = "irc.freenode.net";
            var port = 6667;
            var botNick = "LxbTimeZoneGuru";
            var channel = "#lxbuniquekdskds"; //https://webchat.freenode.net/

            var irc = new TcpClient(ircServer, port);

            NetworkStream stream = irc.GetStream();
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true };

            sw.WriteLine("USER " + botNick + " " + botNick + " " + botNick + " :Hi!");
            sw.WriteLine("NICK " + botNick);
            sw.WriteLine("JOIN " + channel);

            while (true)
            {
                var data = sr.ReadLine();

                if (data != null)
                {
                    if (data.Contains("PING"))
                    {
                        var server = data.Split(':')[1];
                        sw.WriteLine("PONG " + server);

                        continue;
                    }

                    Console.WriteLine(data);

                    if (data.Contains("PRIVMSG"))
                    {
                        //:tepper.freenode.net 005 LxbTimeZoneGuru CHARSET=ascii NICKLEN=16 CHANNELLEN=50 TOPICLEN=390 DEAF=D FNC TARGMAX=NAMES:1,LIST:1,KICK:1,WHOIS:1,PRIVMSG:4,NOTICE:4,ACCEPT:,MONITOR: EXTBAN=$,ajrxz CLIENTVER=3.0 ETRACE KNOCK WHOX :are supported by this server
                        var ex = data.Split(':');
                        var metaData = ex[1].Split(' ');
                        var usr = metaData[0];
                        var ircCommand = metaData[1];
                        var chnel = metaData[2];
                        var mje = ex[2];

                        var commandInfo = mje.Trim().Split(' ');

                        var command = commandInfo[0];
                        var timezone = commandInfo[1];

                        if (this.commands.TryGetValue(command, out string url))
                        {
                            var parameter = new Dictionary<string, string> { { "input", timezone } };
                            var encodedContent = new FormUrlEncodedContent(parameter);
                            HttpResponseMessage response = client.PostAsync(url, encodedContent).Result;
                        }
                    }
                }

                Thread.Sleep(200);
            }
        }
    }
}