using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.Models
{
    public class error
    {
        public int ErrorID { get; set; }
        public string exception { get; set; }
        public string InnerError { get; set; }
        public string Module { get; set; }
        public string Method { get; set; }
        public string param { get; set; }
        public string stackTrace { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public bool? status { get; set; }
        public string note { get; set; }
        public string resolvedBy { get; set; }
    }
}
