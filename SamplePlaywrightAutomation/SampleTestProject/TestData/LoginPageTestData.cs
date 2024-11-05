using API_AutomationFramework.Common;

namespace SampleTestProject.TestData
{
    class LoginPageTestData : BaseTestData
    {
        public LoginPageTestData()
        {
            TestDataFolderName = "LoginPage";
            ResourceFile = "LoginPage";
        }

        public string UserName
        {
            get { return "" + GetTestData("username"); }
        }

        public string Password
        {
            get { return "" + GetTestData("password"); }
        }

        public string StartupURL
        {
            get { return "" + GetTestData("startupurl"); }
        }

    }
        
}
