﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace AUEUMS.Code
{
    public enum ModalSize
    {
        Small,
        Large,
        Medium
    }
    public static class GoogleRecaptchaHelper
    {
        public static async Task<bool> IsReCaptchaPassedAsync(string gRecaptchaResponse, string secret)
        {
            HttpClient httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret) ,
                new KeyValuePair<string, string>("response",gRecaptchaResponse)
            });
            var res = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }
            return true;
        }
    }
}
