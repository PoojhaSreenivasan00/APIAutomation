using SampleTestProject.PageObjects;
using API_AutomationFramework.Common;
using API_AutomationFramework.Helpers;

namespace SampleTestProject.PageActions
{
    public class Action_LoginApplication:LoginPage
    {      
        public async Task Startup(string url)
        {
            await Navigate(url);
            await ValidateNavigation(LoginUsername!, "Unable to navigate to Login Page");
            await TestReport.LogScreenshot("Navigated to Login Page", BrowserPage.getPage);

        }

        public async Task LoginDEF(string emailUserName, string password)
        {
            await EnterText(LoginUsername!, emailUserName);
            await EnterText(LoginPassword!, password);
            await ClickOnElement(LoginSubmit!);

            await ValidateNavigation(LogOut!, "Failed to Login");
            await TestReport.LogScreenshot("Logged In Successfully!", BrowserPage.getPage);
        }

    }
}
