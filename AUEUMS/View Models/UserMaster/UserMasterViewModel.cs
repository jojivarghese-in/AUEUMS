using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class UserMasterViewModel : User
    {
        public UserRole UserRoleList { get; set; }
        public string mode { get; set; }
        public string title { get; set; }
        public string DeactivatedString { get; set; }
        public string InactiveDisplay
        {
            get
            {
                if (DeActivated == true)
                {
                    return "No";
                }
                else
                    return "Yes";
            }
        }
       
    }
}
