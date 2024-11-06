using System.Text.RegularExpressions;

namespace API_AutomationFramework.Helpers
{
    public class Utility
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static string ToSentenceCase(string str)
        {
            str = Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
            return Regex.Replace(str, @"([A-Z]{3,})(?=[A-Z][a-z])", "$1 ");
        }

        public static string getTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timeStamp = now.ToString("MM/dd/yyyy hh:mm:ss");
            string time = timeStamp.Replace(" ", "-").Replace("/", "-").Replace(":", "-");
            return time;
        }
    }
}
