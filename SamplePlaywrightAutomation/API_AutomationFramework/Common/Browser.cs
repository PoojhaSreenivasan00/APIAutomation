using API_AutomationFramework.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace API_AutomationFramework.Common
{
    [TestClass]
    public class BrowserPage : BrowserTest
    {
        public static Dictionary<string, string> GlobalVariables = new Dictionary<string, string>();
        public IBrowserContext Context { get; private set; } = null!;
        private static IPage Page = null!;
        public static IPage getPage
        {
            get { return Page; }
            private set { Page = value; }
        }

        public static string VideoMode = null!;
        public static string TraceMode = null!;
        public static string LastException = "";
        public IList<string> VideoPaths = new List<string>();
        public static string WinUsername = null!;
        public static string WinPassword = null!;
        public static string WinPasswordEncrypt = null!;
        public static string TraceLogPath = null!;
        public static bool IsRetry;

        public BrowserNewContextOptions ContextOptions()
        {
            VideoMode = "" + TestContext?.Properties["videomode"]?.ToString()?.ToLower(); //get video mode from runsettings
            TraceMode = "" + TestContext?.Properties["tracemode"]?.ToString()?.ToLower(); //get trace mode from runsettings
            WinUsername = "" + TestContext?.Properties["winusername"]?.ToString();
            WinPasswordEncrypt = "" + TestContext?.Properties["winpassword"]?.ToString();
            WinPassword = Encoding.ASCII.GetString(Convert.FromBase64String(WinPasswordEncrypt));

            if (VideoMode == "off")
                return new BrowserNewContextOptions()
                {
                    ViewportSize = new()             // To maximize the window
                    {
                        Width = 1920,
                        Height = 1080
                    },
                    HttpCredentials = new HttpCredentials() { Username = WinUsername, Password = WinPassword }
                };
            else
            {
                return new BrowserNewContextOptions()
                {
                    ViewportSize = new()
                    {

                        Width = 1920,
                        Height = 1080
                    },
                    HttpCredentials = new HttpCredentials() { Username = WinUsername, Password = WinPassword },
                    RecordVideoDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PlaywrightReports\\" + TestReport.AssemblyName + "\\TestRecordings\\" + TestContext?.TestName, //video location
                };
            }
        }

        [TestInitialize]

        public async Task PageSetup()
        {
            Context = await NewContextAsync(ContextOptions()).ConfigureAwait(false);
            Context.SetDefaultNavigationTimeout(60000);
            Page = await Context!.NewPageAsync().ConfigureAwait(false);
            //To get Trace log file with Snaps
            if (TraceMode == "on" | TraceMode == "onfailure")
            {
                await Page.Context.Tracing.StartAsync(new()
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
                TraceLogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PlaywrightReports\\" + TestReport.AssemblyName + "\\TraceLogs\\" + TestContext?.TestName;
            }

        }

        [TestCleanup]
        public async Task Cleanup()
        {
            try
            {
                TestReport.WriteTestResultLog(TestContext.FullyQualifiedTestClassName!, TestContext.TestName!, TestContext.CurrentTestOutcome.ToString(), LastException);
                //To Stop and Save the Trace Log
                if (TraceMode == "on" | TraceMode == "onfailure")
                {
                    await Page.Context.Tracing.StopAsync(new()
                    {
                        Path = TraceLogPath + "\\Trace_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff"),//trace log location
                    });
                }
            }
            catch
            {
                Console.WriteLine("Unable to Save Trace Log");
            }

            //Close the Pages and Context
            foreach (var page in Page.Context.Pages.ToList())
            {
                if (VideoMode == "onfailure")
                {
                    VideoPaths.Add(await page.Video!.PathAsync());
                    Console.WriteLine("Video Path added for Page - " + await page.TitleAsync() + "> " + await page.Video.PathAsync());
                }

                await page.CloseAsync();
            }

            await Page.Context.CloseAsync();

            //To Delete Recorded Video when videomode is set to save only OnFailure
            if (VideoMode == "onfailure")
            {
                if (TestContext!.CurrentTestOutcome != UnitTestOutcome.Failed)
                {
                    Thread.Sleep(1000);
                    foreach (var vpath in VideoPaths)
                    {
                        try
                        {
                            if (File.Exists(vpath))
                            {
                                File.Delete(vpath);
                                Console.WriteLine("Video Deleted at " + vpath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error in Deleting Video at " + vpath + "\n" + ex);
                        }
                    }
                }
            }
            if (TraceMode == "onfailure" && !IsRetry && TestContext!.CurrentTestOutcome == UnitTestOutcome.Passed)
            {
                try
                {
                    if (Directory.Exists(TraceLogPath))
                    {
                        Directory.Delete(TraceLogPath, true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in Deleting Tracelog folder at " + TraceLogPath.TrimEnd('\\') + "\n" + ex);
                }
            }
            IsRetry = false;

        }

        public async Task OpenBrowser()
        {
            await PageSetup();
        }

        public async Task CloseBrowser()
        {
            //To Stop and Save the Trace Log
            if (TraceMode == "on" | TraceMode == "onfailure")
            {
                await Page.Context.Tracing.StopAsync(new()
                {
                    Path = TraceLogPath + "\\Trace_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff"),//trace log location
                });

            }

            foreach (var page in Page.Context.Pages.ToList())
            {
                if (VideoMode == "onfailure")
                {
                    VideoPaths.Add(await page.Video!.PathAsync());
                    Console.WriteLine("Video Path added for Page - " + await page.TitleAsync() + "> " + await page.Video.PathAsync());
                }
                await page.CloseAsync();
            }

            await Page.Context.CloseAsync();
        }

        public static async Task Refresh()
        {
            await Page.ReloadAsync();
        }

        public bool IsInvalidTestCase(string environment)
        {
            TestRunFilters.Init(environment);
            return (TestRunFilters.FilterMethods.Count > 0) && !TestRunFilters.FilterMethods.Contains(string.Format("{0}.{1}", TestContext.FullyQualifiedTestClassName, TestContext.TestName));
        }

        public static async Task HandleException(string title, Exception e)
        {
            LastException = e.Message;
            try
            {
                await TestReport.LogScreenshotFail(title, title + " Failed - " + LastException, Page);
            }
            catch
            {
                //eat Exception
            }

            Console.WriteLine(e);
            Assert.Fail(title + " failed - " + e.Message);
        }

        public static async Task SwitchLastPagebyContext()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Current Page Title is >> " + await Page.TitleAsync());
            Page = Page.Context.Pages.Last();
            await Page.BringToFrontAsync();
            await Page.WaitForLoadStateAsync();
            Console.WriteLine("Switched Page Title is >> " + await Page.TitleAsync());
        }

        public static async Task SwitchPagebyTitle(string neededpagetitle)
        {
            Console.WriteLine("Current Page Title is >> " + await Page.TitleAsync());
            foreach (var page in Page.Context.Pages)
            {
                if (await page.TitleAsync() == neededpagetitle)
                {
                    Page = page;
                    await Page.BringToFrontAsync();
                    await Page.WaitForLoadStateAsync();
                    Console.WriteLine("Switched Page Title is >> " + await Page.TitleAsync());
                    break;
                }
            }
        }

        public static async Task SwitchToPage(IPage neededPage)
        {
            Console.WriteLine("Current Page Title is >> " + await Page.TitleAsync());
            Page = neededPage;
            await Page.BringToFrontAsync();
            Console.WriteLine("Switched Page Title is >> " + await Page.TitleAsync());
        }

        public static async Task<IPage> SwitchToNewWindow(string element)
        {
            var newPage = await Page.Context.RunAndWaitForPageAsync(async () =>
            {
                await PageFunctions.ClickOnElement(element);
                Console.WriteLine("Switched Page Title is >> " + await Page.TitleAsync());
            });
            await newPage.WaitForLoadStateAsync();
            Page = newPage;
            return newPage;
        }
        public async Task Retry(Func<Task>? clearMethod, Func<Task> testMethod, int retry = 3, int sleep = 0, [CallerMemberName] string? testName = null, bool closeBrowser = true)
        {

            int current = 1;
            Exception failed = null!;
            retry = Math.Max(1, Math.Min(retry, 10));
            while (current <= retry)
            {
                if (current > 1)
                    TestReport.test?.Log(AventStack.ExtentReports.Status.Info, "<font color = 'blue'> " + "Attempt " + current + " -  " + "<u><i><b>" + testName + "</u></i></b>" + "</font>");

                try
                {
                    await testMethod();
                    failed = null!;
                    break;
                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Test {0} failed ({1}. try): {2}", testName, current, ex);
                    Console.WriteLine("Test {0} failed ({1}. try): {2}", testName, current, ex);
                    failed = ex;
                    IsRetry = true;
                    //To log failure attempt screenshot in report as Warning
                    if (retry > 1)
                    {
                        try
                        {
                            await TestReport.LogScreenshotWarning(testName!, "<font color = 'orange'> <b> " + "Attempt " + current + " failed - " + "</b>" + ex.Message + "</ font >" + "<br>", BrowserPage.getPage);
                        }
                        catch
                        {
                            //eat exception
                        }
                    }

                    if (clearMethod != null)
                        await clearMethod.Invoke();

                    if (closeBrowser && current != retry)
                    {
                        await CloseBrowser();
                        await OpenBrowser();
                    }
                }

                current++;
            }

            if (failed != null)
            {
                await HandleException(testName!, failed);
            }
        }
        public async Task GoToNewTab()
        {
            await SwitchToPage(await BrowserPage.getPage.Context.NewPageAsync());
        }
    }
}