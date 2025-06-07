using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.View_Models
{
    public class AUEUMSRegisterListViewModel
    {
        public List<AUEUMSRegisterViewModel> registers;
        public string mode { get; set; }
        public string src { get; set; }
        public long Id { get; set; }
        public string UserNameDisplay { get; set; }
        public string ctrlid { get; set; }
     

    }
}
