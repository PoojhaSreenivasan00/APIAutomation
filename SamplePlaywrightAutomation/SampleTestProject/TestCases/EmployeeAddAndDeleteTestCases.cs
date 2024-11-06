using API_AutomationFramework.Common;
using API_AutomationFramework.Helpers;
using SampleTestProject.Common;
using SampleTestProject.TestData;

namespace SampleTestProject.TestCases
{
    [TestClass]
    public class EmployeeAddAndDeleteTestCases : BrowserPage
    {
        [TestMethod]
        //Give Meaningful test case names - done
        public async Task ValidateAddAndDeleteEmployee()
        {
            if (IsInvalidTestCase(TestRunSettings.Environment))
            {
                return;
            }

            //change title w.r.t validation - done
            string TITLE = "Add and Delete Eployee Validation";
            TestReport.test = TestReport.extentReport?.CreateTest(TITLE);

            await Retry(null, async() =>
            {
                EmployeeAddAndDeletePageTestData data = new EmployeeAddAndDeletePageTestData();
                data.TestEnvironment = TestRunSettings.Environment;

                await PageAction.OrangePortalLoginApplication.Startup(data.StartupURL);
                await PageAction.OrangePortalLoginApplication.Login(data.UserName, data.Password);
                //Give specific method names so that your test case is readable and understandable - done
                //NavigateToPIM - done
                await PageAction.AddAndDeleteEmployeeApplication.NavigateToPIM();
                //NavigateToAddEmployeePage - done
                await PageAction.AddAndDeleteEmployeeApplication.NavigateToAddEmployeePage();
                await PageAction.AddAndDeleteEmployeeApplication.AddEmployeeDetails();
                await PageAction.AddAndDeleteEmployeeApplication.NavigateToPIM();
                await PageAction.AddAndDeleteEmployeeApplication.SearchForAddedEmployee();
                await PageAction.AddAndDeleteEmployeeApplication.DeleteEmployee();
            }, int.Parse(TestRunSettings.RetryTimes), int.Parse(TestRunSettings.RetryDelay), TITLE);
        }
    }
}
