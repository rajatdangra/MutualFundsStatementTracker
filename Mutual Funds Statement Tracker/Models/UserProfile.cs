using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class UserProfile
    {
        public UserProfile(string email, string pan, string password)
        {
            Email = email;
            PAN = pan;
            Password = password;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + (string.IsNullOrWhiteSpace(FirstName) ? "" : " ") + LastName; } }
        public string Email { get; set; }
        public string PAN { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }
}