using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;


namespace AUEUMS.View_Models
{
    public class TimetableHeaderListViewModel
    {
        public List<TimetableHeaderViewModel> timetableHeaders;     
        public string mode { get; set; }
        public string src { get; set; }
        public string ctrlid { get; set; }
    
    }
}
