using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using NLog;
using OpenQA.Selenium.Support.UI;

namespace Mutual_Funds_Statement_Tracker
{
    //* Set intial parameters from Web.config/AppConfig
    public partial class UserDetails : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            logger.Info("Process Started");
            try
            {
                rta_url.Value = AppConfig.RTA_URL;

                if (!IsPostBack)
                {
                    UserDetailsFormError.Visible = false; // error form
                    UserDetailsResponse.Visible = false;
                }
                else
                {
                    //UserDetailsFormError.Visible = true;
                }
                email.Text = AppConfig.Email;
                pan.Text = AppConfig.PAN;
                password.Text = AppConfig.Password;
            }
            catch (Exception ex)
            {
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }

        protected void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            UserDetailsResponse.Visible = false;
            UserDetailsResponse.InnerText = string.Empty;
            try
            {
                if (
                        string.IsNullOrEmpty(email.Text) ||
                        string.IsNullOrEmpty(pan.Text) ||
                        string.IsNullOrEmpty(password.Text)
                        )
                {
                    //error
                    logger.Error("Missing user details.");
                    UserDetailsFormError.Visible = true;
                    return;
                }
                else
                {
                    logger.Info("Details Submitted. Navigating to RTA website.");
                    UserDetailsFormError.Visible = false;
                    string responseMessage = string.Empty;
                    Navigate(rta_url.Value, out responseMessage);
                    UserDetailsResponse.InnerText = responseMessage;
                    UserDetailsResponse.Visible = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }

        private void Navigate(String url, out string responseMsg)
        {
            responseMsg = "[" + DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt") + "]: ";
            if (String.IsNullOrEmpty(url)) return;
            if (url.Equals("about:blank")) return;
            if (!url.StartsWith("http://") &&
                !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
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

                IWebDriver driver = new ChromeDriver(/*co*/);
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);

                logger.Info("Navigate to RTA url successful");

                try
                {
                    logger.Info("Automation on RTA url started, setting all inputs.");

                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(10);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(500);
                    /* Ignore the exception - NoSuchElementException that indicates that the element is not present */
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                    fluentWait.IgnoreExceptionTypes(typeof(ElementNotVisibleException));
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
                    //SetCalendarDate(driver, "Open calender", new DateTime(1990, 1, 1));

                    //Set End Date
                    //SetCalendarDate(driver, "Open calender", DateTime.Now);

                    IWebElement withZeroRadioClick = fluentWait.Until(a => a.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-9' and @value = 'Y']")));
                    if (withZeroRadioClick != null)
                        withZeroRadioClick.Click();

                    IWebElement emailWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-0' and @placeholder='Email']")));
                    if (emailWE != null)
                        emailWE.SendKeys(email.Text);

                    IWebElement panWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-1' and @placeholder='PAN']")));
                    if (panWE != null)
                        panWE.SendKeys(pan.Text);

                    IWebElement passwordWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-2' and @placeholder='Password']")));
                    if (passwordWE != null)
                        passwordWE.SendKeys(password.Text);

                    IWebElement confirmPasswordWE = fluentWait.Until(a => a.FindElement(By.XPath("//input[@Id='mat-input-3' and @placeholder='Confirm Password']")));
                    if (confirmPasswordWE != null)
                        confirmPasswordWE.SendKeys(password.Text);

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
                        responseMsg += successRefNoResponse;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    responseMsg += "Statement generation not successful";
                    logger.Info(responseMsg);
                    logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                    Response.Write("<span style='color:red'>" + ex.Message + "</span>");
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
                responseMsg += "Statement generation not successful";
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
            catch (Exception ex)
            {
                responseMsg += "Statement generation not successful";
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }

        private void SetCalendarDate(IWebDriver driver, string id, DateTime date)
        {
            //*****Day selection started.
            //Click Datepicker
            driver.FindElement(By.Id(id)).Click();
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();
            //driver.FindElements(By.CssSelector("button[data-pika-year=" + year +"][data-pika-month=" + month + "][data-pika-day=" + day+ "]")).get(0).click();
            //*****day selection finished.

            //Check the operation in 1 seconds
            Thread.Sleep(1000);
        }
    }
}