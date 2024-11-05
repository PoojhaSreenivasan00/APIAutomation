using API_AutomationFramework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleTestProject.TestData
{
    public class EmployeeAddAndDeletePageTestData : BaseTestData
    {
        public EmployeeAddAndDeletePageTestData()
        {
            TestDataFolderName = "EmployeeAddAndDeletePage";
            ResourceFile = "EmployeeAddAndDeletePage";
        }

        public string UserName
        {
            get { return "" + GetTestData("username"); }
        }
        public string Password
        {
            get { return "" + GetTestData("password");  }
        }
        public string StartupURL
        {
            get { return "" + GetTestData("startupurl");  }
        }
        public string firstname
        {
            get { return "" + GetTestData("firstname"); }
        }
        public string middlename
        {
            get { return "" + GetTestData("middlename"); }
        }
        public string lastname
        {
            get { return "" + GetTestData("lastname"); }
        }
        public string filePath
        {
            get { return @"TestData\Images\halloween.jpg"; }
        }
    }
}
