using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.View_Models.UserMaster
{
    public class UserMasterListViewModel
    {
        public List<UserMasterViewModel> usrlists;
        public string mode { get; set; }
        public string src { get; set; }
        public int UserId { get; set; }
        public string ctrlid { get; set; }        

    }
}
