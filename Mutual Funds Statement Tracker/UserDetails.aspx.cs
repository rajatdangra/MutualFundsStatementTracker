using System;
using System.Threading;
using NLog;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Collections.Specialized;
using System.Web.UI;
using System.Threading.Tasks;
using Mutual_Funds_Statement_Tracker.Models;
using System.Diagnostics;

namespace Mutual_Funds_Statement_Tracker
{
    //* Set intial parameters from Web.config/AppConfig
    public partial class MutualFundsStatementRequest : System.Web.UI.Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string cookieName = "UserDetails";

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

                    //Initialize values
                    bool isDataInitialized = false;
                    HttpCookie cookie = GetCookies();
                    if (cookie != null)
                    {
                        string decryptedData = AESEncryption.AESDecryptText(cookie["Data"]);
                        if (decryptedData != null)
                        {
                            isDataInitialized = true;
                            logger.Info(decryptedData);
                            NameValueCollection Params = new NameValueCollection();
                            string[] segments = decryptedData.Split('&');
                            foreach (string seg in segments)
                            {
                                string[] parts = seg.Split('=');
                                if (parts.Length > 1)
                                {
                                    string Key = parts[0].Trim();
                                    string Value = parts[1].Trim();
                                    Params.Add(Key, Value);
                                }
                            }

                            email.Text = Params.Get("Email");
                            pan.Text = Params.Get("PAN");
                            password.Text = Params.Get("Password");
                            password.Attributes["value"] = password.Text;
                            firstname.Text = Params.Get("FirstName");
                            lastname.Text = Params.Get("LastName");
                            phone.Text = Params.Get("Phone");
                        }
                        saveUserDetails.Checked = AppConfig.SaveUserDetails;
                    }

                    if (!isDataInitialized && (Debugger.IsAttached || !AppConfig.SkipDefaultValues))
                    {
                        email.Text = AppConfig.Email;
                        pan.Text = AppConfig.PAN;
                        password.Text = AppConfig.Password;
                        password.Attributes["value"] = password.Text;
                        firstname.Text = AppConfig.FirstName;
                        lastname.Text = AppConfig.LastName;
                        phone.Text = AppConfig.Phone;
                        saveUserDetails.Checked = AppConfig.SaveUserDetails;
                    }
                    if (AppConfig.IsAutomated)
                    {
                        Task.Run(() => AutoSubmit());
                    }
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

        public void AutoSubmit()
        {
            logger.Info("Automatic Process. Waiting for 5 seconds before auto submit.");
            //Wait for 5 seconds
            Thread.Sleep(5000);
            OnSubmitButtonClicked(null, null);
            AutoCloseBrowser();
        }

        public void AutoCloseBrowser()
        {
            logger.Info("Automatic Process. Waiting for 5 seconds before closing.");
            //Wait for 5 seconds
            Thread.Sleep(5000);
            ClientScript.RegisterStartupScript(typeof(Page), "closePage", "window.close();", true);
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
                    SetCookies();
                    UserDetailsFormError.Visible = false;
                    if (saveUserDetails.Checked)
                    {
                        logger.Info("Saving User Details");
                        AppConfig.UpdateConfig(user);
                        logger.Info("Saved User Details");
                    }

                    logger.Info("Details Submitted. Navigating to RTA website.");

                    var response = new SeleniumAutomation(rta_url.Value).Navigate(user);

                    UserDetailsResponse.InnerText = response.Message;
                    UserDetailsResponse.Visible = true;

                    if (!response.IsSuccessfull)
                        Response.Write(response.ErrorMessage);
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
                if (!string.IsNullOrWhiteSpace(errorMsg))
                    errorMsg += "\n";
                errorMsg += $"{email.Text} is an Invalid Email Address. Sample Email format: {AppConfig.SampleEmail}.";
            }
            if (!IsValidPAN(pan.Text))
            {
                isValid = false;
                if (!string.IsNullOrWhiteSpace(errorMsg))
                    errorMsg += "\n";
                errorMsg += $"{pan.Text} is an Invalid PAN. Sample PAN format: {AppConfig.SamplePAN}.";
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

        private void SetCookies()
        {
            try
            {
                logger.Info("Set cookies called.");
                StringBuilder data = new StringBuilder();
                data.Append("SessionID" + "=" + Session.SessionID + "&");
                data.Append("Email" + "=" + email.Text + "&");
                data.Append("PAN" + "=" + pan.Text + "&");
                data.Append("Password" + "=" + password.Text + "&");
                data.Append("FirstName" + "=" + firstname.Text + "&");
                data.Append("LastName" + "=" + lastname.Text + "&");
                data.Append("Phone" + "=" + phone.Text + "&");
                var encryptedData = AESEncryption.AESEncryptText(Convert.ToString(data));

                HttpCookie userInfo = new HttpCookie(cookieName);
                userInfo.Secure = true;
                //userInfo["SessionID"] = Session.SessionID;
                //userInfo["Email"] = email.Text;
                //userInfo["PAN"] = pan.Text;
                //userInfo["Password"] = password.Text;
                //userInfo["FirstName"] = firstname.Text;
                //userInfo["LastName"] = lastname.Text;
                //userInfo["Phone"] = phone.Text;
                userInfo["Data"] = encryptedData;
                userInfo.Expires = DateTime.Now.AddMonths(Convert.ToInt32(AppConfig.CookieExpiry));
                Response.Cookies.Add(userInfo);
                logger.Info("Data: " + Convert.ToString(encryptedData));
            }
            catch (Exception ex)
            {
                logger.Error("Unable to set cookies.\n" + ex);
            }
        }
        private HttpCookie GetCookies()
        {
            return Request.Cookies[cookieName];
        }
    }
}