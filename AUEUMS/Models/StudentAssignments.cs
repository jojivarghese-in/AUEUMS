using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class StudentAssignments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string StudentId { get; set; }
        public int? StudentUsrId { get; set; }
        public Assignments Assignments { get; set; }
        public string AssignmentsRef { get; set; }
        public string AssignmentsStatus { get; set; }
        public DateTime? AssignmentsSubmitDate { get; set; }
        public string AssignmentsSubmitFileName { get; set; }
        public string AssignmentsSubmitFileRef { get; set; }
        public string GradingCriteria { get; set; }
        public int? ScoreRange { get; set; }
        public string Remarks { get; set; }        

      

    }
}
