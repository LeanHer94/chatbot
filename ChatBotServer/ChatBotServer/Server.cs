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