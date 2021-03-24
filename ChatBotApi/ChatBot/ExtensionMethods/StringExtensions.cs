using System.Linq;

namespace ChatBot.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string[] GetRegions(this string str)
        {
            return str.Split('/');
        }

        public static string GetLastRegion(this string str)
        {
            return str.Split('/').Last();
        }
    }
}