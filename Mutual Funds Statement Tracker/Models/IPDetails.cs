using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Web;

namespace Mutual_Funds_Statement_Tracker.Models
{
    public class IPDetails
    {
        public IPDetails()
        {
        }

        public string GetLocalIPv4()
        {
            return GetLocalIPv4Details().ToString();
        }

        private IPAddress GetLocalIPv4Details()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}