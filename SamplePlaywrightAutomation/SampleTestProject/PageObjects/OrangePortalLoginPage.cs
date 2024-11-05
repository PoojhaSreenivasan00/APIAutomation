using API_AutomationFramework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestProject.PageObjects
{
    public class OrangePortalLoginPage : PageFunctions
    {
        public string? LoginUsername {  get; set; }
        public string? LoginPassword { get; set; }
        public string? LoginSubmit {  get; set; }
        public string? DashboardNavigation {  get; set; }
    }
}
