namespace ChatBot.Models
{
    public class ErrorMessages 
    {
        public const string WRONG_COMMAND = "Wrong command";

        public const string WRONG_INPUT = "Wrong input. Expected: '<UserName>: <Command> <Timezone>";

        public const string UNKNOWN_TIMEZONE = "unknown timezone";

        public const string API_ISO_COMPLIANT = "API is not ISO 8601 compliant";

        public const string API_TIMEOUT = "API has timed out";

        public const string API_DATETIME_INVALID = "API datetime is not valid or null";
    }
}