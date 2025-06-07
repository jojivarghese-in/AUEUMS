using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.View_Models
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string ccEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public int? EmployeeId { get; set; }
     }
}
