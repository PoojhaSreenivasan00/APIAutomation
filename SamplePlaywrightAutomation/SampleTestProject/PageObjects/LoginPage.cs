using API_AutomationFramework.Common;

namespace SampleTestProject.PageObjects
{
    public class LoginPage:PageFunctions
    {
        public string? LoginUsername { get; set; }
        public string? LoginPassword { get; set; }
        public string? LoginSubmit { get; set; }
        public string? LogOut { get; set; }

    }
}
