using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class ProgramMajor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string ProgramMajorName { get; set; }
        public CollegeProgram CollegeProgram { get; set; }
        public string ProgramMajorCode { get; set; }

      
    }
}
