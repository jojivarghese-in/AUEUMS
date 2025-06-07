using AUEUMS.Models;
using System.Collections.Generic;
using System;

namespace AUEUMS.View_Models
{
    public class UnitViewModel : Unit
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
    public class ReportViewModel
    {
        public string Type { get; set; }
        public string prj { get; set; }
        public long prjId { get; set; }
        public DateTime? OpDate { get; set; }
        public string Brand { get; set; }
        public User ModifiedUser { get; set; }
        public string OpDateDisplay
        {
            get
            {
                if (OpDate != null)
                {
                    return ((DateTime)OpDate).Day + "/" + ((DateTime)OpDate).Month + "/" + ((DateTime)OpDate).Year;
                }
                else
                    return "";
            }
        }
        public string opListClass
        {
            get
            {
                if (Type == "Quoted")
                    return "badge badge-primary";
                else if (Type == "Won")
                    return "badge badge-success";
                else if (Type == "Lost")
                    return "badge badge-danger";
                else if (Type == "Listed")
                    return "badge badge-primary";
                else
                    return "badge badge-warning";

            }
        }
        public string opTypeClass
        {
            get
            {
                if (Type == "Quoted")
                    return "badge badge-dot badge-dot-xl badge-primary";
                else if (Type == "Won")
                    return "badge badge-dot badge-dot-xl badge-success";
                else if (Type == "Lost")
                    return "badge badge-dot badge-dot-xl badge-danger";
                else if (Type == "Listed")
                    return "badge badge-dot badge-dot-xl badge-primary";
                else
                    return "badge badge-dot badge-dot-xl badge-warning";

            }
        }
        public string opTypeHeader
        {
            get
            {
                if (Type == "Quoted")
                    return "Quoted for " + Brand;
                else if (Type == "Won")
                    return Brand + " Won Business";
                else if (Type == "Lost")
                    return Brand + " Lost Business";
                else if (Type == "Listed")
                    return Brand + " is listed";
                else
                    return Brand + " declined business";

            }
        }

        public string opTypeHeaderClass
        {
            get
            {
                if (Type == "Quoted")
                    return "timeline-title text-primary";
                else if (Type == "Won")
                    return "timeline-title text-success";
                else if (Type == "Lost")
                    return "timeline-title text-danger";
                else if (Type == "Listed")
                    return "timeline-title text-primary";
                else
                    return "timeline-title text-warning";

            }
        }
    }
    public class ReportView
    {

        public List<ReportViewModel> reportViewModels { get; set; }

    }

    public class DashboardView
    {
        public List<UserCompanyAccess> userCompanyAccess { get; set; }

        public List<Company> Companies { get; set; }      
        public int? CompanyCode { get; set; }
    }
   
}
