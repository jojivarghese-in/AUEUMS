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
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Http.Json;
using Azure;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting.Internal;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace AUEUMS.Controllers
{
    public class FacultyController : BaseController
    {
        private readonly APISettings _APISettings;
        private readonly MailSettings _MailSettings;
        private readonly IWebHostEnvironment _env;


        public FacultyController(IOptions<APISettings> APISettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env) : base(APISettings, MailSettings, env)
        {
            _APISettings = APISettings.Value;
            _MailSettings = MailSettings.Value;
            _env = env;

        }

        #region Assignments


        public IActionResult Assignments(string mode, string ctrlid)
        {
            ReturnObject returnObject = new ReturnObject();
            returnObject = GetAssignments();
            if (returnObject.assignmentsViewModels != null)
            {
                List<AssignmentsViewModel> Assignmentslist = returnObject.assignmentsViewModels;

                AssignmentsListViewModel assignmentsListViewModel = new AssignmentsListViewModel();
                assignmentsListViewModel.assignments = Assignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("AssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }

        public IActionResult AssignmentUploaded()
        {
            ReturnObject returnObject = new ReturnObject();
            returnObject = GetAssignments();
            if (returnObject.assignmentsViewModels != null)
            {
                List<AssignmentsViewModel> Assignmentslist = returnObject.assignmentsViewModels;

                AssignmentsListViewModel assignmentsListViewModel = new AssignmentsListViewModel();
                assignmentsListViewModel.assignments = Assignmentslist;
                assignmentsListViewModel.mode = "detail";
                assignmentsListViewModel.ctrlid = "ctrlid";
                return PartialView("AssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }

        public IActionResult AssignmentsAll(string mode, string ctrlid)
        {
            ReturnObject returnObject = GetAssignmentsAll();
            if (returnObject.assignmentsViewModels != null)
            {
                List<AssignmentsViewModel> Assignmentslist = returnObject.assignmentsViewModels;
                AssignmentsListViewModel assignmentsListViewModel = new AssignmentsListViewModel();
                assignmentsListViewModel.assignments = Assignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("AssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }

        public IActionResult StudentEvaluation(string mode, string ctrlid)
        {
            ReturnObject returnObject = StudentAnsweredAssignments();
            if (returnObject.assignmentsForStudentsViewModels != null)
            {
                List<AssignmentsForStudentsViewModel> studentAssignmentslist = returnObject.assignmentsForStudentsViewModels;
                AssignmentsForStudentsListViewModel assignmentsListViewModel = new AssignmentsForStudentsListViewModel();
                assignmentsListViewModel.assignmentsForStudents = studentAssignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("StudentAnsweredAssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }



        [ValidateAntiForgeryToken]
        public IActionResult GetAssignmentsEdit(string mode, long AssignmentId)
        {
            ReturnObject returnObject = GetAssignmentsByRef(AssignmentId);
            if (returnObject.assignmentsViewModel != null)
            {
                AssignmentsViewModel assignmentsViewModel = returnObject.assignmentsViewModel;
                assignmentsViewModel.mode = mode;
                assignmentsViewModel.Id = AssignmentId;
                assignmentsViewModel.title = "Modify Assignment";
                return PartialView("NewAssignments", assignmentsViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult UploadAssignment(string mode, long AssignmentId)
        {
            ReturnObject returnObject = GetAssignmentsByRef(AssignmentId);
            if (returnObject.assignmentsViewModel != null)
            {
                AssignmentsViewModel assignmentsViewModel = returnObject.assignmentsViewModel;
                assignmentsViewModel.mode = mode;
                assignmentsViewModel.Id = AssignmentId;
                assignmentsViewModel.title = "Upload Assignment File";
                return PartialView("UploadAssignments", assignmentsViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddAssignments([FromBody] AssignmentsViewModel assignmentsViewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime DtPostedDateTimeValidate, DtPostedDateTimeCheck=DateTime.UtcNow.AddDays(4);
                DateTime DtDueDateTimeValidate, DtDueDateTimeCheck = DateTime.UtcNow.AddDays(4);

                if (!DateTime.TryParseExact(assignmentsViewModel.PostedDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtPostedDateTimeValidate))
                {  }
                else { DtPostedDateTimeCheck = DtPostedDateTimeValidate; }

                

                if (!DateTime.TryParseExact(assignmentsViewModel.DueDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtDueDateTimeValidate))
                {  }
                else { DtDueDateTimeCheck = DtDueDateTimeValidate; }

                int vCheck = DateTime.Compare(DtDueDateTimeCheck, DtPostedDateTimeCheck);

                if (vCheck<0)
                {
                    ReturnObject returnObject = new ReturnObject();
                    returnObject.Usermessage = "-9";
                    returnObject.SystemExceptionMessage = "Due Date Should be greater than Posted Date";
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


                    AssignmentsViewModel asstViewModel = new AssignmentsViewModel();
                    asstViewModel.AssignmentTitle = assignmentsViewModel.AssignmentTitle;
                    asstViewModel.AssignmentDescription = assignmentsViewModel.AssignmentDescription;

                    DateTime DtPostedDateTime;
                    if (!DateTime.TryParseExact(assignmentsViewModel.PostedDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtPostedDateTime))
                    { asstViewModel.PostedDateTime = null; }
                    else { asstViewModel.PostedDateTime = DtPostedDateTime; }

                    DateTime DtDueDateTime;
                    if (!DateTime.TryParseExact(assignmentsViewModel.DueDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtDueDateTime))
                    { asstViewModel.DueDateTime = null; }
                    else { asstViewModel.DueDateTime = DtDueDateTime; }

                    response = client.PostAsJsonAsync<AssignmentsViewModel>("Faculty/AddAssignmentsAsync", asstViewModel).Result;
                    List<ReturnObject> AssignmentList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(AssignmentList[0]);
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
        public JsonResult UpdateAssignments([FromBody] AssignmentsViewModel assignmentsViewModel)
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

                    AssignmentsViewModel asstViewModel = new AssignmentsViewModel();

                    asstViewModel.Id= assignmentsViewModel.Id;
                    asstViewModel.AssignmentTitle = assignmentsViewModel.AssignmentTitle;
                    asstViewModel.AssignmentDescription = assignmentsViewModel.AssignmentDescription;

                    DateTime DtPostedDateTime;
                    if (!DateTime.TryParseExact(assignmentsViewModel.PostedDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtPostedDateTime))
                    { asstViewModel.PostedDateTime = null; }
                    else { asstViewModel.PostedDateTime = DtPostedDateTime; }

                    DateTime DtDueDateTime;
                    if (!DateTime.TryParseExact(assignmentsViewModel.DueDateTimeString, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DtDueDateTime))
                    { asstViewModel.DueDateTime = null; }
                    else { asstViewModel.DueDateTime = DtDueDateTime; }

                    response = client.PostAsJsonAsync<AssignmentsViewModel>("Faculty/ModifyAssignmentsAsync", asstViewModel).Result;
                    List<ReturnObject> assignmentsList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresult = JsonConvert.SerializeObject(assignmentsList[0]);
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
        public IActionResult NewAssignments(string mode, int AssignmentsID)
        {
            AssignmentsViewModel assignmentsViewModel = new AssignmentsViewModel();       
            assignmentsViewModel.mode = mode;
            assignmentsViewModel.Id = -1;
            assignmentsViewModel.title = "New Assignment";
            return PartialView("NewAssignments", assignmentsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(AssignmentsViewModel model)
        {
            string strFileName = null;

            if (model.File != null && model.File.Length > 0)
            {

                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                strFileName = model.File.FileName;

                string filePath = Path.Combine(uploadPath, model.File.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }
                
              
            }
            Thread.Sleep(500);


            ReturnObject returnObject = GetAssignmentsByRef((int)model.AsstId);
            if (returnObject.assignmentsViewModel != null)
            {
                AssignmentsViewModel assignmentsViewModel = returnObject.assignmentsViewModel;

                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());


                    assignmentsViewModel.UploadFileName = strFileName;
                    assignmentsViewModel.UploadFileRefNo= Guid.NewGuid().ToString();

                    response = client.PostAsJsonAsync<AssignmentsViewModel>("Faculty/UploadUpdatesAssignmentsAsync", assignmentsViewModel).Result;
                    List<ReturnObject> assignmentsList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        returnObject = new ReturnObject();
                        returnObject = GetAssignments();
                        if (returnObject.assignmentsViewModels != null)
                        {
                            return RedirectToAction("DashboardFaculty", "Admin");
                        }
                        else
                        {
                            
                            return RedirectToAction("DashboardFaculty", "Admin");
                        }
                    }
                    
                }

            }
            return RedirectToAction("DashboardFaculty", "Admin");

        }

        [HttpGet]
        public IActionResult DownloadFile(long AssignmentId)
        {
            AssignmentsViewModel assignmentsViewModel = null;
            ReturnObject returnObject = GetAssignmentsByRef(AssignmentId);
            if (returnObject.assignmentsViewModel != null)
            {
                assignmentsViewModel = returnObject.assignmentsViewModel;
            }

            string uploadPath = Path.Combine(_env.WebRootPath, "Docs\\uploads");
            string filePath = Path.Combine(uploadPath, assignmentsViewModel.UploadFileName);

           // var path = Path.Combine(_hostingEnvironment.WebRootPath, "Sample.xlsx");
           // var fs = new FileStream(filePath, FileMode.Open);

            //  return File(fs, "application/octet-stream", assignmentsViewModel.UploadFileName);

            //if (System.IO.File.Exists(filePath))
            //{
            //    return File(System.IO.File.OpenRead(filePath), "application/octet-stream", Path.GetFileName(filePath));
            //}

            try
            {
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    return File(stream, "application/octet-stream", assignmentsViewModel.UploadFileName);
                }
                
                //if (string.IsNullOrEmpty(assignmentsViewModel.UploadFileName))
                //{
                //    return Content("Filename is not provided.");
                //}
                //filePath = Path.Combine(_env.WebRootPath, "uploads", assignmentsViewModel.UploadFileName);
                //if (!System.IO.File.Exists(filePath))
                //{
                //    return Content("File not found.");
                //}
                //byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                //return File(fileBytes, "application/octet-stream", assignmentsViewModel.UploadFileName);
            }
            
            catch (Exception ex)
            {
            }


            //if (!System.IO.File.Exists(filePath))
            //{
            //    WebClient client = new WebClient();
            //    Thread.Sleep(1000);
            //    client.DownloadFile(filePath, assignmentsViewModel.UploadFileName);

            //}

            //string diretorio = Server.MapPath("~/Docs");

            //var ext = ".pdf";
            //file = file + extensao;
            //var arquivo = Path.Combine(diretorio, file);
            //var contentType = "application/pdf";

            //using (var client = new WebClient())
            //{
            //    var buffer = client.DownloadData(arquivo);
            //    return File(buffer, contentType);
            //}
            return Ok("File Download");
        }


        [ValidateAntiForgeryToken]
        public IActionResult UpdateStudentEvaluation(string mode, long AssignmentId, long StudentId)
        {

            ReturnObject returnObject = StudentAnsweredAssignmentsByRef(AssignmentId, StudentId);
            if (returnObject.assignmentsForStudentsViewModel != null)
            {

                AssignmentsForStudentsViewModel assignmentsForStudentsViewModel = returnObject.assignmentsForStudentsViewModel;
                assignmentsForStudentsViewModel.mode = mode;
                assignmentsForStudentsViewModel.pAssignmentsId = AssignmentId;
                assignmentsForStudentsViewModel.pStudentId = StudentId;
                assignmentsForStudentsViewModel.title = "Student Evaluation";
                return PartialView("StudentEvaluation", assignmentsForStudentsViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateEvaluation([FromBody] AssignmentsForStudentsViewModel model)
        {
            if (ModelState.IsValid)
            {

                ReturnObject returnObject = GetStudentAssignmentsByRef((long)model.mAssignmentId, (long)model.mStudentUsrId);

                if (returnObject.studentAssignmentsViewModel != null)
                {
                    StudentAssignmentsViewModel assignmentsViewModel = returnObject.studentAssignmentsViewModel;

                    HttpResponseMessage response;
                    using (var client = new HttpClient())
                    {
                        string accessToken = getAccesssToken();
                        client.BaseAddress = new Uri(Baseurl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());
                        assignmentsViewModel.StudentUsrId = (int)model.mStudentUsrId;
                        assignmentsViewModel.sAsstId = (long)model.mAssignmentId;

                        assignmentsViewModel.GradingCriteria = model.mGradingCriteria;
                        assignmentsViewModel.ScoreRange= (int)model.mScoreRange;
                        assignmentsViewModel.Remarks= model.mRemarks;

                        response = client.PostAsJsonAsync<StudentAssignmentsViewModel>("faculty/ModifyStudentEvaluationAsync", assignmentsViewModel).Result;
                        List<ReturnObject> assignmentsList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonresult = JsonConvert.SerializeObject(assignmentsList[0]);
                            return Json(jsonresult);
                        }
                        else
                        {
                            returnObject = new ReturnObject();
                            returnObject.Usermessage = "-3";
                            string jsonresult = JsonConvert.SerializeObject(returnObject);
                            return Json(jsonresult);

                        }

                    }

                }
                else
                {
                    returnObject = new ReturnObject();
                    returnObject.Usermessage = "-3";
                    string jsonresult = JsonConvert.SerializeObject(returnObject);
                    return Json(jsonresult);

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



        #endregion

    }
}
