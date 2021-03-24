# chatbot
ENVIRONMENT
Install .NET Framework 4.7.2 Runtime. https://dotnet.microsoft.com/download/dotnet-framework/net472
Install SQL Server Express (And Management Studio if you want to make your own queries). https://www.microsoft.com/en-us/sql-server/sql-server-downloads

STEPS
------ To Generate SQL Databse Schema -------
Open ChatBot.sln on your Visual Studio. 
Right click on ChatBotDatabase project and select Publish.
Browse your Local SQL Server and publish to databse ChatBotDatabase
Set your SQL Credentials on web.config -> connection strings

------ To Start API ------
Set ChatBot project as Start Up Project (Right Click)
Press the Run button (IIS Express)

------ To Start Bot Server -----
Open ChatBotServer.sln on yout Visual Studio
Press Start and wait until connection is donde.

------ To Talk with the bot ----
Open https://webchat.freenode.net/ on your browser
Enter your NickName and join the channel configured in IRCClient on ChatBotServer.sln (#lxbuniquekdskds)
Send commands to the bot.
To know which commands are available send !commands

------ To Test the API -----
Open Postman
Make a POST request to http://localhost:53109/api/ + timeat / timepopularity
Set Body RAW JSON with content like { "Timezone": "Vancouver" }
