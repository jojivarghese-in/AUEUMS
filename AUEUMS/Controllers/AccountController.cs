using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AUEUMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using AUEUMS.Settings;
using Microsoft.Extensions.Options;
using AUEUMS.View_Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;


namespace AUEUMS.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly APISettings _APISettings;
        private readonly MailSettings _MailSettings;
        private readonly IWebHostEnvironment _env;
    
        public AccountController(IOptions<APISettings> APISettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env) :base(APISettings, MailSettings, env)
        {
            _APISettings = APISettings.Value;
            _MailSettings = MailSettings.Value;
            _env = env;
            
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Login login = new Login();
            login.ReturnUrl = "";
            return View("AUEUMSLogin", login);

        }

        [AllowAnonymous]
        public IActionResult AUEUMSLogin(string returnUrl)
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Login login = new Login();
            login.ReturnUrl = "";
            return View("AUEUMSLogin", login); 

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login login)
        {

            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            try
            {

                UserResource userResource = validateUser(login);
                string role = "guest";
                if (userResource.success == true)
                {
                    foreach (string roleL in userResource.Roles)
                    {
                        role = roleL;
                    }
                    if (userResource.designation == null)
                    {
                        userResource.designation = "NA";
                    }
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, userResource.FirstName),
                    new Claim(ClaimTypes.Email, userResource.Username),
                    new Claim("FullName", userResource.FirstName),
                    new Claim("Designation", userResource.designation),
                    new Claim("Name", userResource.FirstName),
                    new Claim(ClaimTypes.Role, role)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                        IsPersistent = false,
                    };

                    HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(claimsIdentity),
                       authProperties);


                    if (role == "Administrator")
                    {
                        return RedirectToAction("DashboardAdmin", "Admin");
                    }
                    else if (role == "Faculty")
                    {
                        return RedirectToAction("DashboardFaculty", "Admin");
                    }
                    else if (role == "Student")
                    {
                        return RedirectToAction("DashboardStudent", "Admin");
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return Redirect("/Account/AUEUMSLogin");
                    }

                }
            }
            catch (Exception ex)
            {
                login.errormessage = "Invalid User Credentials , Please Enter valid Email and Password";
                return View("AUEUMSLogin", login);

            }

            return View("AUEUMSLogin", login);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Response(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }
           
            return Redirect("/Account/AUEUMSLogin");


        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult UserAccessDenied()
        {
            return View();
        }


        public ActionResult logOut()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/AUEUMSLogin");
        }
        public UserResource validateUser(Login login)
        {

            using (var client = new HttpClient())
            {
                UserReg userReg = new UserReg();
                userReg.Username = login.Email;
                Custom.CustomSecurity security = new Custom.CustomSecurity();
                userReg.Password = security.Encrypt(login.Password,true);
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("Users/ValidateUserAsync", userReg).Result;
                if (response.IsSuccessStatusCode)
                {
                    UserResource UserResource = response.Content.ReadAsAsync<UserResource>().Result;
                    UserResource.success = true;
                    return UserResource;
                }
                else
                {
                    UserResource UserResource = new UserResource();
                    UserResource.success = false;
                    UserResource.message = "Invalid User Credentials , Please Enter valid Email and Password";
                    return UserResource;
                }
            }

        }        

    }
}