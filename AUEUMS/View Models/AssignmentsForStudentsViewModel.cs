using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.AspNetCore.Http;

namespace AUEUMS.View_Models
{
    public class AssignmentsForStudentsViewModel
    {
        public int? mStudentUsrId { get; set; }
        public long? mAssignmentId { get; set; }
        public string mAssignmentsRef { get; set; }
        public User mStudentNoRef { get; set; }
        public string mAssignmentsFileName { get; set; }
        public string mAssignmentsSubmitFileRef { get; set; }
        public string mAssignmentsSubmitFileName { get; set; }
        public string mAssignmentTitle { get; set; }
        public string mAssignmentDescription { get; set; }
        public string mGradingCriteria { get; set; }
        public int mScoreRange { get; set; }
        public string mRemarks { get; set; }
        public DateTime? mDueDateTime { get; set; }
        public DateTime? mPostedDateTime { get; set; }
        public DateTime? mAssignmentsSubmitDate { get; set; }
        public string mAssignmentsStatus { get; set; }

        public int displayDateExpiry
        {
            get
            {
                if (mDueDateTime > mPostedDateTime)

                    return 0;
                else
                    return 1;
            }
        }
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
        public string displaymDueDateTime
        {
            get
            {
                if (mDueDateTime != null)

                    return ((DateTime)mDueDateTime).Day + "/" + ((DateTime)mDueDateTime).Month + "/" + ((DateTime)mDueDateTime).Year;
                else
                    return "";
            }
        }
        public string displaymAssignmentsStatus
        {
            get
            {
                if (mAssignmentsStatus != null)

                    return mAssignmentsStatus;
                else
                    return "pending";
            }
        }
        public string displaymPostedDateTime
        {
            get
            {
                if (mPostedDateTime != null)
                    return ((DateTime)mPostedDateTime).Day + "/" + ((DateTime)mPostedDateTime).Month + "/" + ((DateTime)mPostedDateTime).Year;
                else
                    return "";
            }
        }
        public string displaymAssignmentsSubmitDate
        {
            get
            {
                if (mAssignmentsSubmitDate != null)
                    return ((DateTime)mAssignmentsSubmitDate).Day + "/" + ((DateTime)mAssignmentsSubmitDate).Month + "/" + ((DateTime)mAssignmentsSubmitDate).Year;
                else
                    return "";
            }
        }
        public string UsrFullName
        {
            get
            { 
                if (mStudentNoRef != null)
                {
                    if ((!string.IsNullOrEmpty(mStudentNoRef.FirstName)) && (!string.IsNullOrEmpty(mStudentNoRef.LastName)))
                    {
                        return mStudentNoRef.FirstName + ' ' + mStudentNoRef.LastName;
                    }
                    else
                    {
                        return mStudentNoRef.FirstName;
                    }
                }
                else
                {
                    return "";
                }

            }

        }
        public IEnumerable<NameValueMaster> SelGradingCriteria
        {
            get
            {
                List<NameValueMaster> GradingCriteriaTypes = new List<NameValueMaster>();
                GradingCriteriaTypes.Add(new NameValueMaster { DisplayValue = "Clarity", DisplayID = 1 });
                GradingCriteriaTypes.Add(new NameValueMaster { DisplayValue = "Completeness", DisplayID = 2 });

                return GradingCriteriaTypes;
            }
        }

        public IEnumerable<NameValueMaster> SelScorerange
        {
            get
            {
                List<NameValueMaster> ScorerangeTypes = new List<NameValueMaster>();
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "1", DisplayID = 1 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "2", DisplayID = 2 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "3", DisplayID = 3 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "4", DisplayID = 4 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "5", DisplayID = 5 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "6", DisplayID = 6 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "7", DisplayID = 7 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "8", DisplayID = 8 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "9", DisplayID = 9 });
                ScorerangeTypes.Add(new NameValueMaster { DisplayValue = "10", DisplayID = 10 });

                return ScorerangeTypes;
            }
        }

        public long? pAssignmentsId { get; set; }
        public long? pStudentId { get; set; }
        public string mode { get; set; }
        public string title { get; set; }
        public IFormFile File { get; set; }
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
