using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AUEUMS.View_Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using AUEUMS.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Azure;
using System.Globalization;
using AUEUMS.Models;

namespace AUEUMS.Controllers
{
    [Authorize]
    public class StudentController : BaseController
    {
        private readonly APISettings _APISettings;
        private readonly MailSettings _MailSettings;
        private readonly IWebHostEnvironment _env;

        public StudentController(IOptions<APISettings> APISettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env) : base(APISettings, MailSettings, env)
        {
            _APISettings = APISettings.Value;
            _MailSettings = MailSettings.Value;
            _env = env;
        }

        #region Student Assignments
        public IActionResult StudentAssignments(string mode, string ctrlid)
        {
            ReturnObject returnObject = new ReturnObject();
            returnObject = GetAssignmentsForStudents();
            if (returnObject.assignmentsForStudentsViewModels != null)
            {
                List<AssignmentsForStudentsViewModel> studentAssignmentslist = returnObject.assignmentsForStudentsViewModels;

                AssignmentsForStudentsListViewModel assignmentsListViewModel = new AssignmentsForStudentsListViewModel();
                assignmentsListViewModel.assignmentsForStudents = studentAssignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("StudentAssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }
        public IActionResult StudentAssignmentsAll(string mode, string ctrlid)
        {
            ReturnObject returnObject = GetAssignmentsForStudentsAll();
            if (returnObject.assignmentsForStudentsViewModels != null)
            {
                List<AssignmentsForStudentsViewModel> studentAssignmentslist = returnObject.assignmentsForStudentsViewModels;
                AssignmentsForStudentsListViewModel assignmentsListViewModel = new AssignmentsForStudentsListViewModel();
                assignmentsListViewModel.assignmentsForStudents = studentAssignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("StudentAssignmentsList", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
        }


        public IActionResult AssignmentsFromFacultyAll(string mode, string ctrlid)
        {
            ReturnObject returnObject = new ReturnObject();
            returnObject = StudentAssignmentsFromFacultybyRef();
            if (returnObject.assignmentsViewModels != null)
            {
                List<AssignmentsViewModel> studentAssignmentslist = returnObject.assignmentsViewModels;

                AssignmentsFornFacultyListViewModel assignmentsListViewModel = new AssignmentsFornFacultyListViewModel();
                assignmentsListViewModel.assignmentsForFaculty = studentAssignmentslist;
                assignmentsListViewModel.mode = mode;
                assignmentsListViewModel.ctrlid = ctrlid;
                return PartialView("StudentAssignmentsFromFaculty", assignmentsListViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }

        [ValidateAntiForgeryToken]
        public IActionResult UploadAssignmentByStudent(string mode, long AssignmentId,long StudentId)
        {
            ReturnObject returnObject = GetAssignmentsByRef(AssignmentId);
            if (returnObject.assignmentsViewModel != null)
            {
                AssignmentsViewModel assignmentsViewModel = returnObject.assignmentsViewModel;
                assignmentsViewModel.mode = mode;
                assignmentsViewModel.AsstId = AssignmentId;
                assignmentsViewModel.StudentAsstId = StudentId;
                assignmentsViewModel.title = "Upload Assignment Completion";
                return PartialView("UploadStudentAssignments", assignmentsViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }
      
        }

        [HttpPost]
        public async Task<IActionResult> UploadStudentFile(AssignmentsViewModel model)
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


            ReturnObject returnObject = GetStudentAssignmentsByRef((long)model.AsstId,(long) model.StudentAsstId);

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

                    assignmentsViewModel.AssignmentsSubmitFileName = strFileName;
                    assignmentsViewModel.AssignmentsSubmitFileRef = Guid.NewGuid().ToString();

                    response = client.PostAsJsonAsync<StudentAssignmentsViewModel>("Student/ModifyStudentAssignmentsAsync", assignmentsViewModel).Result;
                    List<ReturnObject> assignmentsList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        
                    }

                }

            }
            else
            {
                HttpResponseMessage response;
                using (var client = new HttpClient())
                {
                    string accessToken = getAccesssToken();
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccesssToken());

           
                    StudentAssignmentsViewModel asstViewModel = new StudentAssignmentsViewModel();
                    asstViewModel.StudentUsrId = (int)model.StudentAsstId;
                    asstViewModel.AssignmentsSubmitDate = DateTime.UtcNow.AddHours(4);
                    asstViewModel.sAsstId = model.AsstId;

                    asstViewModel.AssignmentsSubmitFileName = strFileName;
                    asstViewModel.AssignmentsSubmitFileRef = Guid.NewGuid().ToString();


                    response = client.PostAsJsonAsync<StudentAssignmentsViewModel>("Student/AddStudentAssignmentsAsync", asstViewModel).Result;
                    List<ReturnObject> AssignmentList = response.Content.ReadAsAsync<List<ReturnObject>>().Result;
                    if (response.IsSuccessStatusCode)
                    {
                       
                    }
                    else
                    {
                      
                    }
                }

            }

                return RedirectToAction("DashboardStudent", "Admin");

        }

        [ValidateAntiForgeryToken]
        public IActionResult ViewAssignmentByStudent(string mode, long AssignmentId, long StudentId)
        {

            ReturnObject returnObject = StudentAnsweredAssignmentsByRef(AssignmentId, StudentId);
            if (returnObject.assignmentsForStudentsViewModel != null)
            {

                AssignmentsForStudentsViewModel assignmentsForStudentsViewModel = returnObject.assignmentsForStudentsViewModel;
                assignmentsForStudentsViewModel.mode = mode;
                assignmentsForStudentsViewModel.pAssignmentsId = AssignmentId;
                assignmentsForStudentsViewModel.pStudentId = StudentId;
                assignmentsForStudentsViewModel.title = "Assignment View";
                return PartialView("StudentAssignmentEvaluation", assignmentsForStudentsViewModel);
            }
            else
            {
                return PartialView("ExcpetionPartial");
            }

        }





        #endregion

    }
}
