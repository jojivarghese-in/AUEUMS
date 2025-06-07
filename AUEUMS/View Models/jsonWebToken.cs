using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.View_Models
{
    public class AccessToken
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public long expiration { get; set; }
    }
    public class CRMAccessToken
    {
        public string token_type { get; set; }
        public string scope { get; set; }
        public string expires_in { get; set; }
        public string ext_expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }
    public class RefreshToken
    {
        public string Token { get; set; }
        public string userEmail { get; set; }
        public long expiration { get; set; }
    }
    public class CurrentUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CRMCurrentUser
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string resource { get; set; }
    }
}
