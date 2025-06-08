using AUEUMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AUEUMS.View_Models;
using Microsoft.VisualBasic.CompilerServices;
using System.Globalization;
using System.Linq;
using AUEUMS.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Text;


namespace AUEUMS.Controllers
{
    public class AdminController : BaseController
    {
        private readonly APISettings _APISettings;
        private readonly MailSettings _MailSettings;
        private readonly IWebHostEnvironment _env;
   

        public AdminController(IOptions<APISettings> APISettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env) : base(APISettings,MailSettings,env)
        {
            _APISettings = APISettings.Value;
            _MailSettings = MailSettings.Value;
            _env = env;
           
        }
        public ActionResult DashboardAdmin()
        {
            string accessToken = getAccesssToken();

            if (accessToken == "-1")
            {
                return RedirectToAction("login", "Account", new { returnUrl = "Admin/DashboardAdmin" });
            }
            else if (accessToken == "-2")
            {
                return Redirect("/Account/UserAccessDenied");
            }
            else
            {
                UpdateUser();
                DashboardView dashboardView = new DashboardView();
                UserModel user = GetUserByEmail(HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value);
                return View("DashboardAdmin", dashboardView);
            }
        }
        public ActionResult DashboardFaculty()
        {
            string accessToken = getAccesssToken();

            if (accessToken == "-1")
            {
                return RedirectToAction("login", "Account", new { returnUrl = "Admin/DashboardFaculty" });
            }
            else if (accessToken == "-2")
            {
                return Redirect("/Account/UserAccessDenied");
            }
            else
            {
                UpdateUser();
                DashboardView dashboardView = new DashboardView();
                UserModel user = GetUserByEmail(HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value);
                dashboardView.Companies = null;
                return View("DashboardFaculty", dashboardView);
            }
        }
        public ActionResult DashboardStudent()
        {
            string accessToken = getAccesssToken();

            if (accessToken == "-1")
            {
                return RedirectToAction("login", "Account", new { returnUrl = "Admin/DashboardStudent" });
            }
            else if (accessToken == "-2")
            {
                return Redirect("/Account/UserAccessDenied");
            }
            else
            {
                UpdateUser();
                DashboardView dashboardView = new DashboardView();
                UserModel user = GetUserByEmail(HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value);
                return View("DashboardStudent", dashboardView);
            }
        }
        public List<Company> GetCompanies()
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
                     GetAsync("Admin/GetCompanieseAsync").Result;
                List<ReturnObject> ReturnObjectList = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                //ProjectInfo.Insert(0, new Project { ProjectId = 0, ProjectName = "Select" });
                List<Company> CompanylST = new List<Company>();
                if (response.IsSuccessStatusCode)
                {
                    CompanylST = ReturnObjectList[0].Companies;
                    return CompanylST;
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
        public IActionResult DefaultQuick()
        {
            return PartialView("DefaultQuickView");
        }
        public ActionResult Dashboard()
        {
            string accessToken = getAccesssToken();

            if (accessToken == "-1")
            {
                return RedirectToAction("login", "Account", new { returnUrl = "Admin/Dashboard" });
            }
            else if (accessToken == "-2")
            {
                return Redirect("/Account/UserAccessDenied");
            }
            else
            {
                UpdateUser();
                DashboardView dashboardView = new DashboardView();
                UserModel user = GetUserByEmail(HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value);
              
                return View("Dashboard", dashboardView);
            }
        }
        #region Dashboard

        public IActionResult LoadDashboard(string mode, string ctrlid)
        {
            DashboardView dashViewModel = new DashboardView();

            return PartialView("DashboardAdmin", dashViewModel);


        }
        public IActionResult GetReportAssignmentList(int AssignmentId, int number)
        {
            if (number == 0)
                number = 200;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return PartialView("ExcpetionPartial");
                }
                else if (accessToken == "-2")
                {
                    return PartialView("ExcpetionPartial");
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetReportAssignmentListAsync" + $"/{number}" + $"/{AssignmentId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    AssignmentListDashboardViewModel reportView = new AssignmentListDashboardViewModel();
                    reportView = ReturnObject[0].assignmentListDashboardViewModel;

                    if (AssignmentId == -1)
                    {
                        return PartialView("TreeByAssignment", reportView);
                    }
                    else
                    {
                        return PartialView("TreeByAssignment", null);
                    }                    
                }
                else
                {
                    return PartialView("ExcpetionPartial");
                }
            }
        }
        public IActionResult GetFacultyReportAssignmentList(int AssignmentId, int number)
        {
            if (number == 0)
                number = 200;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return PartialView("ExcpetionPartial");
                }
                else if (accessToken == "-2")
                {
                    return PartialView("ExcpetionPartial");
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetFacultyReportAssignmentListAsync" + $"/{number}" + $"/{AssignmentId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    AssignmentListDashboardViewModel reportView = new AssignmentListDashboardViewModel();
                    reportView = ReturnObject[0].assignmentListDashboardViewModel;

                    if (AssignmentId == -1)
                    {
                        return PartialView("TreeByFacultyAssignment", reportView);
                    }
                    else
                    {
                        return PartialView("TreeByFacultyAssignment", null);
                    }
                }
                else
                {
                    return PartialView("ExcpetionPartial");
                }
            }
        }
        public IActionResult GetReportStudentAssignments(int usrId, int number)
        {
            if (number == 0)
                number = 200;
            using (var client = new HttpClient())
            {
                if (usrId == 0)
                    usrId = -1;

                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return PartialView("ExcpetionPartial");
                }
                else if (accessToken == "-2")
                {
                    return PartialView("ExcpetionPartial");
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetReportStudentAssignmentsAsync" + $"/{number}" + $"/{usrId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    DashboardReportStudentAssignmentsList reportView = new DashboardReportStudentAssignmentsList();

                    reportView = ReturnObject[0].ReportStudentAssignmentsList;

                    if (usrId == -1)
                        return PartialView("TBVewByUser", reportView);


                    return PartialView("TBVewByUser", null);
                }
                else
                {
                    return PartialView("ExcpetionPartial");
                }
            }

            return PartialView("TBVewByUser", null);

        }
        public IActionResult GetReportAUEUMSActiveUsers(int usrId, int number)
        {
            if (number == 0)
                number = 200;
            using (var client = new HttpClient())
            {
                if (usrId == 0)
                    usrId = -1;

                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return PartialView("ExcpetionPartial");
                }
                else if (accessToken == "-2")
                {
                    return PartialView("ExcpetionPartial");
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAUEUMSUserListByModifiedDateAsync" + $"/{number}" + $"/{usrId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    AUEUMSUserListDashboardViewModel reportView = new AUEUMSUserListDashboardViewModel();

                    reportView = ReturnObject[0].AUEUMSUserListDashboardViewModel;

                    if (usrId == -1)
                        return PartialView("TBVewByUserManagerAdmin", reportView);


                    return PartialView("TBVewByUserManagerAdmin", null);
                }
                else
                {
                    return PartialView("ExcpetionPartial");
                }
            }

            return PartialView("TBVewByUser", null);

        }
        public IActionResult GetReportAUEUMSActiveStudentUsers(int usrId, int number)
        {
            if (number == 0)
                number = 200;
            using (var client = new HttpClient())
            {
                if (usrId == 0)
                    usrId = -1;

                string accessToken = getAccesssToken();
                if (accessToken == "-1")
                {
                    return PartialView("ExcpetionPartial");
                }
                else if (accessToken == "-2")
                {
                    return PartialView("ExcpetionPartial");
                }
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetReportAUEUMSActiveStudentUsersAsync" + $"/{number}" + $"/{usrId}").Result;
                List<ReturnObject> ReturnObject = response.Content.
                             ReadAsAsync<List<ReturnObject>>().Result;
                if (response.IsSuccessStatusCode)
                {
                    AUEUMSUserListDashboardViewModel reportView = new AUEUMSUserListDashboardViewModel();

                    reportView = ReturnObject[0].AUEUMSUserListDashboardViewModel;

                    if (usrId == -1)
                        return PartialView("TBVewByUserManager", reportView);


                    return PartialView("TBVewByUserManager", null);
                }
                else
                {
                    return PartialView("ExcpetionPartial");
                }
            }

            return PartialView("TBVewByUser", null);

        }

        #endregion


        #region AUEUMS Users
        public IActionResult AUEUMSRegisters(string mode, string ctrlid)
        {
            ReturnObject returnObject = new ReturnObject();
            returnObject = GetAUEUMSRegisters();
            if (returnObject.AUEUMSUserViewModels != null)
            {
                List<AUEUMSRegisterViewModel> AUEUMSRegisterlist = returnObject.AUEUMSUserViewModels;
                AUEUMSRegisterListViewModel usrListViewModel = new AUEUMSRegisterListViewModel();
                usrListViewModel.registers = AUEUMSRegisterlist;
                usrListViewModel.mode = mode;
                usrListViewModel.ctrlid = ctrlid;

                return PartialView("UserManagerList", usrListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }


        }
        public IActionResult AUEUMSRegisterAll(string mode, string ctrlid)
        {
            ReturnObject returnObject = GetAUEUMSRegistersAll();
            if (returnObject.AUEUMSUserViewModels != null)
            {
                List<AUEUMSRegisterViewModel> AUEUMSRegisterlist = returnObject.AUEUMSUserViewModels;
                AUEUMSRegisterListViewModel usrListViewModel = new AUEUMSRegisterListViewModel();
                usrListViewModel.registers = AUEUMSRegisterlist;
                usrListViewModel.mode = mode;
                usrListViewModel.ctrlid = ctrlid;
                return PartialView("UserManagerList", usrListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }
        public IActionResult RefreshAUEUMSRegister(int Id)
        {

            ReturnObject returnObject = new ReturnObject();
            returnObject = GetAUEUMSRegisters();
            List<AUEUMSRegisterViewModel> AUEUMSRegisterlist = new List<AUEUMSRegisterViewModel>();
            AUEUMSRegisterListViewModel usrListViewModel = new AUEUMSRegisterListViewModel();
            usrListViewModel.registers = AUEUMSRegisterlist;
            usrListViewModel.Id = Id;
            return PartialView("AUEUMSRegisterSelectView", usrListViewModel);

        }
        [ValidateAntiForgeryToken]
        public IActionResult GetAUEUMSRegister(string mode, long Id)
        {
            
            ReturnObject returnObject = GetAUEUMSRegisterByRef(Id);
            if (returnObject.AUEUMSUserViewModel != null)
            {
                AUEUMSRegisterViewModel usrViewModel = returnObject.AUEUMSUserViewModel;

                List<CollegeViewModel> collegeList = GetCollegeNameAll();
                usrViewModel.collegeList = collegeList;
                if (usrViewModel.collegeList != null)
                {
                    usrViewModel.collegeID = usrViewModel.College.Id;
                }

                List<RoleListViewModel> rList = GetRoleListAll();
                usrViewModel.rolelist = rList;
                if (usrViewModel.rolelist != null)
                {
                    usrViewModel.RoleListID = usrViewModel.RoleListID;
                }

                usrViewModel.mode = mode;
                usrViewModel.Id = returnObject.AUEUMSUserViewModel.Id;

                usrViewModel.title = "Modify User";
                return PartialView("NewUserManager", usrViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }
        [ValidateAntiForgeryToken]
        public IActionResult GetUserForApproval(string mode, long Id)
        {

            ReturnObject returnObject = GetUserForApprovalByRef(Id);
            if (returnObject.AUEUMSUserViewModel != null)
            {
                AUEUMSRegisterViewModel usrViewModel = returnObject.AUEUMSUserViewModel;

                List<RoleListViewModel> rolelists= GetRoleListAll();
                usrViewModel.rolelist = rolelists;
                if (usrViewModel.rolelist != null)
                {
                    usrViewModel.RoleListID = 3;
                }

                usrViewModel.mode = mode;
                usrViewModel.Id = returnObject.AUEUMSUserViewModel.Id;

                usrViewModel.title = "User Approval";
                return PartialView("ApproveUserManager", usrViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddAUEUMSRegister([FromBody] AUEUMSRegisterViewModel AUEUMSRegisterdata)
        {
            if (ModelState.IsValid)
            {
                ReturnObject returnObjectCont = GetAUEUMSRegisterByEmail(AUEUMSRegisterdata.Email);
                if (returnObjectCont.AUEUMSUserViewModel != null)
                {
                    if ((returnObjectCont.AUEUMSUserViewModel.Email == AUEUMSRegisterdata.Email))
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-9";
                        returnObject.SystemExceptionMessage = "User Email already exists";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }
           
                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());

                    if (AUEUMSRegisterdata.collegeID != null)
                    {
                        College collegeName = new College();
                        collegeName.Id = Convert.ToInt32(AUEUMSRegisterdata.collegeID);
                        AUEUMSRegisterdata.College = collegeName;
                    }
                    AUEUMSRegisterdata.EmailVerified = false;
                    AUEUMSRegisterdata.DeActivated = false;
                   

                    response = client.PostAsJsonAsync<AUEUMSRegisterViewModel>("Admin/AddAUEUMSUserRegisterAsync", AUEUMSRegisterdata).Result;
                    List<ReturnObject> UsrList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(UsrList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }

            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateAUEUMSRegister([FromBody] AUEUMSRegisterViewModel AUEUMSRegisterdata)
        {
            if (ModelState.IsValid)
            {
         
                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                    AUEUMSRegisterViewModel userViewModel = new AUEUMSRegisterViewModel();

                    userViewModel.Id = AUEUMSRegisterdata.Id;
                    userViewModel.SecretKey= AUEUMSRegisterdata.SecretKey;
                    userViewModel.FirstName= AUEUMSRegisterdata.FirstName.Trim();
                    userViewModel.LastName= AUEUMSRegisterdata.LastName.Trim();
                    userViewModel.AUEUMSUserType = AUEUMSRegisterdata.AUEUMSUserType;

                    if (AUEUMSRegisterdata.collegeID != null)
                    {
                        College collegeName = new College();
                        collegeName.Id = Convert.ToInt32(AUEUMSRegisterdata.collegeID);
                        AUEUMSRegisterdata.College = collegeName;
                    }
                    userViewModel.PostalCode= AUEUMSRegisterdata.PostalCode;
                    userViewModel.Phone= AUEUMSRegisterdata.Phone.Trim();
                    userViewModel.Address= AUEUMSRegisterdata.Address;
                    userViewModel.City= AUEUMSRegisterdata.City.Trim();
                    userViewModel.Email= AUEUMSRegisterdata.Email.Trim();
                    userViewModel.Password= AUEUMSRegisterdata.Password;
                    userViewModel.EmailVerified = true;
                    userViewModel.DeActivated = false;
             
                    response = client.PostAsJsonAsync<AUEUMSRegisterViewModel>("Admin/ModifyAUEUMSUserRegisterAsync", userViewModel).Result;
                    List<ReturnObject> UsrList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(UsrList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }

            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [ValidateAntiForgeryToken]
        public JsonResult ActivateDeactivateAUEUMSRegister([FromBody] AUEUMSRegisterViewModel AUEUMSRegisterdata)
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
                    
                    if (AUEUMSRegisterdata.InactiveDisplayFlag.ToLower() == "no")
                    {
                        AUEUMSRegisterdata.DeActivated = false;
                    }
                    else
                    {
                        AUEUMSRegisterdata.DeActivated = true;
                    }

                    if (AUEUMSRegisterdata.DeActivated == true)
                    {
                        response = client.PostAsJsonAsync("Admin/DeActivateAUEUMSUserAsync", AUEUMSRegisterdata).Result;
                   
                    }
                    else
                    {
                        response = client.PostAsJsonAsync("Admin/ActivateAUEUMSUserAsync", AUEUMSRegisterdata).Result;

                    }
                    List<ReturnObject> ClientList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(ClientList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }
            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);


            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult NewAUEUMSRegister(string mode, int AUEUMSRegisterID)
        {
            AUEUMSRegisterViewModel AUEUMSRegisterViewModel = new AUEUMSRegisterViewModel();

            AUEUMSRegisterViewModel.collegeList = GetCollegeNameAll();
            AUEUMSRegisterViewModel.collegeProgramList = GetCollegeProgramAll();
            AUEUMSRegisterViewModel.programMajorList = GetProgramMajorAll();
            AUEUMSRegisterViewModel.rolelist = GetRoleListAll();
           
            AUEUMSRegisterViewModel.mode = mode;
            AUEUMSRegisterViewModel.Id = -1;
            AUEUMSRegisterViewModel.title = "New User Register";
            return PartialView("NewUserManager", AUEUMSRegisterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ApproveAUEUMSUser([FromBody] AUEUMSRegisterViewModel AUEUMSRegisterdata)
        {
            if (ModelState.IsValid)
            {

                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                    AUEUMSRegisterViewModel userViewModel = new AUEUMSRegisterViewModel();

                    userViewModel.Id = AUEUMSRegisterdata.Id;
                    userViewModel.SecretKey = AUEUMSRegisterdata.SecretKey;
                    userViewModel.FirstName = AUEUMSRegisterdata.FirstName;
                    userViewModel.LastName = AUEUMSRegisterdata.LastName;
                    userViewModel.PostalCode = AUEUMSRegisterdata.PostalCode;
                    userViewModel.Phone = AUEUMSRegisterdata.Phone;
                    userViewModel.Address = AUEUMSRegisterdata.Address;
                    userViewModel.City = AUEUMSRegisterdata.City;                   

                    if (AUEUMSRegisterdata.RoleListID != null)
                    {
                        Role rl = new Role();
                        rl.Id= Convert.ToInt16(AUEUMSRegisterdata.RoleListID);
                        userViewModel.RoleListID = rl.Id;
                    }
                    userViewModel.Email = AUEUMSRegisterdata.Email;

                    userViewModel.Password = AUEUMSRegisterdata.Password;
                    userViewModel.EmailVerified = true;
                    userViewModel.DeActivated = false;
                    response = client.PostAsJsonAsync<AUEUMSRegisterViewModel>("Admin/ApproveAUEUMSUserAsync", userViewModel).Result;
                    List<ReturnObject> UsrList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(UsrList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }

            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [HttpGet]
        public ActionResult getAUEUMSUserListPartialViewForContainer(string MenuID, int size)
        {
            string accessToken = getAccesssToken();
            if (accessToken == "-1")
            {
                return RedirectToAction("login", "Account", new { returnUrl = "Admin/DashboardAdmin" });
            }
            else if (accessToken == "-2")
            {
                return Redirect("Account/UserAccessDenied");
            }
            if (size != 0)
            {
                HttpContext.Session.SetString("days", size.ToString());
            }

            int days = Convert.ToInt32(HttpContext.Session.GetString("days"));

            var userEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email).Value;
            AUEUMSRegisterListViewModel sellistViewModel = new AUEUMSRegisterListViewModel();

            return PartialView("UserManagerListPartial", sellistViewModel);

        }

        [HttpPost]
        public ActionResult GetAllAUEUMSExternalUsersJson([FromBody] DatatableParams datatableParams)
        {

            using (var client = new HttpClient())
            {
                int Id = 0;
                int start = 0;
                int length = 10;
                string sortColumn = "";
                string sortDir = "";
                string searchStr = "-1";
                string src = "";

                if (datatableParams != null)
                {
                    Id = datatableParams.Id;
                    src = datatableParams.src;
                    if (datatableParams.searchStr != null && datatableParams.searchStr != "")
                        searchStr = datatableParams.searchStr;

                    sortColumn = Convert.ToString(datatableParams.column);
                    sortDir = datatableParams.dir;
                    start = datatableParams.start;
                    length = datatableParams.length;
                }

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
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/GetAllAUEUMSExternalUsersJsonAsync" + $"/{Id}" + $"/{src}" + $"/{start}" + $"/{length}" + $"/{sortColumn}" + $"/{sortDir}" + $"/{searchStr}").Result;


                if (response.IsSuccessStatusCode)
                {
                    string quoteListJson = response.Content.ReadAsAsync<string>().Result;
                    return Ok(quoteListJson);
                }
                else
                {
                    return Ok(null);
                }
            }
        }


        #endregion
        #region Timetable Header

        public IActionResult TimetableList(string mode, string ctrlid)
        {
            ReturnObject returnObject = TimetableListAll();
            if (returnObject.timetableHeaderViewModels != null)
            {
                List<TimetableHeaderViewModel> tHeaderlist = returnObject.timetableHeaderViewModels;
                TimetableHeaderListViewModel tListViewModel = new TimetableHeaderListViewModel();
                tListViewModel.timetableHeaders = tHeaderlist;               
                tListViewModel.mode = mode;
                tListViewModel.ctrlid = ctrlid;
                return PartialView("StudentTimetableList", tListViewModel);

            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult NewTimetableHeader(string mode, int TimeTableID)
        {
            TimetableHeaderViewModel timetableHeaderViewModel = new TimetableHeaderViewModel();
            timetableHeaderViewModel.mode = mode;
            timetableHeaderViewModel.Id = -1;
            timetableHeaderViewModel.tblRoomlist = GetRoomsAll();
            timetableHeaderViewModel.tblYearlist = GetYearAll();
            timetableHeaderViewModel.tblSemesterTypelist = GetSemesterTypeAll();
            timetableHeaderViewModel.title = "New Timetable Header";
            return PartialView("NewTimetableHeader", timetableHeaderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTimetableHeader([FromBody] TimetableHeaderViewModel timetableHeaderViewModel)
        {
            if (ModelState.IsValid)
            {
                //DateTime dateValue = new DateTime(2008, 6, 11);
                //Console.WriteLine(dateValue.ToString("dddd"));

                //DateTime dateValue = new DateTime(2008, 6, 11);
                //int dayFromWeek=(int)dateValue.DayOfWeek;


                // Validation Weekends

                DateTime DtTimetableValidate, DtTimetableCheck = DateTime.UtcNow.AddDays(4);

                if (!DateTime.TryParseExact(timetableHeaderViewModel.TimetableDateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtTimetableValidate))
                {  }
                else { DtTimetableCheck = DtTimetableValidate; }
                   
                int dayFromWeek = (int)DtTimetableCheck.DayOfWeek;
                if (dayFromWeek>5 && dayFromWeek<8)
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    returnObject.SystemExceptionMessage = "University Operating Days: Sunday to Thursday only";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return Json(jsonresult);
                }
               


                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());


                    TimetableHeaderViewModel ttHeaderViewModel = new TimetableHeaderViewModel();
                    ttHeaderViewModel.SemesterTypelistId = timetableHeaderViewModel.SemesterTypelistId;
                    ttHeaderViewModel.YearlistId = timetableHeaderViewModel.YearlistId;
                    ttHeaderViewModel.RoomlistId = timetableHeaderViewModel.RoomlistId;
         
                    DateTime DtTimetableHdrTime;
                    if (!DateTime.TryParseExact(timetableHeaderViewModel.TimetableDateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtTimetableHdrTime))
                    { ttHeaderViewModel.TDate = null; }
                    else { ttHeaderViewModel.TDate = DtTimetableHdrTime; }


                    response = client.PostAsJsonAsync<TimetableHeaderViewModel>("Admin/AddTimetableHeaderAsync", ttHeaderViewModel).Result;
                   
                    List<ReturnObject> TimetableHeaderList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(TimetableHeaderList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }
            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateTimetableHeader([FromBody] TimetableHeaderViewModel timetableHeaderViewModel)
        {
            if (ModelState.IsValid)
            {

                // Validation Weekends

                DateTime DtTimetableValidate, DtTimetableCheck = DateTime.UtcNow.AddDays(4);

                if (!DateTime.TryParseExact(timetableHeaderViewModel.TimetableDateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtTimetableValidate))
                { }
                else { DtTimetableCheck = DtTimetableValidate; }

                int dayFromWeek = (int)DtTimetableCheck.DayOfWeek;
                if (dayFromWeek > 5 && dayFromWeek < 8)
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    returnObject.SystemExceptionMessage = "University Operating Days: Sunday to Thursday only";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return Json(jsonresult);
                }





                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());


                    TimetableHeaderViewModel ttHeaderViewModel = new TimetableHeaderViewModel();
                    ttHeaderViewModel.Id = timetableHeaderViewModel.Id;
                    ttHeaderViewModel.SemesterTypelistId = timetableHeaderViewModel.SemesterTypelistId;
                    ttHeaderViewModel.YearlistId = timetableHeaderViewModel.YearlistId;
                    ttHeaderViewModel.RoomlistId = timetableHeaderViewModel.RoomlistId;

                    DateTime DtTimetableHdrTime;
                    if (!DateTime.TryParseExact(timetableHeaderViewModel.TimetableDateString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtTimetableHdrTime))
                    { ttHeaderViewModel.TDate = null; }
                    else { ttHeaderViewModel.TDate = DtTimetableHdrTime; }

                    response = client.PostAsJsonAsync<TimetableHeaderViewModel>("Admin/UpdateTimetableHeaderAsync", ttHeaderViewModel).Result;
                   
                    List<ReturnObject> TimetableHeaderList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(TimetableHeaderList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }

            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetTimetableHdrEdit(string mode, long TimetableHdrId)
        {
            ReturnObject returnObject = GetTimetableHdrEditByRef(TimetableHdrId);
            if (returnObject.timetableHeaderViewModel != null)
            {
                TimetableHeaderViewModel timetableHeaderViewModel = returnObject.timetableHeaderViewModel;
                timetableHeaderViewModel.mode = mode;
                timetableHeaderViewModel.Id = TimetableHdrId;
                timetableHeaderViewModel.tblRoomlist = GetRoomsAll();
                timetableHeaderViewModel.SemesterTypelistId = timetableHeaderViewModel.tblSemesterType.Id;
                timetableHeaderViewModel.YearlistId = timetableHeaderViewModel.tblYear.Id;
                timetableHeaderViewModel.RoomlistId = timetableHeaderViewModel.tblRoom.Id;
                timetableHeaderViewModel.tblYearlist = GetYearAll();
                timetableHeaderViewModel.tblSemesterTypelist = GetSemesterTypeAll();
                timetableHeaderViewModel.title = "Modify Timetable Header";
                return PartialView("NewTimetableHeader", timetableHeaderViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }
        
        [ValidateAntiForgeryToken]
        public IActionResult NewTimetableDetails(string mode, int TimetableHdrId)
        {
            TimetableDetailViewModel timetableDetailViewModel = new TimetableDetailViewModel();
            timetableDetailViewModel.mode = mode;
            timetableDetailViewModel.Id = -1;
            ReturnObject returnObject = GetTimetableHdrEditByRef(TimetableHdrId);
            if (returnObject.timetableHeaderViewModel != null)
            {
                TimetableHeaderViewModel timetableHeaderViewModel = returnObject.timetableHeaderViewModel;
                DateTime dtTTDate=(DateTime)timetableHeaderViewModel.TDate;
                int dayofweek = (int)dtTTDate.DayOfWeek;
                timetableDetailViewModel.DayslistId = dayofweek+1;
            }
            timetableDetailViewModel.TimeTableHdrParamId = TimetableHdrId;
            timetableDetailViewModel.tblDayslist = GetDaysAll();
            timetableDetailViewModel.tblTimeSlotlist = GetTimeSlotAll();
            timetableDetailViewModel.tblCourseSeclist = GetCourseSectionAll();
            timetableDetailViewModel.tblCourselist = GetCourseAll();

            CourseSectionListViewModel courseSectionListViewModel = new CourseSectionListViewModel();
            courseSectionListViewModel.CtnrCourseSeclist = GetCourseSectionAll();
            courseSectionListViewModel.typeCourseSec = "-1";
            timetableDetailViewModel.crsSecListViewModel = courseSectionListViewModel;


            timetableDetailViewModel.title = "New Timetable Details";

            return PartialView("NewTimetableDetails", timetableDetailViewModel);
        }
        [ValidateAntiForgeryToken]
        public IActionResult refreshCourseSecSelectView(long tdlCourseId)
        {
            ReturnObject returnObjectCourse = GetCourseByRef(tdlCourseId);
            if (returnObjectCourse.courseViewModel != null)
            {
                CourseViewModel crslist = returnObjectCourse.courseViewModel;
                CourseSectionListViewModel courseSectionListViewModel = new CourseSectionListViewModel();
                courseSectionListViewModel.CtnrCourseSeclist = GetCourseSectionByRef(tdlCourseId);
                courseSectionListViewModel.typeCourseSec = "-1";

                return PartialView("CourseSecSelectView", courseSectionListViewModel);
            }
            else
            {

                return PartialView("ExcpetionPartial");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTimetableDetail([FromBody] TimetableDetailViewModel timetableDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                ReturnObject returnTTObject = ValidateTimetable(timetableDetailViewModel);
                if (returnTTObject.isTimetableExists != 0)
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    returnObject.SystemExceptionMessage = "Same Timetable Exists";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return Json(jsonresult);

                }
                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());


                    TimetableDetailViewModel ttDetailViewModel = new TimetableDetailViewModel();
                    ttDetailViewModel.TimeTableHdrParamId = timetableDetailViewModel.TimeTableHdrParamId;
                    ttDetailViewModel.CourselistId = timetableDetailViewModel.CourselistId;
                    ttDetailViewModel.CourseSeclistId = timetableDetailViewModel.CourseSeclistId;
                    ttDetailViewModel.DayslistId = timetableDetailViewModel.DayslistId;
                    ttDetailViewModel.TimeSlotlistId = timetableDetailViewModel.TimeSlotlistId;

                    response = client.PostAsJsonAsync<TimetableDetailViewModel>("Admin/AddTimetableDetailsAsync", ttDetailViewModel).Result;

                    List<ReturnObject> TimetableHeaderList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(TimetableHeaderList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }
            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateTimetableDetail([FromBody] TimetableDetailViewModel timetableDetailViewModel)
        {
            if (ModelState.IsValid)
            {
                ReturnObject returnTTObject = ValidateTimetable(timetableDetailViewModel);
                if (returnTTObject.isTimetableExists != 0)
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    returnObject.SystemExceptionMessage = "Same Timetable Exists";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return Json(jsonresult);

                }



                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());


                    TimetableDetailViewModel ttDetailViewModel = new TimetableDetailViewModel();
                    ttDetailViewModel.Id = timetableDetailViewModel.Id;
                    ttDetailViewModel.TimeTableHdrParamId = timetableDetailViewModel.TimeTableHdrParamId;
                    ttDetailViewModel.CourselistId = timetableDetailViewModel.CourselistId;
                    ttDetailViewModel.CourseSeclistId = timetableDetailViewModel.CourseSeclistId;
                    ttDetailViewModel.DayslistId = timetableDetailViewModel.DayslistId;
                    ttDetailViewModel.TimeSlotlistId = timetableDetailViewModel.TimeSlotlistId;

                    response = client.PostAsJsonAsync<TimetableDetailViewModel>("Admin/UpdateTimetableDetailAsync", ttDetailViewModel).Result;

                    List<ReturnObject> TimetableHeaderList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(TimetableHeaderList[0]);
                        return Json(jsonresult);
                    }
                    else
                    {
                        ReturnObject returnObject = new ReturnObject();
                        returnObject.Usermessage = "-3";
                        string jsonresult = JsonConvert.SerializeObject(returnObject);
                        return Json(jsonresult);
                    }
                }

            }
            else
            {
                ReturnObject returnObject = new ReturnObject();
                returnObject.Usermessage = "-4";
                string jsonresult = JsonConvert.SerializeObject(returnObject);
                return Json(jsonresult);
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult ViewTTDetailsListPartial(string mode, long TimetableHdrId)
        {

            ReturnObject returnObject = TimetableDetailListAll(TimetableHdrId);
            if (returnObject.timetableDetailViewModels != null)
            {
                List<TimetableDetailViewModel> tDtllist = returnObject.timetableDetailViewModels;
                TimetableDetailListViewModel tListViewModel = new TimetableDetailListViewModel();
                tListViewModel.timetableDetails = tDtllist;
                tListViewModel.mode = mode;
                tListViewModel.ctrlid = "ctrlid";
                tListViewModel.Hdrparamid = TimetableHdrId;
                return PartialView("StudentTimetableDetail", tListViewModel);
            
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }
        [ValidateAntiForgeryToken]
        public IActionResult GetTTDetailsEdit(string mode, long HdrId, long DtlId)
        {
            ReturnObject returnObject = GetTTDetailsByRef(HdrId, DtlId);
            if (returnObject.timetableDetailViewModel != null)
            {
                TimetableDetailViewModel ttdetailViewModel = returnObject.timetableDetailViewModel;
                ttdetailViewModel.mode = mode;
                ttdetailViewModel.Id = DtlId;
                ttdetailViewModel.TimeTableHdrParamId = HdrId;

                ttdetailViewModel.tblDayslist = GetDaysAll();
                ttdetailViewModel.tblTimeSlotlist = GetTimeSlotAll();
                ttdetailViewModel.tblCourseSeclist = GetCourseSectionAll();
                ttdetailViewModel.tblCourselist = GetCourseAll();

                CourseSectionListViewModel courseSectionListViewModel = new CourseSectionListViewModel();
                courseSectionListViewModel.CtnrCourseSeclist = GetCourseSectionAll();
                courseSectionListViewModel.typeCourseSec = ttdetailViewModel.tblCourseSection.Id.ToString();
                ttdetailViewModel.crsSecListViewModel = courseSectionListViewModel;
                ttdetailViewModel.CourselistId= ttdetailViewModel.tblCourse.Id;
                ttdetailViewModel.CourseSeclistId = ttdetailViewModel.tblCourseSection.Id;
                ttdetailViewModel.TimeSlotlistId = ttdetailViewModel.tblTimeSlot.Id;
                ttdetailViewModel.DayslistId = ttdetailViewModel.tblDays.Id;

                ttdetailViewModel.title = "Modify Timetable Details";
                return PartialView("NewTimetableDetails", ttdetailViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }

        public ActionResult DashboardAdminHome(string mode, string ctrlid,string src)
        {
       
           return RedirectToAction("DashboardAdmin", "Admin");
           
        }
        public ActionResult DashboardStudentHome(string mode, string ctrlid, string src)
        {    
          return RedirectToAction("DashboardStudent", "Admin");
          
        }
        public ActionResult DashboardFacultyHome(string mode, string ctrlid)
        {
            return RedirectToAction("DashboardFaculty", "Admin");
          
        }

        [ValidateAntiForgeryToken]
        public IActionResult ViewTTReportListPartial(string mode)
        {
            long TimetableHdr = 1;
            ReturnObject returnObject = TimetableDetailListAll(TimetableHdr);
            if (returnObject.timetableDetailViewModels != null)
            {
                List<TimetableDetailViewModel> tDtllist = returnObject.timetableDetailViewModels;
                TimetableDetailListViewModel tListViewModel = new TimetableDetailListViewModel();
                tListViewModel.timetableDetails = tDtllist;
                tListViewModel.mode = mode;
                tListViewModel.ctrlid = "ctrlid";
                return PartialView("StudentTimetableReport", tListViewModel);

            }
            else
            {

                return PartialView("ExcpetionPartial");
            }

        }


        [HttpGet]
        [ValidateAntiForgeryToken]
        public JsonResult getTimetableDetailsByHdrIdJSON(long estId)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.GetAsync("Admin/getTimetableDetailsByHdrIdJSONAsync" + $"/{estId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string ttDetails = response.Content.ReadAsAsync<string>().Result;

                    return Json(ttDetails);

                }
                else
                {
                    return null;
                }
            }
        }


        [HttpGet]
        [ValidateAntiForgeryToken]
        public string getReportTimetableGroupbyJSON(string grpColumn)
        {
            HttpResponseMessage response;
            using (var client = new HttpClient())
            {
                string accessToken = getAccesssToken();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                response = client.GetAsync("Admin/getReportTimetableGroupbyJSONAsync" + $"/{grpColumn}").Result;
                if (response.IsSuccessStatusCode)
                {
                    string estDetails = response.Content.ReadAsAsync<string>().Result;
                    return estDetails;
                }
                else
                {
                    return null;
                }
            }
        }


        [HttpPost]
        public ActionResult GetAllTimetableJson([FromBody] DatatableParams datatableParams)
        {
            string grpColumn = "CourseName";
            using (var client = new HttpClient())
            {
                int Id = 0;
                int start = 0;
                int length = 10;
                string sortColumn = "";
                string sortDir = "";
                string searchStr = "-1";
                string src = "";

                if (datatableParams != null)
                {
                    Id = datatableParams.Id;
                    src = datatableParams.src;
                    if (datatableParams.searchStr != null && datatableParams.searchStr != "")
                        searchStr = datatableParams.searchStr;

                    sortColumn = Convert.ToString(datatableParams.column);
                    sortDir = datatableParams.dir;
                    start = datatableParams.start;
                    length = datatableParams.length;
                }

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
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                HttpResponseMessage response = client.
                     GetAsync("Admin/getReportTimetableGroupbyJSONAsync" + $"/{grpColumn}").Result;


                if (response.IsSuccessStatusCode)
                {
                    string tListJson = response.Content.ReadAsAsync<string>().Result;

                    var tempListJson = JsonConvert.DeserializeObject(tListJson);
                    return Ok(tempListJson);
                }
                else
                {
                    return Ok(null);
                }
            }
        }
        public IActionResult GetMenu()
        {
            return PartialView("Menu");

        }
        #endregion
        
    }
}