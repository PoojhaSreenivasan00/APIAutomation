using API_AutomationFramework.Helpers;
using Microsoft.Playwright;

namespace API_AutomationFramework.Common
{
    public class BrokenLinks : PageFunctions
    {

        public static async Task<List<string>> CollectAnchorUrls(string baseUrl, List<string> ignoreStrings)
        {
            var aElements = await BrowserPage.getPage.QuerySelectorAllAsync("a");
            List<string> urls = new List<string>();

            foreach (var we in aElements)
            {
                string innerTxt = await we.InnerTextAsync();
                if (!string.IsNullOrEmpty(innerTxt))
                {
                    bool found = false;
                    foreach (string word in ignoreStrings)
                    {
                        if (innerTxt.Replace(" ", "").ToLower().Contains(word))
                            found = true;
                    }

                    if (found) continue;
                }

                string? link = await we.GetAttributeAsync("href");

                if (link!.Equals("#") | string.IsNullOrEmpty(link))
                {
                    continue;
                }
                if (link.Equals("javascript:void(0);"))
                {

                     string? link1 = await we.GetAttributeAsync("onclick");
                     if (link1!.Contains("'"))
                         link = link1.Split("'")[1];
                     else link = link1;
                }
                if (!string.IsNullOrEmpty(link))
                {
                    bool found = false;
                    foreach (string word in ignoreStrings)
                    {
                        if (link.Replace(" ", "").ToLower().Contains(word))
                            found = true;
                    }

                    if (found) continue;
                }

                if (!link.StartsWith("http"))
                    link = baseUrl.Substring(0, baseUrl.IndexOf("/", 9)) + link;

                try
                {
                    string truncLink = "";
                    bool found = false;

                    if (link.Contains("?"))
                    {
                        truncLink = link.Substring(0, link.IndexOf("?"));
                        foreach (string url in urls)
                        {
                            if (url.StartsWith(truncLink))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        truncLink = link.Substring(0, link.LastIndexOf("/"));
                        if (!truncLink.Equals("http:/"))
                        {
                            foreach (string url in urls)
                            {
                                if (!url.Contains("?") && url.StartsWith(truncLink))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                    }

                    if (!found)
                        urls.Add(link);
                }
                catch
                {
                    //Eat this
                }
            }

            return urls;
        }

        public static async Task ValidateBrokenLinks(string baseUrl, List<string> urls)
        {
            foreach (string link in urls)
            {
                try
                {

                    IResponse? Response = await BrowserPage.getPage.GotoAsync(link);
                    int statusCode = Response!.Status;

                    if (statusCode >= 100 && statusCode < 400) //Good requests
                    {
                        TestReport.test?.Pass(string.Format("Passed with Status {0} - {1}:{2}", statusCode, await Title(), link));
                    }
                    else if (statusCode >= 500 && statusCode <= 510) //Server Errors
                    {
                        TestReport.test?.Fail(string.Format("Failed with Status {0} - {1}:{2}", statusCode, await Title(), link));
                    }
                    else
                    {
                        TestReport.test?.Warning(string.Format("Unknown Status {0} - {1}:{2}", statusCode, await Title(), link));
                    }
                }
                catch (Exception ex)
                {
                    TestReport.test?.Warning(string.Format("Exception Occurred {0} - {1}:{2}", ex.Message, await Title(), link));
                }
                finally
                {
                    await Navigate(baseUrl);
                }
                }
            }
        }
    }

