using SampleTestProject.TestData;
using SampleTestProject.Common;
using API_AutomationFramework.Helpers;
using API_AutomationFramework.Common;

namespace SampleTestProject.TestCases
{
    [TestClass]
    public class LoginTestCases : BrowserPage
    {

        [AssemblyInitialize]
        public static void Setup(TestContext context)
        {
            TestRunSettings.Init(context);
            TestReport.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            TestReport.DocumentTitle = "Smoke Test for " + TestReport.AssemblyName;
            TestReport.ReportName = TestReport.DocumentTitle;
            TestReport.HostName = Environment.MachineName;
            TestReport.RunEnvironment = (string.IsNullOrEmpty(TestRunSettings.Environment) ? "Dev External" : TestRunSettings.Environment);
            TestReport.SetTestReport(context);
        }

        [AssemblyCleanup]
        public static void SuiteCleanup()
        {
            TestReport.extentReport?.Flush();
        }

        [TestMethod]
        public async Task ValidateLogin()
        {
            if (IsInvalidTestCase(TestRunSettings.Environment))
            {
                return;
            }

            string TITLE = "Login Validation";
            TestReport.test = TestReport.extentReport?.CreateTest(TITLE);
            
            await Retry(null, async () =>
            {
                LoginPageTestData loginData = new LoginPageTestData();
                loginData.TestEnvironment = TestRunSettings.Environment;

                await PageAction.ActionLoginApplication.Startup(loginData.StartupURL);
                await PageAction.ActionLoginApplication.LoginDEF(loginData.UserName, loginData.Password);

            }, int.Parse(TestRunSettings.RetryTimes), int.Parse(TestRunSettings.RetryDelay), TITLE);

        }
    }
}
