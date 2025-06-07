using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;


namespace AUEUMS.View_Models
{
    public class TimetableDetailListViewModel
    {
        public List<TimetableDetailViewModel> timetableDetails;
        public string mode { get; set; }
        public string src { get; set; }
        public string ctrlid { get; set; }
        public long? Hdrparamid { get; set; }
    
    }
}
