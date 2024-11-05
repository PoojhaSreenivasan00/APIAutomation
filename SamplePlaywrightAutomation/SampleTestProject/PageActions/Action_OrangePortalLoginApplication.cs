using API_AutomationFramework.Common;
using API_AutomationFramework.Helpers;
using SampleTestProject.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestProject.PageActions
{
    public class Action_OrangePortalLoginApplication : OrangePortalLoginPage
    {
        public async Task Startup(string url)
        {
            await Navigate(url);
            await ValidateNavigation(LoginUsername!, "Unable to navigate to login page");
            await TestReport.LogScreenshot("Navidated to login page", BrowserPage.getPage);
        }

        public async Task Login(string username, string password)
        {
            await EnterText(LoginUsername!, username);
            await EnterText(LoginPassword!, password);
            await ClickOnElement(LoginSubmit!);
            //Validate Navigation
            await ValidateNavigation(DashboardNavigation!, "Failed to Login - Dashboard page not displayed");
            await TestReport.LogScreenshot("Logged In Successfully!", BrowserPage.getPage);
        }
    }
}
