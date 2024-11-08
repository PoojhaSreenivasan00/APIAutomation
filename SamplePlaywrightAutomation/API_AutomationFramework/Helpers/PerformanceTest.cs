using API_AutomationFramework.Common;
using System.Diagnostics;

namespace API_AutomationFramework.Helpers
{
    public class PerformanceTest
    {
        public static async Task WaitUntilPageLoad()
        {
            try
            {
                // Start the Stopwatch
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await BrowserPage.getPage.WaitForLoadStateAsync();
                bool isJqueryUsed = await BrowserPage.getPage.EvaluateAsync<bool>("typeof(jQuery) != 'undefined'");
                if (isJqueryUsed)
                {
                    do
                    {
                        //Testing the number of active connections to a server and will evaluate true when the number of connections is equal to zero. 
                        //which implies page is fully loaded
                        bool ajaxIsComplete = await BrowserPage.getPage.EvaluateAsync<bool>("(window.jQuery != null) && (jQuery.active == 0)");

                        if (ajaxIsComplete)
                            break;

                    } while (stopwatch.Elapsed.TotalSeconds <= 120);
                }
                stopwatch.Stop();
                await BrowserPage.getPage.WaitForFunctionAsync("document.readyState == 'complete'");
                if (stopwatch.Elapsed.TotalSeconds >= 120)
                {
                    TestReport.test?.Log(AventStack.ExtentReports.Status.Info, "<font color = 'yellow'> " + "Page load wait timed out" + "</font>");
                }

            }
            catch
            {
                //eat Exception
            }
        }
    }

}