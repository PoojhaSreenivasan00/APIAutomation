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

        public static Action_OrangePortalLoginApplication OrangePortalLoginApplication
        {
            get
            {
                Action_OrangePortalLoginApplication orangePortalLoginApplication = JsonFileReader.ReadJson<Action_OrangePortalLoginApplication>(@"PageObjects\\OrangePortalLoginPage.json");
                return orangePortalLoginApplication;
            }
        }
        
        public static Action_AddAndDeleteEmployeeApplication AddAndDeleteEmployeeApplication
        {
            get
            {
                Action_AddAndDeleteEmployeeApplication addAndDeleteEmployeeApplication = JsonFileReader.ReadJson<Action_AddAndDeleteEmployeeApplication>(@"PageObjects\\OrangePortalDashboardNavigatePage.json");
                return addAndDeleteEmployeeApplication;
            }
        }



    }
}
