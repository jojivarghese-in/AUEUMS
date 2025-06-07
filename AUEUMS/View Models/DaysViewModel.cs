using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class DaysViewModel : tblDays
    {
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
