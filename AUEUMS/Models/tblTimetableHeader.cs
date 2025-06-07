using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class tblTimetableHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public tblSemesterType tblSemesterType { get; set; }
        public tblYear tblYear { get; set; }
        public tblRoom tblRoom { get; set; }
        public DateTime? TDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public User UserCreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public User UserModifiedBy { get; set; }
    }
}
