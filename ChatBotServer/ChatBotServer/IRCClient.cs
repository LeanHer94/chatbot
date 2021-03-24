using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ChatBotServer
{
    public class IRCClient
    {
        private const string TIME_AT = "!timeat";
        private const string TIME_POPULARITY = "!timepopularity";
        private const string COMMANDS = "!commands";

        private IDictionary<string, string> commandApis;

        private IDictionary<string, Action<string>> commandsLocal;

        private string ircServer { get; }

        private int port { get; }

        private string botNick { get; }

        private string channel { get; }

        private TcpClient client;

        private NetworkStream stream;

        private StreamReader sr;

        private StreamWriter sw;

        private IRCCommands IRCCommands;

        private ApiClient apiClient;

        public IRCClient(ApiClient apiClient)
        {
            this.IRCCommands = new IRCCommands(this);
            this.apiClient = apiClient;
            this.ircServer = "irc.freenode.net";
            this.port = 6667;
            this.botNick = "LxbTimeZoneGuru";
            this.channel = "#lxbuniquekdskds";

            this.client = new TcpClient(this.ircServer, this.port);

            this.stream = this.client.GetStream();
            this.sr = new StreamReader(this.stream);
            this.sw = new StreamWriter(this.stream) { NewLine = "\r\n", AutoFlush = true };

            this.commandApis = new Dictionary<string, string>
            {
                { TIME_AT, "timeat" },
                { TIME_POPULARITY, "timepopularity" }
            };

            this.commandsLocal = new Dictionary<string, Action<string>>
            {
                { COMMANDS, GetAvailableCommands }
            };
        }

        public void Connect()
        {
            this.IRCCommands.User(this.botNick);
            this.IRCCommands.Nick(this.botNick);
            this.IRCCommands.Join(this.channel);
        }

        public void Run()
        {
            while (true)
            {
                var data = this.sr.ReadLine();

                if (!string.IsNullOrWhiteSpace(data))
                {
                    this.IRCCommands.Execute(data);

                    Console.WriteLine(data);
                }

                Thread.Sleep(200);
            }
        }

        public void Write(string stream)
        {
            this.sw.WriteLine(stream);
        }

        public void ExecuteCommand(string ircCommand, string command, string timezone)
        {
            if (this.commandApis.TryGetValue(command, out string url))
            {
                var result = this.apiClient.Call(url, timezone);

                var print = ircCommand.Replace("[r]", result);

                Console.WriteLine(print);
                this.Write(print);
            }
            else if (this.commandsLocal.TryGetValue(command, out Action<string> action))
            {
                action(ircCommand);
            }
        }

        public void GetAvailableCommands(string ircCommand)
        {
            var result = $"{ TIME_AT }, { TIME_POPULARITY}";
            var print = ircCommand.Replace("[r]", result);

            this.Write(print);
        }

        public bool IsAvailableCommand(string command)
        {
            return !string.IsNullOrEmpty(command) &&
                (this.commandsLocal.TryGetValue(command, out _) ||
                this.commandApis.TryGetValue(command, out _));
        }
    }
}