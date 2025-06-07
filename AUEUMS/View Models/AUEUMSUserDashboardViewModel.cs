using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class AUEUMSUserDashboardViewModel
    {
        public User UsrInfo { get; set; }
        public AUEUMSUserRegister umsRegister { get; set; }

        public string roleName { get; set; }

        public string InactiveDisplay
        {
            get
            {
                if (UsrInfo.DeActivated == true)
                {
                    return "No";
                }
                else
                    return "Yes";
            }
        }
        public string SpUsrFullName
        {
            get
            {
                if ((!string.IsNullOrEmpty(UsrInfo.FirstName)) && (!string.IsNullOrEmpty(UsrInfo.LastName)))
                {
                    return UsrInfo.FirstName + ' ' + UsrInfo.LastName;
                }
                else
                {
                    return UsrInfo.FirstName;
                }

            }

        }



    }
}
