using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace API_AutomationFramework.Helpers
{
    public class TestReport
    {
        public static ExtentV3HtmlReporter? htmlReport;
        public static AventStack.ExtentReports.ExtentReports? extentReport;
        public static ExtentTest? test;
        public static string? projectPath;
        public static TestContext? _testContext;
        public static string? AssemblyName;
        public static string? DocumentTitle;
        public static string? ReportName;
        public static string? HostName;
        public static string? RunEnvironment;

        public static string ReportsBasePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public static string ReportsPath()
        {
            return ReportsBasePath() + "\\PlaywrightReports\\" + AssemblyName;
        }

        public static string ReportsSummaryPath()
        {
            return ReportsBasePath() + "\\ReportsSummary\\" + AssemblyName;
        }

        public static string ReportsImagePath()
        {
            return ReportsPath() + "\\Images";
        }


        public static void SetTestReport(TestContext context)
        {
            if (extentReport != null) return;

            _testContext = context;

            extentReport = new AventStack.ExtentReports.ExtentReports();

            projectPath = AppDomain.CurrentDomain.BaseDirectory;

            htmlReport = new ExtentV3HtmlReporter(ReportsPath() + "\\dashboard.html");
            htmlReport.Config.DocumentTitle = DocumentTitle;
            htmlReport.Config.ReportName = ReportName;
            htmlReport.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

            extentReport.AttachReporter(htmlReport);

            Directory.CreateDirectory(ReportsImagePath());

            //Add QA system info to html report

            extentReport.AddSystemInfo("Host Name", HostName);
            extentReport.AddSystemInfo("Environment", RunEnvironment);
        }

        public static string ReplaceSpecialCharacters(string comment)
        {
            return Regex.Replace(Regex.Replace(comment, @"(\s+|!|:|""|@|')", "_"), @"(\\|\/|\*|&|<|>|\+|\||\?)", "");
        }

        public static async Task LogScreenshot(string comment, IPage _page, Status status = Status.Pass)
        {
            string fileName = ReplaceSpecialCharacters(comment) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff") + ".png";

            await _page.ScreenshotAsync(new()
            {
                Path = ReportsImagePath() + @"\" + fileName
            });
            test?.Log(status, comment, MediaEntityBuilder.CreateScreenCaptureFromPath(@".\Images\" + fileName).Build());

        }

        public static async Task LogScreenshotFail(string fileName, string comment, IPage _page)
        {
            fileName = "Fail_" + ReplaceSpecialCharacters(fileName) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff") + ".png";

            await _page.ScreenshotAsync(new()
            {
                Path = ReportsImagePath() + @"\" + fileName
            });
            test?.Log(Status.Fail, comment, MediaEntityBuilder.CreateScreenCaptureFromPath(@".\Images\" + fileName).Build());
        }

        public static async Task LogScreenshotWarning(string fileName, string comment, IPage _page)
        {
            string failedfileName = "AttemptFailed_" + ReplaceSpecialCharacters(fileName) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff") + ".png";
            await _page.ScreenshotAsync(new()
            {
                Path = ReportsImagePath() + @"\" + failedfileName
            });
            test?.Log(Status.Warning, comment, MediaEntityBuilder.CreateScreenCaptureFromPath(@".\Images\" + failedfileName).Build());
        }

        public static void LogScreenshot(string comment, IPage page, Image image, Status status = Status.Pass)
        {
            string fileName = ReplaceSpecialCharacters(comment) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff") + ".png";
            image.Save(ReportsImagePath() + @"\" + fileName!, System.Drawing.Imaging.ImageFormat.Png);
            test?.Log(status, comment, MediaEntityBuilder.CreateScreenCaptureFromPath(@".\Images\" + fileName).Build());
        }

        public static void WriteTestResultLog(string appName, string methodName, string outcome, string failureMessage = "")
        {
            try
            {
                string fileName = "\\TestResults.log";
                string path = ReportsSummaryPath() + fileName;
                Directory.CreateDirectory(ReportsSummaryPath());

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    appName = appName.Substring(0, appName.IndexOf(".")).Replace("API_", "");
                    writer.WriteLine(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", Utility.ToSentenceCase(appName), Utility.ToSentenceCase(methodName), outcome, failureMessage));
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }
        public static async Task LogScreenshotFullPage(string comment, IPage _page, Status status = Status.Pass)
        {
            string fileName = ReplaceSpecialCharacters(comment) + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff") + ".png";
          
            await _page.ScreenshotAsync (new()
            {
                Path = ReportsImagePath() + @"\" + fileName,
                FullPage = true,
            });
            test?.Log(status, comment, MediaEntityBuilder.CreateScreenCaptureFromPath(@".\Images\" + fileName).Build());

        }

    }
}
