using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AUEUMS.View_Models
{
    public class DashboardReportStudentAssignments
    {
        public int? mStudentUsrId { get; set; }
        public long? mAssignmentId { get; set; }
        public string mAssignmentTitle { get; set; }
        public string mGradingCriteria { get; set; }
        public int mScoreRange { get; set; }

        public string DisplayAssignmentTitle
        {
            get
            {
                if (mAssignmentTitle != null)
                {
                    if (mAssignmentTitle.Length > 100)
                    {
                        return mAssignmentTitle.Substring(0, 95) + "....";

                    }
                    else
                    {
                        return mAssignmentTitle;
                    }
                }
                else
                    return "";
            }
        }
        public string DisplayScore
        {
            get
            {
                if (mScoreRange != 0)
                {
                    return "Score - "+ mScoreRange;
                }
                else
                    return "";
            }
        }
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
