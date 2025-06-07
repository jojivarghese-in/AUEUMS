using AUEUMS.Models;

namespace AUEUMS.View_Models
{
    public class NameValueMasterViewModel : NameValueMaster
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
