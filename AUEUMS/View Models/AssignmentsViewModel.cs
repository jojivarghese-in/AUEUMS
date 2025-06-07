using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.AspNetCore.Http;

namespace AUEUMS.View_Models
{
    public class AssignmentsViewModel : Assignments
    {

        public string DueDateTimeString { get; set; }
        public string PostedDateTimeString { get; set; }
        public IFormFile File { get; set; }
        public long? AsstId { get; set; }
        public long? StudentAsstId { get; set; }
        public string displayDueDateTime
        {
            get
            {
                if (DueDateTime != null)
                  
                    return ((DateTime)DueDateTime).Day + "/" + ((DateTime)DueDateTime).Month + "/" + ((DateTime)DueDateTime).Year;
                else
                    return "";
            }
        }

        public string displayPostedDateTime
        {
            get
            {
                if (PostedDateTime != null)
                    return ((DateTime)PostedDateTime).Day + "/" + ((DateTime)PostedDateTime).Month + "/" + ((DateTime)PostedDateTime).Year;
                else
                    return "";
            }
        }
        public string mode { get; set; }
        public string title { get; set; }
        public string modifiedDateString { get; set; }


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
