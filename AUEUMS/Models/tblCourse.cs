using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class tblCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int? LabHrs { get; set; }
        public int? LectureHrs { get; set; }
        public int? Capacity { get; set; }
     

    }
}
