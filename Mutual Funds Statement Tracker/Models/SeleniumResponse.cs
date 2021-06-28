using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class SeleniumResponse
    {
        public SeleniumResponse()
        {
            IsSuccessfull = false;
            Message = string.Empty;
            ErrorMessage = string.Empty;
        }
        public bool IsSuccessfull;
        public string Message;
        public string ErrorMessage;
    }
}