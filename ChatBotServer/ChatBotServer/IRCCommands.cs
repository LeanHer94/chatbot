using System;
using System.Collections.Generic;

namespace ChatBotServer
{
    public class IRCCommands
    {
        public const string MESSAGE = "PRIVMSG";

        public const string PING = "PING";

        public const string PONG = "PONG";

        public const string USER = "USER";

        public const string NICK = "NICK";

        public const string JOIN = "JOIN";

        private IDictionary<string, Action<string>> ircCommands { get; }

        private IRCClient client;

        public IRCCommands(IRCClient client)
        {
            this.client = client;

            this.ircCommands = new Dictionary<string, Action<string>>
            {
                { PING, Ping },
                { MESSAGE, Message },
                { USER, User },
                { NICK, Nick },
                { JOIN, Join }
            };
        }

        public void Execute(string data)
        {
            if (data.Contains(PING))
            {
                this.ircCommands[PING](data);
            }

            if (data.Contains(MESSAGE))
            {
                this.ircCommands[MESSAGE](data);
            }
        }

        public void User(string data)
        {
            this.client.Write($"{ USER } { data } { data } { data } :Hi!");
        }

        public void Nick(string data)
        {
            this.client.Write($"{ NICK } { data }");
        }

        public void Join(string data)
        {
            this.client.Write($"{ JOIN } { data }");
        }

        private void Ping(string data)
        {
            var server = data.Split(':')[1];

            this.client.Write($"{ PONG } { server }");
        }

        private void Message(string data)
        {
            var ex = data.Split(':');
            var metaData = ex[1].Split(' ');
            var usr = metaData[0];
            var channel = metaData[2];
            var mje = ex[2];

            var commandInfo = mje.Trim().Split(' ');

            if (commandInfo.Length > 1)
            {
                var command = commandInfo[0];
                var timezone = commandInfo[1];
                var ircCommand = $"{ MESSAGE } { channel } : Hey @ { usr } it is [r]";

                this.client.ExecuteCommand(ircCommand, command, timezone);                
            } 
            else if (this.client.IsAvailableCommand(commandInfo[0]))
            {
                var ircCommand = $"{ MESSAGE } { channel } : Hey @ { usr } they are [r]";
                this.client.ExecuteCommand(ircCommand, commandInfo[0], null);
            }
        }
    }
}