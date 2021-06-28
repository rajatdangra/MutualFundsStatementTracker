using Mutual_Funds_Statement_Tracker.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker
{
    public class AppConfig
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal static string Email => Convert.ToString(ConfigurationManager.AppSettings["Email"]);
        internal static string Password => Convert.ToString(ConfigurationManager.AppSettings["Password"]);
        internal static string PAN => Convert.ToString(ConfigurationManager.AppSettings["PAN"]);
        internal static string Phone => Convert.ToString(ConfigurationManager.AppSettings["Phone"]);
        internal static string FirstName => Convert.ToString(ConfigurationManager.AppSettings["FirstName"]);
        internal static string LastName => Convert.ToString(ConfigurationManager.AppSettings["LastName"]);
        internal static string FullName => FirstName + (string.IsNullOrWhiteSpace(FirstName) ? "" : " ") + LastName;
        internal static string CookieExpiry => Convert.ToString(ConfigurationManager.AppSettings["CookieExpiry"]);
        internal static bool SaveUserDetails => String.Equals(ConfigurationManager.AppSettings["SaveUserDetails"], "1");
        internal static string PollingTime => Convert.ToString(ConfigurationManager.AppSettings["PollingTime"]);
        internal static string TimeOut => Convert.ToString(ConfigurationManager.AppSettings["TimeOut"]);
        internal static string RTA_URL => Convert.ToString(ConfigurationManager.AppSettings["RTA_URL"]);
        internal static bool IsAutomated => String.Equals(ConfigurationManager.AppSettings["IsAutomated"], "1");
        internal static string AggregatorEmailIds => Convert.ToString(ConfigurationManager.AppSettings["AggregatorEmailIds"]);
        internal static string Mail_Subject => Convert.ToString(ConfigurationManager.AppSettings["Mail_Subject"]);
        internal static string Retry_Count => Convert.ToString(ConfigurationManager.AppSettings["Retry_Count"]);
        internal static string Sync_Frequency => Convert.ToString(ConfigurationManager.AppSettings["Sync_Frequency"]);
        internal static bool IsForceToHttps => Convert.ToBoolean(ConfigurationManager.AppSettings["ForceToHttps"]);
        internal static string Port => Convert.ToString(ConfigurationManager.AppSettings["Port"]);

        public static void UpdateConfig(UserProfile defaultUserSettings)
        {
            logger.Info("UpdateConfig started");
            try
            {
                System.Configuration.Configuration configFile = null;
                if (System.Web.HttpContext.Current != null)
                {
                    configFile =
                        System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                }
                else
                {
                    configFile =
                        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }
                var settings = configFile.AppSettings.Settings;

                Dictionary<string, string> appSettings = new Dictionary<string, string>();
                appSettings.Add("Email", defaultUserSettings.Email);
                appSettings.Add("PAN", defaultUserSettings.PAN);
                appSettings.Add("FirstName", defaultUserSettings.FirstName);
                appSettings.Add("Lastname", defaultUserSettings.LastName);
                appSettings.Add("Phone", defaultUserSettings.Phone);
                appSettings.Add("Password", defaultUserSettings.Password);

                bool change = false;
                foreach (var kv in appSettings)
                {
                    var key = kv.Key;
                    var value = kv.Value;
                    if (settings[key] == null)
                    {
                        logger.Warn("While reading the web.config, this line had no key/value attributes modify: " + key);
                        //settings.Add(key, value);
                    }
                    else if (settings[key].Value != value)
                    {
                        change = true;
                        settings[key].Value = value;
                    }
                }
                if (change)
                {
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                    logger.Info("Updated web.config file");
                }
                else
                {
                    logger.Info("No need to update web.config file because appsetting are accurate in web.config file");
                }
            }
            catch (Exception ex)
            {
                logger.Info("Unable to update web.config file" + "\n" + ex);
            }
            finally
            {
                logger.Info("UpdateConfig end");
            }
        }
    }
}