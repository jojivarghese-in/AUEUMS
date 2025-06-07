using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.AspNetCore.Http;

namespace AUEUMS.View_Models
{
    public class StudentAssignmentsViewModel : StudentAssignments
    {
  
        public IFormFile File { get; set; }
        public string mode { get; set; }
        public string title { get; set; }
        public string modifiedDateString { get; set; }
        public long? sAsstId { get; set; }
        public long? sStudentAsstId { get; set; }

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
