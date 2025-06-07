using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AUEUMS.Settings
{
    public class APISettings
    {
        public string url { get; set; }
    }

    public class GoogleOAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
   


}
