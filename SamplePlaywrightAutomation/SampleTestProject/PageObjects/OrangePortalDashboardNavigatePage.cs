using API_AutomationFramework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestProject.PageObjects
{
    public class OrangePortalDashboardNavigatePage : PageFunctions
    {
        public string? PIMTab {  get; set; }
        public string? AddEmployeeDetailButton { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName {  get; set; }
        public string? LastName { get; set; }
        public string? Image {  get; set; }
        public string? AddEmployeeSaveButton { get; set; }
        public string? AddEmployeeSaveNavigation { get; set; }
        public string? SearchEmployeeName {  get; set; }
        public string? SearchButton {  get; set; }
        public string? DisplayEmployeeFirstName {  get; set; }
        public string? DisplayEmployeeLastName { get; set; }
        public string? DeleteButton {  get; set; }
        public string? ConfirmDeleteButton { get; set; }
        public string? EmployeeNotFoundToast {  get; set; }
    }
}
