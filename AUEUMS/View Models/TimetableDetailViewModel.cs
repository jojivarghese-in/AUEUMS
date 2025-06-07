using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.AspNetCore.Http;

namespace AUEUMS.View_Models
{
    public class TimetableDetailViewModel : tblTimetableDetail
    {

        public string mode { get; set; }
        public string title { get; set; }
        public string modifiedDateString { get; set; }
        public List<DaysViewModel> tblDayslist { get; set; }
        public List<TimeSlotViewModel> tblTimeSlotlist { get; set; }
        public List<CourseSectionViewModel> tblCourseSeclist { get; set; }
        public List<CourseViewModel> tblCourselist { get; set; }
        public CourseSectionListViewModel crsSecListViewModel { get; set; }
        public long? DayslistId { get; set; }
        public long? TimeSlotlistId { get; set; }
        public long? CourseSeclistId { get; set; }
        public long? CourselistId { get; set; }
        public long? TimeTableHdrParamId { get; set; }
        public string ToXML()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
        }
    }
}
