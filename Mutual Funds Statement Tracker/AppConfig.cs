using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker
{
    public class AppConfig
    {
        internal static string Email => Convert.ToString(ConfigurationManager.AppSettings["Email"]);
        internal static string Password => Convert.ToString(ConfigurationManager.AppSettings["Password"]);
        internal static string PAN => Convert.ToString(ConfigurationManager.AppSettings["PAN"]);
        internal static string Phone => Convert.ToString(ConfigurationManager.AppSettings["Phone"]);
        internal static string FirstName => Convert.ToString(ConfigurationManager.AppSettings["FirstName"]);
        internal static string LastName => Convert.ToString(ConfigurationManager.AppSettings["LastName"]);
        internal static bool SaveUserDetails => String.Equals(ConfigurationManager.AppSettings["SaveUserDetails"], "1");
        internal static string PollingTime => Convert.ToString(ConfigurationManager.AppSettings["PollingTime"]);
        internal static string TimeOut => Convert.ToString(ConfigurationManager.AppSettings["TimeOut"]);
        internal static string RTA_URL => Convert.ToString(ConfigurationManager.AppSettings["RTA_URL"]);
        internal static bool IsAutomated => String.Equals(ConfigurationManager.AppSettings["IsAutomated"], "1");
        //internal static bool IsAutomated => Convert.ToBoolean(ConfigurationManager.AppSettings["IsAutomated"]); //Use this for values = "true" or "false"
        internal static string AggregatorEmailIds => Convert.ToString(ConfigurationManager.AppSettings["AggregatorEmailIds"]);
        internal static string Mail_Subject => Convert.ToString(ConfigurationManager.AppSettings["Mail_Subject"]);
        internal static string Retry_Count => Convert.ToString(ConfigurationManager.AppSettings["Retry_Count"]);
        internal static string Sync_Frequency => Convert.ToString(ConfigurationManager.AppSettings["Sync_Frequency"]);
    }
}