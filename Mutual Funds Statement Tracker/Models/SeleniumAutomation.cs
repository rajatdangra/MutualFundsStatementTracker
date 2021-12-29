using Mutual_Funds_Statement_Tracker.Methods;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class SeleniumAutomation
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public SeleniumAutomation()
        {
        }

        public SeleniumResponse Navigate(String url, UserProfile user)
        {
            var response = new SeleniumResponse();
            response.Message = "[" + DateTime.Now.ToDetailString() + "]: ";
            if (String.IsNullOrEmpty(url)) return response;
            if (url.Equals("about:blank")) return response;
            if (!url.StartsWith("http://") &&
                !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            IWebDriver driver = null;
            try
            {
                ////Via JS
                //StringBuilder sb = new StringBuilder();
                //sb.Append("<script type = 'text/javascript'>");
                //sb.Append("window.open('");
                //sb.Append(url);
                //sb.Append("','_self');");
                //sb.Append("</script>");
                //ClientScript.RegisterStartupScript(this.GetType(),
                //        "script", sb.ToString());

                logger.Info("Start Navigating to RTA URL: " + url);
                if (Debugger.IsAttached || !AppConfig.ShowAutomation)
                {
                    //IWebDriver driver = new ChromeDriver(driverService, chromeOptions);
                    driver = new ChromeDriver();
                }
                else
                {
                    //ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                    //driverService.HideCommandPromptWindow = true;

                    ChromeOptions chromeOptions = new ChromeOptions();
                    //chromeOptions.AddArgument("--incognito"); //Opens in incognito mode
                    //chromeOptions.AddArguments("--start-maximized"); //Maximize window
                    chromeOptions.AddArguments("--display"); //Maximize window
                                                             //chromeOptions.AddArguments("--window-size=1920,1080"); //Set Window size
                    chromeOptions.AddArgument("--disable-notifications"); //Disable Popup Site Notifications
                    chromeOptions.AddArgument("--disable-popup-blocking"); //Disables pop-ups displayed
                    chromeOptions.AddArgument("--disable-renderer-backgrounding");
                    chromeOptions.AddArgument("--disable-headless-mode");
                    chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager;
                    //chromeOptions.AddExcludedArgument("enable-automation"); // Hide Automated Warning
                    //chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                    //chromeOptions.AddArgument("no-sandbox");
                    //chromeOptions.AddArguments("--headless"); //Hide visibility
                    //chromeOptions.LeaveBrowserRunning = false;
                    //chromeOptions.AddAdditionalCapability(ChromeOptions.Capability);

                    var ip = new IPDetails().GetLocalIPv4(); //Server IP
                    var port = AppConfig.Port;
                    var serverUrl = "http://" + ip + ":" + port;
                    driver = new RemoteWebDriver(new Uri(serverUrl + "/wd/hub"), chromeOptions);
                }
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);

                logger.Info("Navigate to RTA url successful. URL: " + url);

                try
                {
                    logger.Info("Automation on RTA url started, setting all inputs.");

                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(AppConfig.TimeOut));
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(Convert.ToInt32(AppConfig.PollingTime));
                    /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                    fluentWait.IgnoreExceptionTypes(typeof(ElementNotVisibleException));
                    fluentWait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                    fluentWait.Message = "Element to be searched not found";

                    // Find the text input element by its id

                    #region Accept T&C's
                    IWebElement acceptRadioClick = fluentWait.Until(a => a.FindElement(By.XPath("//mat-radio-button[@id = 'mat-radio-2' and @value = 'ACCEPT']/label/div")));
                    if (acceptRadioClick != null)
                    {
                        acceptRadioClick.Click();
                        IWebElement proceedButtonClick = fluentWait.Until(a => a.FindElement(By.XPath("//input[@type='button' and @value = 'PROCEED']")));
                        if (proceedButtonClick != null)
                            proceedButtonClick.Click();
                    }
                    #endregion

                    #region Main Website

                    IWebElement detailedRadioClick = fluentWait.Until(a => a.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-6' and @value = 'detailed']")));
                    if (detailedRadioClick != null)
                        detailedRadioClick.Click();

                    IWebElement specificPeriodRadioClick = fluentWait.Until(a => a.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-14' and @value = 'SP']")));
                    if (specificPeriodRadioClick != null)
                        specificPeriodRadioClick.Click();

                    //Set Start Date
                    SetCalendarDate(fluentWait, "mat-form-field-suffix ng-tns-c4-8 ng-star-inserted", new DateTime(1990, 4, 1), ref response);

                    //Set End Date
                    SetCalendarDate(fluentWait, "mat-form-field-suffix ng-tns-c4-9 ng-star-inserted", DateTime.Now, ref response);

                    IWebElement withZeroRadioClick = fluentWait.Until(a => a.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-8' and @value = 'N']")));
                    if (withZeroRadioClick != null)
                        withZeroRadioClick.Click();

                    IWebElement emailWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-0' and @placeholder='Email']")));

                    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Convert.ToInt32(AppConfig.TimeOut)));
                    ////wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException)); // ignore stale exception issues
                    //wait.Until(ExpectedConditions.TextToBePresentInElementValue(emailWE, text));

                    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
                    //fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-0' and @placeholder='Email']")).Text.Length >= 6);

                    if (emailWE != null)
                        emailWE.SendKeys(user.Email);

                    IWebElement panWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-1' and @placeholder='PAN']")));
                    if (panWE != null)
                        panWE.SendKeys(user.PAN);

                    IWebElement passwordWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-2' and @placeholder='Password']")));
                    if (passwordWE != null)
                        passwordWE.SendKeys(user.Password);

                    IWebElement confirmPasswordWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-3' and @placeholder='Confirm Password']")));
                    if (confirmPasswordWE != null)
                        confirmPasswordWE.SendKeys(user.Password);

                    IWebElement submitButton = fluentWait.Until(a => a.FindElement(By.XPath("//button[@type='submit']")));
                    if (submitButton != null)
                    {
                        submitButton.Click();
                        logger.Info("Request Details Submitted.");
                        //logger.Info("Waiting for 5 seconds to load page.");
                        //Thread.Sleep(5000);
                    }

                    IWebElement successReferenceNumber = fluentWait.Until(a => a.FindElement(By.XPath("//div[@class='success']/div/p")));
                    if (successReferenceNumber != null)
                    {
                        var successRefNoResponse = successReferenceNumber.Text;
                        logger.Info("Automation on RTA url successful.");
                        logger.Info(successRefNoResponse);
                        response.Message += successRefNoResponse;
                        response.Message += successRefNoResponse;
                        response.IsSuccessfull = true;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    response.Message += "Statement generation not successful";
                    logger.Info(response.Message);
                    logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                    response.ErrorMessage = "<span style='color:red'>" + ex.Message + "</span>";
                }
                finally
                {
                    if (driver != null)
                    {
                        logger.Info("Closing Browser.");
                        driver.Quit();
                    }
                }
            }
            catch (System.UriFormatException ex)
            {
                response.Message += "Statement generation not successful";
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                response.ErrorMessage = "<span style='color:red'>" + ex.Message + "</span>";
            }
            catch (Exception ex)
            {
                response.Message += "Statement generation not successful";
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                response.ErrorMessage = "<span style='color:red'>" + ex.Message + "</span>";
            }
            finally
            {
                if (driver != null)
                {
                    logger.Info("Closing Browser.");
                    driver.Quit();
                }
            }

            return response;
        }

        private void SetCalendarDate(DefaultWait<IWebDriver> fluentWait, string className, DateTime date, ref SeleniumResponse response)
        {
            try
            {
                logger.Info("Setting date: " + date.ToString("dd-MMM-yyyy"));
                //*****Day selection started.
                //Click Datepicker
                IWebElement datePicker = fluentWait.Until(a => a.FindElement(By.XPath("//div[@class='" + className + "']/mat-datepicker-toggle/button[@aria-label='Open calendar']")));
                datePicker.Click();

                int year = date.Year;
                string monthBeginning = date.AddDays(-date.Day + 1).ToString("dd-MMM-yyyy");
                string exactDateFormat = date.ToString("dd-MMM-yyyy");

                IWebElement choseDate = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-period-button mat-button']")));
                choseDate.Click();

                IWebElement yearRange = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-period-button mat-button']/span[@class='mat-button-wrapper']")));
                string[] years = yearRange.Text.Split('–');
                int startYear = Convert.ToInt32(years[0].Trim());
                int endYear = Convert.ToInt32(years[1].Trim());
                IWebElement prevButton = null;
                IWebElement nextButton = null;
                if (year < startYear)
                {
                    prevButton = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-previous-button mat-icon-button']")));
                    while (year < startYear)
                    {
                        prevButton.Click();
                        yearRange = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-period-button mat-button']/span[@class='mat-button-wrapper']")));
                        years = yearRange.Text.Split('–');
                        startYear = Convert.ToInt32(years[0].Trim());
                        endYear = Convert.ToInt32(years[1].Trim());
                    }
                }
                else if (year > endYear)
                {
                    nextButton = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-next-button mat-icon-button']")));
                    while (year > endYear)
                    {
                        nextButton.Click();
                        yearRange = fluentWait.Until(a => a.FindElement(By.XPath("//button[@class='mat-calendar-period-button mat-button']/span[@class='mat-button-wrapper']")));
                        years = yearRange.Text.Split('–');
                        startYear = Convert.ToInt32(years[0].Trim());
                        endYear = Convert.ToInt32(years[1].Trim());
                    }
                }
                IWebElement clickYear = fluentWait.Until(a => a.FindElement(By.XPath("//td[(@class='mat-calendar-body-cell ng-star-inserted' or @class='mat-calendar-body-cell mat-calendar-body-active ng-star-inserted') and @aria-label='" + year + "']")));
                clickYear.Click();
                IWebElement clickMonth = fluentWait.Until(a => a.FindElement(By.XPath("//td[(@class='mat-calendar-body-cell ng-star-inserted' or @class='mat-calendar-body-cell mat-calendar-body-active ng-star-inserted') and @aria-label='" + monthBeginning + "']")));
                clickMonth.Click();
                IWebElement clickDay = fluentWait.Until(a => a.FindElement(By.XPath("//td[(@class='mat-calendar-body-cell ng-star-inserted' or @class='mat-calendar-body-cell mat-calendar-body-active ng-star-inserted') and @aria-label='" + exactDateFormat + "']")));
                clickDay.Click();

                logger.Info("Date set successful: " + date.ToString("dd-MMM-yyyy"));
                //*****day selection finished.
            }
            catch (Exception ex)
            {
                response.Message += "Statement generation not successful";
                logger.Info(response.Message);
                logger.Error("Error while setting date: " + date.ToString("dd-MMM-yyyy"));
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                response.ErrorMessage = "<span style='color:red'>" + ex.Message + "</span>";
            }
        }
    }
}