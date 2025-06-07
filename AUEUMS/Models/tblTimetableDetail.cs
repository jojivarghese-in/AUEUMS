using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class tblTimetableDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public tblTimetableHeader tblTimetableHeader { get; set; }
        public tblCourse tblCourse { get; set; }
        public tblCourseSection tblCourseSection { get; set; }
        public tblTimeSlot tblTimeSlot { get; set; }
        public string TimeSlotName { get; set; }
        public tblDays tblDays { get; set; }
        public DateTime? CreatedDate { get; set; }
        public User UserCreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public User UserModifiedBy { get; set; }
    }
}
