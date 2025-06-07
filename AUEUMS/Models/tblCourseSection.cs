using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class tblCourseSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public tblCourse tblCourse { get; set; }
        public long? facultyId { get; set; }
        public string ClassName { get; set; }
        public string SectionNo { get; set; }
        public int? SeatAllowed { get; set; }
     

    }
}
