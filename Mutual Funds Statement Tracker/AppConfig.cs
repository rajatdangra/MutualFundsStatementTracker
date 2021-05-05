using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker
{
    public class AppConfig
    {
        public static string Email => Convert.ToString(ConfigurationManager.AppSettings["Email"]);
        public static string Password => Convert.ToString(ConfigurationManager.AppSettings["Password"]);
        public static string PAN => Convert.ToString(ConfigurationManager.AppSettings["PAN"]);
        public static string PollingTime => Convert.ToString(ConfigurationManager.AppSettings["PollingTime"]);
        public static string TimeOut => Convert.ToString(ConfigurationManager.AppSettings["TimeOut"]);
        public static string RTA_URL => Convert.ToString(ConfigurationManager.AppSettings["RTA_URL"]);
        public static string IsAutomated => Convert.ToString(ConfigurationManager.AppSettings["IsAutomated"]);
        public static string AggregatorEmailIds => Convert.ToString(ConfigurationManager.AppSettings["AggregatorEmailIds"]);
        public static string Mail_Subject => Convert.ToString(ConfigurationManager.AppSettings["Mail_Subject"]);
        public static string Retry_Count => Convert.ToString(ConfigurationManager.AppSettings["Retry_Count"]);
        public static string Sync_Frequency => Convert.ToString(ConfigurationManager.AppSettings["Sync_Frequency"]);
    }
}