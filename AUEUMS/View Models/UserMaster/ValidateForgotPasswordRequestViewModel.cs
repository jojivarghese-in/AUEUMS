using System;
using System.Collections.Generic;
using System.Linq;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class ValidateForgotPasswordRequestViewModel
    {
        public string secretKey { get; set; }
        public string mode { get; set; }
        public string src { get; set; }
        public string ctrlid { get; set; }
        public string title { get; set; }
        public string ReturnUrl { get; set; }

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
