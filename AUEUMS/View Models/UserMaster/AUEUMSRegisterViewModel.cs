using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class AUEUMSRegisterViewModel : AUEUMSUserRegister
    {

        public string UserNameDisplay
        {
            get
            {
                return (FirstName + ' ' + LastName);
            }
        }
       
        public string EmailVerifiedDisplay
        {
            get
            {
                if (EmailVerified != null)
                {
                    if (EmailVerified == false)
                    {
                        return "No";
                    }
                    else

                        return "Yes";
                }
                else
                {
                    return "";
                }
            }
        }
        public string InactiveDisplay
        {
            get
            {
                if (DeActivated != null)
                {
                    if (DeActivated == true)
                    {
                        return "No";
                    }
                    else
                        return "Yes";
                }
                else
                {
                    return "";
                }
            }
        }
        public User UsrInfo { get; set; }

        public List<CollegeViewModel> collegeList { get; set; }
        public long? collegeID { get; set; }
        public string collegeName { get; set; }

        public List<CollegeProgramViewModel> collegeProgramList { get; set; }
        public long? collegeProgramID { get; set; }
        public string collegeProgramName { get; set; }

        public List<ProgramMajorViewModel> programMajorList { get; set; }
        public string programMajorName { get; set; }
        public long? programMajorID { get; set; }

        public string UsrId
        {
            get;
            set;
        }
        public List<RoleListViewModel> rolelist { get; set; }
        public int? RoleListID
        {
            get;
            set;
        }
    
        public string mode { get; set; }
        public string src { get; set; }
        public string ctrlid { get; set; }
        public string title { get; set; }
        public string confirmPwd { get; set; }
        public string ReturnUrl { get; set; }
        public string InactiveDisplayFlag { get; set; }

        public string errormessage { get; set; }

        public string ToXML()
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this);
                return stringwriter.ToString();
            }
        }

    }
}
