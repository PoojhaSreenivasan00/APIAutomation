namespace API_AutomationFramework.Helpers
{
    public class TestRunSettings
    {
        public static Dictionary<string, string> RunSettings = new Dictionary<string, string>();
        public static string RUN_SETTINGS_FILE = "runsettings.xml";
        public static TestContext? TestContext { get; set; }

        public TestRunSettings()
        {

        }

        public static void Init(TestContext testContext)
        {
            TestContext = testContext;
        }

        public static string Environment { get { return "" + TestContext?.Properties["environment"]?.ToString(); } }
        public static string AppCategory { get { return "" + TestContext?.Properties["appcategory"]?.ToString(); } }
        public static string RetryTimes { get { return "" + TestContext?.Properties["retrytimes"]?.ToString(); } }
        public static string RetryDelay { get { return "" + TestContext?.Properties["retrydelay"]?.ToString(); } }
        public static string AzurePortalConnectionString { get { return "" + TestContext?.Properties["AzurePortalConnectionString"]?.ToString(); } }


        public static string getBaseUrl(string appName)
        {
            return "" + TestContext?.Properties[appName]?.ToString();
        }
    }
}
