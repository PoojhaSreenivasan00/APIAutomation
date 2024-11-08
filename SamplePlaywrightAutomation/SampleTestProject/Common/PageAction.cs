using SampleTestProject.PageActions;
using API_AutomationFramework.Helpers;

namespace SampleTestProject.Common
{
    public static class PageAction
    {      

        public static Action_LoginApplication ActionLoginApplication
        {
            get
            {
                Action_LoginApplication loginapplication = JsonFileReader.ReadJson<Action_LoginApplication>(@"PageObjects\\LoginPage.json");
                return loginapplication;
            }
        }
    }
}
