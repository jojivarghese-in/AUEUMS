using AUEUMS.Models;
using AUEUMS.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using AUEUMS.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Text;


namespace AUEUMS.Controllers
{
    [Authorize]
    public class BaseController: Controller
    {
        public string Baseurl = "";
        private readonly APISettings _APISettings;
        private readonly MailSettings _MailSettings;
        private readonly IWebHostEnvironment _env;
 
        public BaseController(IOptions<APISettings> APISettings, IOptions<MailSettings> MailSettings,IWebHostEnvironment env)
        {
            _APISettings = APISettings.Value;
            _MailSettings = MailSettings.Value;
            Baseurl = _APISettings.url;
            _env = env;
        }

        #region Ums Users
        public string getAccesssToken()
        {
          
            string role = HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role).Value;
            if(role == "guest")
            {
                return "-2";
            }

            if (HttpContext.Session.GetString("token") != null)
            {
                long expiry = Convert.ToInt64(HttpContext.Session.GetString("tokenExp"));
                if (DateTime.UtcNow.AddMinutes(1).Ticks >= expiry)
                {
                    using (var client = new HttpClient())
                    {
                        //refresh the access token using
                        client.BaseAddress = new Uri(Baseurl);
                        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                        client.DefaultRequestHeaders.Accept.Add(contentType);

                        RefreshToken refreshToken = new RefreshToken();
                        refreshToken.Token = HttpContext.Session.GetString("tokenRefresh");
                        refreshToken.userEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value;

                        string stringData = JsonConvert.SerializeObject(refreshToken);
                        var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                        HttpResponseMessage response = client.PostAsync("auth/RefreshToken", contentData).Result;
                        string stringJWT = response.Content.ReadAsStringAsync().Result;
                        AccessToken jwt = JsonConvert.DeserializeObject<AccessToken>(stringJWT);
                        HttpContext.Session.SetString("token", jwt.accessToken);
                        HttpContext.Session.SetString("tokenExp", jwt.expiration.ToString());
                        HttpContext.Session.SetString("tokenRefresh", jwt.refreshToken.ToString());
                        return jwt.accessToken;
                    }
                }
                else
                {
                    return HttpContext.Session.GetString("token");
                }
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);                 

                    CurrentUser userModel = new CurrentUser();
                    userModel.Username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value;
                  
                    Custom.Security security = new Custom.Security();
                    userModel.Password = security.GetPasswordEncrypt(true);

                    string stringData = JsonConvert.SerializeObject(userModel);
                    var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsJsonAsync("auth/login", userModel).Result;

                    string stringJWT = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        AccessToken jwt = JsonConvert.DeserializeObject<AccessToken>(stringJWT);
                        HttpContext.Session.SetString("token", jwt.accessToken);
                        HttpContext.Session.SetString("tokenExp", jwt.expiration.ToString());
                        HttpContext.Session.SetString("tokenRefresh", jwt.refreshToken.ToString());
                        return jwt.accessToken;
                    }
                    catch (Exception ex)
                    {
                        return "-1";                        
                    }
                }
            }
        }
       
        public NameValueMasterViewModel CreateNameValueMasterOrSelect(string TypeName, string DisplayValue)
        {
            NameValueMasterViewModel nameValueMasterViewModel;
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.
                   GetAsync("NameValueMaster/GetNameValueMasterByTypeAndValueAsync" + $"/{TypeName}" + $"/{DisplayValue}").Result;
                if (response.IsSuccessStatusCode)
                {
                    List<ReturnObject> ReturnObjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    nameValueMasterViewModel = ReturnObjectList[0].NameValueMasterViewModel;
                    if(nameValueMasterViewModel == null)
                    {
                        //create it
                        nameValueMasterViewModel = createNameValueMaster(TypeName, DisplayValue);
                        return nameValueMasterViewModel;
                    }
                    else
                    {
                        return nameValueMasterViewModel;
                    }
                }
                else
                {                    
                    return null;
                }
            }
        }
        public NameValueMasterViewModel  createNameValueMaster(string TypeName, string DisplayValue)
        {
            if (DisplayValue == null || DisplayValue == "")
                return null;
            NameValueMasterViewModel nameValueMasterViewModel = new NameValueMasterViewModel();
            nameValueMasterViewModel.TypeName = TypeName;
            nameValueMasterViewModel.DisplayValue = DisplayValue;
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                response = client.PostAsJsonAsync("NameValueMaster/AddNameValueMasterAsync", nameValueMasterViewModel).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return null;
                }
                List<ReturnObject> ProjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    if (ProjectList[0].NameValueMasterViewModel == null)
                    {
                        return null;
                    }
                    else
                    {
                        return ProjectList[0].NameValueMasterViewModel;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public List<NameValueMasterViewModel> GetNameValueList(string TypeName)
        {
            List<NameValueMasterViewModel> nameValueMasterViewModels = new List<NameValueMasterViewModel>();
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                //string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.
                   GetAsync("NameValueMaster/GetNameValueMastersByTypeAsync" + $"/{TypeName}").Result;                
                if (response.IsSuccessStatusCode)
                {
                    List<ReturnObject> ReturnObjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    nameValueMasterViewModels = ReturnObjectList[0].NameValueMasterViewModels;
                    return nameValueMasterViewModels;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public List<UserModel> GetUsers()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Users/GetAll").Result;
                List<UserModel> lstUsers = response.Content.
                             ReadAsAsync<List<UserModel>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    List<UserModel> lstUsersSales = new List<UserModel>();
                    foreach(UserModel userModel in lstUsers)
                    {
                        if(userModel.Roles.Contains("Sales_Owner"))
                        {
                            lstUsersSales.Add(userModel);
                        }
                    }
                    return lstUsersSales;
                }
                else
                {                   
                    return null;
                }
            }
        }
        public UserModel GetUserById(int userid)
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Users/GetById" + $"/{userid}").Result;
                UserModel lstUserSales = response.Content.
                             ReadAsAsync<UserModel>().Result;
                if (response.IsSuccessStatusCode)
                {                    
                    return lstUserSales;
                }
                else
                {
                    return null;
                }
            }
        }
        public UserModel GetUserByEmail(string email)
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Users/GetUserByEmail" + $"/{email}").Result;
                UserModel lstUserSales = response.Content.
                             ReadAsAsync<UserModel>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return lstUserSales;
                }
                else
                {
                    return null;
                }
            }
        }
        public User UpdateUser()
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                UserUpdateModel userUpdateModel = new UserUpdateModel();
                string imageUrl = "";
                if (User.Claims.Where(x => x.Type == "urn:google:picture").Count() != 0)
                {
                    imageUrl = User.Claims.Where(x => x.Type == "urn:google:picture").FirstOrDefault().Value;
                }
                else
                {
                    imageUrl = "images/avatars/1.jpg";
                }
                userUpdateModel.ImageUrl = imageUrl;
                userUpdateModel.LastLoggedIn = DateTime.Now;
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                response = client.PostAsJsonAsync("Users/Update", userUpdateModel).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return null;
                }
                
                return null;
              
            }
        }
        public ReturnObject GetAUEUMSRegisterByRef(long UsrID)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSRegisteredUserByRefAsync" + $"/{UsrID}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public ReturnObject GetAUEUMSRegisters()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSRegisteredUsersAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public ReturnObject GetAUEUMSRegistersAll()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSRegisteredUsersAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public ReturnObject GetAUEUMSRegisterByEmail(string email)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSRegisteredUserByEmail" + $"/{email}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public ReturnObject GetUserForApprovalByRef(long UsrID)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSRegisteredUserByRefAsync" + $"/{UsrID}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }


        #endregion


        #region RoleList

        public List<RoleListViewModel> GetRoleListAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetRoleListAllAsync").Result;
                List<ReturnObject> ReturnObjectList = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                
                List<RoleListViewModel> rolelst = new List<RoleListViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    rolelst = ReturnObjectList[0].rolelistViewModels;
                    return rolelst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        #endregion

        #region Faculty Assignments
        public ReturnObject GetAssignments()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Faculty/GetAssignmentsAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject GetAssignmentsAll()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Faculty/GetAssignmentsAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject GetAssignmentsByRef(long AssignmentsId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Faculty/GetAssignmentsByRefAsync" + $"/{AssignmentsId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject StudentAnsweredAssignments()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Faculty/StudentAnsweredAssignmentsAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        #endregion

        #region Student Assignments

        public ReturnObject GetAssignmentsForStudents()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Student/GetAssignmentsForStudentsAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject GetAssignmentsForStudentsAll()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Student/GetAssignmentsForStudentsAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject GetAssignmentsForStudentsByRef(long AssignmentsId,long StudentId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Student/GetAssignmentsForStudentsByRef" + $"/{AssignmentsId}" + $"/{StudentId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject GetStudentAssignmentsByRef(long AssignmentsId, long StudentId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Student/GetStudentAssignmentsByRef" + $"/{AssignmentsId}" + $"/{StudentId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public ReturnObject StudentAnsweredAssignmentsByRef(long AssignmentsId, long StudentId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Faculty/StudentAnsweredAssignmentsByRef" + $"/{AssignmentsId}" + $"/{StudentId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }



        #endregion

        #region College Programs
        public List<CollegeViewModel> GetCollegeNameAll()
        {
            List<CollegeViewModel> CollegeViewModels = new List<CollegeViewModel>();
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.
                   GetAsync("Admin/GetCollegeNameAllAsync").Result;
                List<ReturnObject> ReturnObjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    CollegeViewModels = ReturnObjectList[0].collegeViewModels;
                    return CollegeViewModels;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public List<CollegeProgramViewModel> GetCollegeProgramAll()
        {
            List<CollegeProgramViewModel> CollegeProgramViewModels = new List<CollegeProgramViewModel>();
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.
                   GetAsync("Admin/GetCollegeProgramAllAsync").Result;
                List<ReturnObject> ReturnObjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    CollegeProgramViewModels = ReturnObjectList[0].collegeProgramViewModels;
                    return CollegeProgramViewModels;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public List<ProgramMajorViewModel> GetProgramMajorAll()
        {
            List<ProgramMajorViewModel> ProgramMajorViewModels = new List<ProgramMajorViewModel>();
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.
                   GetAsync("Admin/GetProgramMajorAllAsync").Result;
                List<ReturnObject> ReturnObjectList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    ProgramMajorViewModels = ReturnObjectList[0].programMajorViewModels;
                    return ProgramMajorViewModels;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        #endregion
        #region Timetable
        public ReturnObject TimetableListAll()
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/TimetableListAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject TimetableDetailListAll(long tHeaderId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/TimetableDetailListAllAsync" + $"/{tHeaderId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public List<RoomViewModel> GetRoomsAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetRoomsAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<RoomViewModel> roomlst = new List<RoomViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    roomlst= ReturnObject[0].RoomViewModels;
                    return roomlst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }
        public List<YearViewModel> GetYearAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetYearsAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<YearViewModel> yearlst = new List<YearViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    yearlst = ReturnObject[0].YearViewModels;
                    return yearlst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }
        public List<SemesterTypeViewModel> GetSemesterTypeAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetSemesterTypeAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<SemesterTypeViewModel> semsterTypelst = new List<SemesterTypeViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    semsterTypelst = ReturnObject[0].SemesterTypeViewModels;
                    return semsterTypelst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public ReturnObject GetTimetableHdrEditByRef(long TimetableHdrId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetTimetableHeaderByRefAsync" + $"/{TimetableHdrId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public List<DaysViewModel> GetDaysAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetDaysAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<DaysViewModel> dayslst = new List<DaysViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    dayslst = ReturnObject[0].daysViewModels;
                    return dayslst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public List<TimeSlotViewModel> GetTimeSlotAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetTimeSlotAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<TimeSlotViewModel> timeslotlst = new List<TimeSlotViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    timeslotlst = ReturnObject[0].timeSlotViewModels;
                    return timeslotlst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public List<CourseSectionViewModel> GetCourseSectionAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetCourseSectionAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<CourseSectionViewModel> courseseclst = new List<CourseSectionViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    courseseclst = ReturnObject[0].courseSectionViewModels;
                    return courseseclst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public ReturnObject GetCourseByRef(long CourseId)
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetCourseByRefAsync" + $"/{CourseId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }
        public List<CourseViewModel> GetCourseAll()
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetCourseAllAsync").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<CourseViewModel> courselst = new List<CourseViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    courselst = ReturnObject[0].courseViewModels;
                    return courselst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public List<CourseSectionViewModel> GetCourseSectionByRef(long CourseId)
        {
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetCourseSectionByRef" + $"/{CourseId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                List<CourseSectionViewModel> courseseclst = new List<CourseSectionViewModel>();
                if (response.IsSuccessStatusCode)
                {
                    courseseclst = ReturnObject[0].courseSectionViewModels;
                    return courseseclst;
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }


        }

        public ReturnObject GetTTDetailsByRef(long HdrId, long DtlId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetTimetableDetailsByRefAsync" + $"/{HdrId}" + $"/{DtlId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject ValidateTimetableDetails(long paramHrdId, long CourselistId, long CourseSeclistId, long TimeSlotlistId, long DayslistId)
        {

            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return null;
                }
                else if (accessToken == "-2")
                {
                    return null;
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/ValidateTimetableDetailsByRefAsync" + $"/{paramHrdId}" + $"/{CourselistId}" + $"/{CourseSeclistId}" + $"/{TimeSlotlistId}" + $"/{DayslistId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    return ReturnObject[0];
                }
                else
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return null;
                }
            }
        }

        public ReturnObject ValidateTimetable([FromBody] TimetableDetailViewModel timetableDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response;

                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    response = client.PostAsJsonAsync("Admin/ValidateTimetableDetailsByRefAsync", timetableDetailViewModel).Result;
                    List<ReturnObject> TListData = response.Content.ReadAsAsync<List<ReturnObject>>().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return TListData[0];
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-4";
                        return returnObject;
                    }
                }
            }
            else
            {
                return null;
            }
        }



        #endregion
    }
}
