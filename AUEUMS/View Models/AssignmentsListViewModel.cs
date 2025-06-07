using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;


namespace AUEUMS.View_Models
{
    public class AssignmentsListViewModel
    {
        public List<AssignmentsViewModel> assignments;
        public string mode { get; set; }
        public string src { get; set; }
        public long AssignmentsId { get; set; }
        public string ctrlid { get; set; }
    
    }
}
