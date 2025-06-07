using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class Assignments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string AssignmentTitle { get; set; }
        public string AssignmentDescription { get; set; }
        public DateTime? DueDateTime { get; set; }
        public DateTime? PostedDateTime { get; set; }
        public string UploadFileRefNo { get; set; }
        public string UploadFileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public User UserCreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public User UserModifiedBy { get; set; }
      

    }
}
