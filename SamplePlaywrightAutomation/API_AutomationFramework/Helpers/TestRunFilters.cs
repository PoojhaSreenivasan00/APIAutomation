using System.Data;

namespace API_AutomationFramework.Helpers
{
    public class TestRunFilters
    {
        public static List<string> FilterMethods = new List<string>();
        public static string TEST_FILTERS_FILE = "testfilters.xml";

        public TestRunFilters()
        {

        }

        public static void Init(string environment)
        {
            if (FilterMethods.Count > 0 || string.IsNullOrEmpty(environment)) return;

            using (DataSet ds = new DataSet())
            {
                ds.ReadXml(TEST_FILTERS_FILE);

                FilterMethods = new List<string>();
                for (int i = 0; i < ds.Tables[environment]!.Rows.Count; i++)
                {
                    FilterMethods.Add("" + ds.Tables[environment]!.Rows[i][0]);
                }
            }
        }
    }
}
