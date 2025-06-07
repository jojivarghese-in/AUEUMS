using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class ReturnObject
    {
        public bool result { get; set; }
        public string pkID { get; set; }
        public string Usermessage { get; set; }
        public string SystemExceptionMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public long? isTimetableExists { get; set; }
        public UnitViewModel UnitViewModel { get; set; }
        public List<UnitViewModel> UnitViewModels { get; set; }
        public NameValueMasterViewModel NameValueMasterViewModel { get; set; }
        public List<NameValueMasterViewModel> NameValueMasterViewModels { get; set; }
        public StatusViewModel StatusViewModel { get; set; }
        public List<StatusViewModel> StatusViewModels { get; set; }
        public List<UserCompanyAccess> UserCompanyAccessList { get; set; }
        public ReportViewModel ReportViewModel { get; set; }
        public List<ReportViewModel> ReportViewModels { get; set; }
        public Company Company { get; set; }
        public List<Company> Companies { get; set; }     
        public RoleListViewModel rolelistViewModel { get; set; }
        public List<RoleListViewModel> rolelistViewModels { get; set; }
        public List<EnumValueViewModel> enumValueViewModels { get; set; }   
        public AUEUMSRegisterViewModel AUEUMSUserViewModel { get; set; }
        public List<AUEUMSRegisterViewModel> AUEUMSUserViewModels { get; set; }
        public AUEUMSUserDashboardViewModel AUEUMSUserDashboardViewModel { get; set; }
        public AUEUMSUserListDashboardViewModel AUEUMSUserListDashboardViewModel { get; set; }
        public CollegeProgramViewModel collegeProgramViewModel { get; set; }
        public List<CollegeProgramViewModel> collegeProgramViewModels { get; set; }
        public CollegeViewModel collegeViewModel { get; set; }
        public List<CollegeViewModel> collegeViewModels { get; set; }
        public ProgramMajorViewModel programMajorViewModel { get; set; }
        public List<ProgramMajorViewModel> programMajorViewModels { get; set; }
        public AssignmentsViewModel assignmentsViewModel { get; set; }
        public List<AssignmentsViewModel> assignmentsViewModels { get; set; }
        public StudentAssignmentsViewModel studentAssignmentsViewModel { get; set; }
        public List<StudentAssignmentsViewModel> studentAssignmentsViewModels { get; set; }
        public AssignmentsForStudentsViewModel assignmentsForStudentsViewModel { get; set; }
        public List<AssignmentsForStudentsViewModel> assignmentsForStudentsViewModels { get; set; }
        public TimetableHeaderViewModel timetableHeaderViewModel { get; set; }
        public List<TimetableHeaderViewModel> timetableHeaderViewModels { get; set; }
        public TimetableHeaderListViewModel timetableHeaderListViewModel { get; set; }
        public TimetableDetailViewModel timetableDetailViewModel { get; set; }
        public List<TimetableDetailViewModel> timetableDetailViewModels { get; set; }
        public TimetableDetailListViewModel timetableDetailListViewModel { get; set; }
        public RoomViewModel RoomViewModel { get; set; }
        public List<RoomViewModel> RoomViewModels { get; set; }
        public YearViewModel YearViewModel { get; set; }
        public List<YearViewModel> YearViewModels { get; set; }
        public SemesterTypeViewModel SemesterTypeViewModel { get; set; }
        public List<SemesterTypeViewModel> SemesterTypeViewModels { get; set; }
        public TimeSlotViewModel timeSlotViewModel { get; set; }
        public List<TimeSlotViewModel> timeSlotViewModels { get; set; }
        public CourseViewModel courseViewModel { get; set; }
        public List<CourseViewModel> courseViewModels { get; set; }
        public CourseSectionViewModel courseSectionViewModel { get; set; }
        public List<CourseSectionViewModel> courseSectionViewModels { get; set; }
        public CourseSectionListViewModel courseSectionListViewModel { get; set; }
        public DaysViewModel daysViewModel { get; set; }
        public List<DaysViewModel> daysViewModels { get; set; }
        public DashboardReportStudentAssignments dashboardReportStudentAssignments { get; set; }
        public List<DashboardReportStudentAssignments> dashboardReportStudentAssignmentslist { get; set; }
        public DashboardReportStudentAssignmentsList ReportStudentAssignmentsList { get; set; }
        public AssignmentListDashboardViewModel assignmentListDashboardViewModel { get; set; }
    }
}
