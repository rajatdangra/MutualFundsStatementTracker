using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using NLog;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Mutual_Funds_Statement_Tracker
{
    //* Set intial parameters from Web.config/AppConfig
    public partial class MutualFundsStatementRequest : System.Web.UI.Page
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
                    email.Text = AppConfig.Email;
                    pan.Text = AppConfig.PAN;
                    password.Text = AppConfig.Password;
                }
                else
                {
                    //UserDetailsFormError.Visible = true;
                }
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
            UserDetailsFormError.Visible = false;
            UserDetailsFormError.InnerText = string.Empty;
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
                    string errorMsg = "Please fill all mandatory fields";
                    UserDetailsFormError.InnerText = "< span style = 'color:red' > " + errorMsg + " </ span >";
                    return;
                }
                else if (ValidateParameters())
                {
                    UserProfile user = new UserProfile(email.Text, pan.Text, password.Text) { FirstName = firstname.Text, LastName = lastname.Text, Phone = phone.Text };
                    logger.Info("User Details: " + user.ToString());
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

        private bool ValidateParameters()
        {
            bool isValid = true;
            string errorMsg = "";
            if (!IsValidEmail(email.Text))
            {
                isValid = false;
                errorMsg += email.Text + " is an Invalid Email Address.";
            }
            if (!IsValidPAN(pan.Text))
            {
                isValid = false;
                errorMsg += pan.Text + " is an Invalid PAN.";
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                UserDetailsFormError.Visible = true;
                UserDetailsFormError.InnerText = errorMsg;
                logger.Error(errorMsg);
            }
            return isValid;
        }
        private bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            return match.Success;
        }
        private bool IsValidPAN(string pan)
        {
            Regex regex = new Regex("([A-Z]){5}([0-9]){4}([A-Z]){1}$");
            Match match = regex.Match(pan);
            return match.Success;
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
                    fluentWait.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(AppConfig.TimeOut));
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(Convert.ToInt32(AppConfig.PollingTime));
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
                    SetCalendarDate(fluentWait, "mat-form-field-suffix ng-tns-c4-8 ng-star-inserted", new DateTime(1990, 1, 1), ref responseMsg);

                    //Set End Date
                    SetCalendarDate(fluentWait, "mat-form-field-suffix ng-tns-c4-9 ng-star-inserted", DateTime.Now, ref responseMsg);

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

        private void SetCalendarDate(DefaultWait<IWebDriver> fluentWait, string className, DateTime date, ref string responseMsg)
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
                responseMsg += "Statement generation not successful";
                logger.Info(responseMsg);
                logger.Error("Error while setting date: " + date.ToString("dd-MMM-yyyy"));
                logger.Error(ex.Message + "\nInner Ex: " + ex.InnerException);
                Response.Write("<span style='color:red'>" + ex.Message + "</span>");
            }
        }
    }

    public class UserProfile
    {
        public UserProfile(string email, string pan, string password)
        {
            Email = email;
            PAN = pan;
            Password = password;
        }
        public string FirstName;
        public string LastName;
        public string Email;
        public string PAN;
        public string Password;
        public string Phone;
    }
}