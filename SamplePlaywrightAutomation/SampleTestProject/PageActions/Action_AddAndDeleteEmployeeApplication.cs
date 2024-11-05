using API_AutomationFramework.Common;
using API_AutomationFramework.Helpers;
using SampleTestProject.PageObjects;
using SampleTestProject.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestProject.PageActions
{
    public class Action_AddAndDeleteEmployeeApplication :OrangePortalDashboardNavigatePage
    {
        private EmployeeAddAndDeletePageTestData data;
        private string employeeName;
        public Action_AddAndDeleteEmployeeApplication()
        {
            data = new EmployeeAddAndDeletePageTestData();
            employeeName = data.firstname + " " + data.middlename + " " + data.lastname;
        }
        public async Task NavigateToPIM()
        {
            await ClickOnElement(PIMTab!);
            //Name PO Properly
            await ValidateNavigation(AddEmployeeDetailButton!, "PIM Navigation Failed");
            await TestReport.LogScreenshot("Navidated to PIM", BrowserPage.getPage);
        }
        public async Task NavigateToAddEmployeePage()
        {
            await ClickOnElement(AddEmployeeDetailButton!);
            await ValidateNavigation(AddEmployeeSaveButton!, "Navigation to Add Employee page failed");
            await TestReport.LogScreenshot("Navidated to Add Employee page", BrowserPage.getPage);
        }
        public async Task AddEmployeeDetails()
        {
            //Pass  file path as test data from test data folder  DO NOT HARDCODE -             get { return @"TestData\BaseImages\DEF_LoginForm.png"; }
            await EnterText(FirstName!, data.firstname);
            await EnterText(MiddleName!, data.middlename);
            await EnterText(LastName!, data.lastname);
            await UploadFile(Image!, data.filePath);
            await TestReport.LogScreenshot("Employee details filled", BrowserPage.getPage);

            await ClickOnElement(AddEmployeeSaveButton!);
            // Wrong PO, Use proper element to validate
            //var displayedName = await GetText(AddEmployeeSaveNavigation!);
            //Assert.AreEqual(displayedName, employeeName);
            await ValidateNavigation(AddEmployeeSaveNavigation!, "Add employee failed");
            await TestReport.LogScreenshot("Employee added!", BrowserPage.getPage);
        }
        public async Task SearchForAddedEmployee()
        {
            //search using full nmae including middle name
            
            await EnterText(SearchEmployeeName!, employeeName) ;
            await ClickOnElement(SearchButton!);
            //Added Employee record is not validated 
            //add steps to validate the added record in table in separate method
            var firstName = await GetText(DisplayEmployeeFirstName!);
            var lastName = await GetText(DisplayEmployeeLastName!);
            Assert.AreEqual(firstName + " " + lastName, employeeName);
            await TestReport.LogScreenshot("Employee found", BrowserPage.getPage);
        }
        public async Task DeleteEmployee()
        {
            //add delete steps in separate method
            await ClickOnElement(DeleteButton!);
            await ClickOnElement(ConfirmDeleteButton!);
            //validate deletion after delete
            //search for the deleted record in table - should not show results
            await ClickOnElement(SearchButton!);
            await WaitForElement(EmployeeNotFoundToast!);
            await ValidateNavigation(EmployeeNotFoundToast!, "Remove Employee Failed");
            await TestReport.LogScreenshot("Employee deleted!", BrowserPage.getPage);
        }
    }
}
