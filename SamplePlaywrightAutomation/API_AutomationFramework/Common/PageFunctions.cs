using Microsoft.Playwright;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Syncfusion.PdfToImageConverter;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text;
using API_AutomationFramework.Helpers;


namespace API_AutomationFramework.Common
{
    public class PageFunctions
    {
        public static async Task Navigate(string url)
        {
            try
            {
                await BrowserPage.getPage.GotoAsync(url);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Navigate to URL - " + url + "\n" + e);
                throw new Exception("Failed to Navigate to URL - " + url);
            }
        }

        public static async Task<string> Title()
        {
            try
            {
                return "" + await BrowserPage.getPage.TitleAsync();
            }
            catch
            {
                Console.WriteLine("Failed to get Page Title");
                return string.Empty;
            }
        }

        public static async Task ValidateTitle(string title, string failureMessage)
        {
            try
            {
                if (!(await Title()).ToUpper().Equals(title.ToUpper()))
                {
                    Console.WriteLine("Title Validation failed - Actual title is " + await Title() + " different from Expected title " + title);
                    throw new Exception(failureMessage);
                }
                else
                {
                    Console.WriteLine("Validated Page Title - " + title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Title Validation failed - " + e);
                throw new Exception(failureMessage);
            }
        }

        public static async Task ValidateNavigation(string element, string failureMessage)
        {
            try
            {
                if (!await ElementDisplayed(element))
                {
                    throw new Exception(failureMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Validate Navigation - " + e);
                throw new Exception(failureMessage);
            }
        }

        public static async Task<bool> IsElementDisplayed(string element)
        {
            //This will immediately throw error if element is not visible and does not wait
            try
            {
                await BrowserPage.getPage.WaitForLoadStateAsync();
                bool visible = await Locator(element).IsVisibleAsync();

                if (!visible)
                {
                    Console.WriteLine(string.Format("{0}:  [{1}] not found", DateTime.Now.ToLocalTime(), element));
                }
                return visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}:  [{1}] not found - {2}", DateTime.Now.ToLocalTime(), element, e.Message));
                return false;
            }
        }

        public static async Task<bool> ElementDisplayed(string element)
        {
            try
            {
                await Locator(element).First.ClickAsync(new LocatorClickOptions { Trial = true });
                bool visible = await Locator(element).First.IsVisibleAsync();

                if (!visible)
                {
                    Console.WriteLine(string.Format("{0}:  [{1}] not found", DateTime.Now.ToLocalTime(), await GetText(element)));
                }
                return visible;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}:  [{1}] not found - {2}", DateTime.Now.ToLocalTime(), element, e.Message));
                return false;
            }
        }

        public static async Task EnterText(string element, string strValue)
        {
            if (!strValue.ToLower().Equals("<::blank::>"))
            {
                try
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] text box", DateTime.Now.ToLocalTime(), strValue, await Locator(element).GetAttributeAsync("id")));
                    var loc = Locator(element).First;
                    await loc.ClearAsync();

                    if (!strValue.ToLower().Equals("<::empty::>"))
                        await loc.FillAsync(strValue);
                }
                catch (Exception exp)
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] is failed with exception {3}", DateTime.Now.ToLocalTime(), strValue, element, exp.Message));
                }

            }
        }

        public static async Task ClickOnElement(string element)
        {
            string? elementName = "";
            try
            {
                elementName = await GetText(element);

                if (elementName?.Length > 30)
                    elementName = elementName.Substring(0, 30);
            }
            catch
            {
                Console.WriteLine("Failed to get element name before click");
            }
            try
            {
                await Locator(element).ClickAsync();
                Console.WriteLine(string.Format("{0}: Clicked on [{1}]", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Click failed on [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }
        }

        public static async Task ClickOnElement(ILocator locator)
        {
            string elementName = "";
            try
            {
                if ((await GetText(locator))?.Length > 30)
                    elementName = (await GetText(locator))!.Substring(0, 30);
                else
                    elementName = (await GetText(locator))!;
            }
            catch
            {
                Console.WriteLine("Failed to get element name before click");
            }
            try
            {
                await locator.ClickAsync();
                Console.WriteLine(string.Format("{0}: Clicked on [{1}]", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Click failed on [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }
        }

        public async Task DoubleClickOnElement(string element)
        {
            string? elementName = await GetText(element);
            try
            {
                await Locator(element).DblClickAsync();
                Console.WriteLine(string.Format("{0}: Double Clicked on [{1}]", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Double Click failed on [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }

        }

        public static void WaitFor(int n)
        {
            Thread.Sleep(TimeSpan.FromSeconds(n));
        }

        public static async Task MoveTo(string element)
        {
            try
            {
                await Locator(element).ScrollIntoViewIfNeededAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to Scroll " + element + " into View\n" + ex);
            }
        }

        public static async Task<string?> GetText(string element)
        {
            try
            {
                string innertext = await Locator(element).First.InnerTextAsync();
                return innertext?.Trim().Replace("\n", "");
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string?> GetText(ILocator element)
        {
            try
            {
                string innertext = await element.InnerTextAsync();
                return innertext?.Trim().Replace("\n", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get Inner Text of " + element + "\n" + ex);
                return string.Empty;
            }
        }

        public static async Task<string?> GetAttribute(string element, string attribute)
        {
            try
            {
                string attributeValue = (await Locator(element).First.GetAttributeAsync(attribute))!;
                return attributeValue?.Trim().Replace("\n", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get Value of Attribute: " + attribute + " for " + element + "\n" + ex);
                return string.Empty;
            }
        }

        public static ILocator Locator(string element)
        {
            return BrowserPage.getPage.Locator(element);
        }

        public static ILocator Locators(string element)
        {
            return BrowserPage.getPage.Locator(element);
        }

        public static string getURL()
        {
            return BrowserPage.getPage.Url.ToString();
        }

        public static async Task<int> getCount(string element)
        {
            return await Locators(element).CountAsync();
        }

        public static async Task WaitForElement(string element)
        {
            await Locator(element).WaitForAsync();
        }

        public static async Task WaitForElement(string element, float timeout)
        {
            try
            {
                await Locator(element).WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = timeout * 1000
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Waiting for Element " + element + " timed out after " + timeout + " milliseconds\n" + ex);
                throw new Exception("Waiting for Element " + element + " timed out after " + timeout + " milliseconds");
            }
        }

        public static async Task SelectFromListBox(string element, string value)
        {
            try
            {
                await Locator(element).SelectOptionAsync(value);
                Console.WriteLine("Selected - " + value + " from List Box value");
            }
            catch
            {
                try
                {
                    await WaitForElement(element);
                    await MoveTo(element);
                    //IReadOnlyList<ILocator> listboxvalues = await Locator(element).AllAsync();
                    await BrowserPage.getPage.QuerySelectorAsync(element + "//*");
                    foreach (var elem in await BrowserPage.getPage.QuerySelectorAllAsync(element + "//*"))
                    {
                        var textContent = await elem.TextContentAsync();

                        if (textContent != null && textContent.Equals(value))
                        {
                            await elem.ClickAsync();
                            Console.WriteLine("Selected - " + value + " - from List Box  value");
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to Select " + value + " from List Box\n" + e);
                    throw new Exception("Failed to Select " + value + " from List Box");
                }
            }
        }

        public static async Task SelectMultipleFromListBox(string element, string[] values)
        {
            try
            {
                await Locator(element).SelectOptionAsync(values);
                foreach (var val in values)
                {
                    Console.WriteLine("Selected - " + val + " from List Box value");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to Select values from List Box" + e);
            }
        }

        public static async Task<bool> IsElementSelected(string element)
        {
            try
            {
                bool eleVisible = await Locator(element).IsCheckedAsync();
                if (!eleVisible)
                {
                    Console.WriteLine(string.Format("{0}: [{1}] element selected", DateTime.Now.ToLocalTime(), element));
                }
                return eleVisible;
            }

            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}:  [{1}] not found - {2}", DateTime.Now.ToLocalTime(), await GetText(element), e.Message));
                return false;
            }

        }

        public static async Task<bool> IsEnabled(string element)
        {
            try
            {
                bool enabled = await Locator(element).IsEnabledAsync();
                if (!enabled)
                {
                    Console.WriteLine(string.Format("{0}: [{1}] element not enabled", DateTime.Now.ToLocalTime(), element));
                }
                return enabled;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0}:  [{1}] not found - {2}", DateTime.Now.ToLocalTime(), element, e.Message));
                return false;
            }
        }

        public static async Task<string?> GetInputValue(string element)
        {
            try
            {
                return await Locator(element).InputValueAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to get value of element " + element + "\n" + ex);
                return string.Empty;
            }
        }

        public static async Task PressSequentially(string element, string strValue, int delay)
        {

            int delayTime = delay * 1000;
            try
            {
                await Locator(element!).PressSequentiallyAsync(strValue, new() { Delay = delayTime });

            }

            catch
            {
                Console.WriteLine("Failed to enter " + strValue);
            }
        }
        public static async Task<string?> GetInputValue(ILocator element)
        {
            try
            {
                return await element.InputValueAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to get value of element " + element + "\n" + ex);
                return string.Empty;
            }
        }
        public static async Task<string?> GetSelectedDropdownText(string element)
        {
            try
            {
                string elementText = await Locator(element).EvaluateAsync<string>("sel => sel.options[sel.options.selectedIndex].textContent");

                return elementText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get Text of Select Dropdown: for " + element + "\n" + ex);
                return string.Empty;
            }
        }

        public static async Task SendKeys(string element, string keyaction)
        {
            await Locator(element).PressAsync(keyaction);

        }

        public static void CreatePDF(string targetFilePath, string fileName)
        {
            try
            {
                targetFilePath = Path.GetFullPath(targetFilePath);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                PdfDocument pdf = new PdfDocument();
                PdfPage pdfPage = pdf.AddPage();
                XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
                graph.DrawString("AMAP PDF FILE", font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.Center);

                try
                {
                    if (File.Exists(targetFilePath + fileName + ".pdf"))
                        File.Delete(targetFilePath + fileName + ".pdf");
                }
                catch
                {
                    //eat exception
                }

                pdf.Save(targetFilePath + fileName + ".pdf");
            }
            catch
            {
                throw new Exception("Error in Creating PDF with the file name " + fileName);
            }

        }

        public static void ConvertPDFintoJPEG(string targetFilePath, string jpegfileName, string pdffilename)
        {
            try
            {
                FileStream inputPDFStream = new FileStream(@targetFilePath + pdffilename + ".pdf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                PdfToImageConverter imageConverter = new PdfToImageConverter(inputPDFStream);
                Bitmap image = new Bitmap(imageConverter.Convert(0, false, false));

                try
                {
                    if (File.Exists(targetFilePath + jpegfileName + ".jpeg"))
                        File.Delete(targetFilePath + jpegfileName + ".jpeg");
                }
                catch
                {
                    //eat exception
                }

                image.Save(targetFilePath + jpegfileName + ".jpeg", ImageFormat.Jpeg);
            }
            catch
            {
                throw new Exception("Error in converting PDF into JPEG with the file name " + jpegfileName);
            }

        }

        public static string? GetFormattedPO(string element, params string[] elementText)
        {
            return string.Format(element!, elementText);

        }

        public static async Task UploadFile(string element, string filePath)
        {
            await Locator(element).SetInputFilesAsync(filePath);
        }

        public static async Task UploadMultipleFiles(string element, string[] filePathArray)
        {
            await Locator(element).SetInputFilesAsync(filePathArray);
        }

        public static async Task ScrollPageDown(int val)
        {
            await BrowserPage.getPage.EvaluateAsync($"window.scrollBy(0, {val})");
        }

        public static async Task ScrollPageUp(int val)
        {
            await BrowserPage.getPage.EvaluateAsync($"window.scrollBy(0, -{val})");
        }

        public static async Task SelectCheckBox(string element)
        {
            await WaitForElement(element);
            await MoveTo(element);

            string? elementName;
            try
            {
                var text = await GetText(element);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await BrowserPage.getPage.Locator(element).GetAttributeAsync("name");
            }
            try
            {
                await BrowserPage.getPage.Locator(element).CheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] checkbox checked", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Failed to Check the Checkbox [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }

        }

        public static async Task SelectCheckBox(ILocator locator)
        {


            string? elementName;
            try
            {
                var text = await GetText(locator);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await locator.GetAttributeAsync("name");
            }
            try
            {
                await locator.CheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] checkbox checked", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Failed to Check the Checkbox [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }

        }

        public static async Task DeSelectCheckBox(string element)
        {
            await WaitForElement(element);
            await MoveTo(element);

            string? elementName;
            try
            {
                var text = await GetText(element);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await BrowserPage.getPage.Locator(element).GetAttributeAsync("name");
            }
            try
            {
                await BrowserPage.getPage.Locator(element).UncheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] checkbox unchecked", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: Failed to Uncheck the Checkbox [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }

        }

        public static async Task SelectRadioButton(string element)
        {
            await MoveTo(element);

            string? elementName;
            try
            {
                var text = await GetText(element);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await BrowserPage.getPage.Locator(element).GetAttributeAsync("name");
            }

            try
            {
                await BrowserPage.getPage.Locator(element).CheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] Radio Button selected", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: failed to select the Radio Button [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }
        }

        public static async Task SelectRadioButton(ILocator locator)
        {

            string? elementName;
            try
            {
                var text = await GetText(locator);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await locator.GetAttributeAsync("name");
            }

            try
            {
                await locator.CheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] Radio Button selected", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: failed to select the Radio Button [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }
        }

        public static async Task UnSelectRadioButton(string element)
        {
            await MoveTo(element);

            string? elementName;
            try
            {
                var text = await GetText(element);
                elementName = !(text?.Length > 20) ? text : text.Substring(0, 20);
            }
            catch
            {
                elementName = await BrowserPage.getPage.Locator(element).GetAttributeAsync("name");
            }

            try
            {
                await BrowserPage.getPage.Locator(element).UncheckAsync();
                Console.WriteLine(string.Format("{0}: [{1}] Radio Button unselected", DateTime.Now.ToLocalTime(), elementName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}: failed to unselect the Radio Button [{1}] - {2}", DateTime.Now.ToLocalTime(), elementName, ex.Message));
            }
        }

        public static async Task<IReadOnlyList<ILocator>> GetLocatorList(string element)
        {
            IReadOnlyList<ILocator> ele = await BrowserPage.getPage.Locator(element).AllAsync();
            return ele;
        }

        public static async Task ClearForm()
        {
            try
            {
                var selectors = new string[] { "input[type=text]", "textarea" };

                // Clear the values of the form elements identified by the selectors
                foreach (var selector in selectors)
                {
                    await BrowserPage.getPage.EvaluateAsync($@"(selector) => {{
         var formElements = document.querySelectorAll('{selector}');
         formElements.forEach(element => {{
             element.value = '';
         }});
     }}", selector);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to clear data" + e);
                throw new Exception("Failed to  clear data");
            }


        }

        public static async Task<bool> IsElementDisabled(string element)
        {
            bool isDisabled = false;
            try
            {
                if (await Locator(element).GetAttributeAsync("disabled") != null)
                    isDisabled = true;
            }
            catch
            {
                isDisabled = false;
            }
            return isDisabled;
        }
        public static async Task<IFileChooser> ClickOnFileSelector(string element)
        {
            var fileChooser = await BrowserPage.getPage.RunAndWaitForFileChooserAsync(async () =>
            {
                await Locator(element).ClickAsync();
            });
            return fileChooser;

        }
        public static async Task EnterText(ILocator element, string strValue)
        {
            if (!strValue.ToLower().Equals("<::blank::>"))
            {
                try
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] text box", DateTime.Now.ToLocalTime(), strValue, await element.GetAttributeAsync("id")));
                    var loc = element.First;
                    await loc.ClearAsync();

                    if (!strValue.ToLower().Equals("<::empty::>"))
                        await loc.FillAsync(strValue);
                }
                catch (Exception exp)
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] is failed with exception {3}", DateTime.Now.ToLocalTime(), strValue, await element.GetAttributeAsync("id"), exp.Message));
                }

            }
        }
        public static async Task SelectFromListBox(ILocator element, string value)
        {
            try
            {
                await element.SelectOptionAsync(value);
                Console.WriteLine("Selected - " + value + " from List Box value");
            }
            catch
            {
                try
                {
                    await element.WaitForAsync();
                    await element.ScrollIntoViewIfNeededAsync();
                    IReadOnlyList<ILocator> listboxvalues = await element.AllAsync();
                    foreach (var elem in listboxvalues)
                    {
                        var textContent = await elem.TextContentAsync();

                        if (textContent != null && textContent.Equals(value))
                        {
                            await elem.ClickAsync();
                            Console.WriteLine("Selected - " + value + " - from List Box  value");
                            break;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to Select " + value + " from List Box\n" + e);
                    throw new Exception("Failed to Select " + value + " from List Box");
                }
            }
        }
        public static async Task<string?> GetAttribute(ILocator element, string attribute)
        {
            try
            {
                string attributeValue = (await element.GetAttributeAsync(attribute))!;
                return attributeValue?.Trim().Replace("\n", "");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to get Value of Attribute: " + attribute + " for " + element + "\n" + ex);
                return string.Empty;
            }
        }
        public static async Task ClearText(string element)
        {
            try
            {
                var loc = Locator(element).First;
                await loc.ClearAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to clear text" + e);
                throw new Exception("Failed to clear text");
            }

        }

        public static void CloseFileUploadWindow()
        {
            WindowsUtility.CloseWindow("#32770", "Open");
        }

        public static async Task EnterTextAndPressTab(string element, string strValue)
        {
            if (!strValue.ToLower().Equals("<::blank::>"))
            {
                try
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] text box", DateTime.Now.ToLocalTime(), strValue, await Locator(element).GetAttributeAsync("id")));
                    var loc = Locator(element).First;
                    await loc.ClearAsync();

                    if (!strValue.ToLower().Equals("<::empty::>"))
                        await loc.FillAsync(strValue);
                    await SendKeys(element, "Tab");
                }
                catch (Exception exp)
                {
                    Console.WriteLine(string.Format("{0}: Enter the value [{1}] in [{2}] is failed with exception {3}", DateTime.Now.ToLocalTime(), strValue, await Locator(element).GetAttributeAsync("id"), exp.Message));
                }

            }
        }

        public static async Task HighlightElement(string element)
        {
            try
            {
                await Locator(element).HighlightAsync();
            }
            catch
            {
                Console.WriteLine("Unable to Highlight Element");
            }
        }
        public static async Task WaitForPageLoad()
        {
            await PerformanceTest.WaitUntilPageLoad();
        }
        public static async Task<string> WaitForDownloadAndSave(string element)
        {
            try
            {
                var waitForDownloadTask = BrowserPage.getPage.WaitForDownloadAsync();
                await ClickOnElement(element);
                var download = await waitForDownloadTask!;
                string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string filePath = Path.Combine(userProfile, "Downloads", download.SuggestedFilename);
                await download.SaveAsAsync(filePath);
                return filePath;
            }
            catch
            {
                throw new Exception("Failed to Download");
            }
        }
    }
}

