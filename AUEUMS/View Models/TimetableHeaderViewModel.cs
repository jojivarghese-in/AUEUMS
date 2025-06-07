using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUEUMS.Models;
using Microsoft.AspNetCore.Http;

namespace AUEUMS.View_Models
{
    public class TimetableHeaderViewModel : tblTimetableHeader
    {

        public string mode { get; set; }
        public string title { get; set; }
        public string modifiedDateString { get; set; }
        public List<RoomViewModel> tblRoomlist { get; set; }
        public List<YearViewModel> tblYearlist { get; set; }
        public List<SemesterTypeViewModel> tblSemesterTypelist { get; set; }
      
        public long? RoomlistId { get; set; }
        public long? YearlistId { get; set; }
        public long? SemesterTypelistId { get; set; }
        public string TimetableDateString { get; set; }
        public string DayofWeekString { get; set; }

        public string displayDayofWeek
        {
            get
            {
                if (TDate != null)

                    return ((DateTime)TDate).ToString("dddd");
                else
                    return "";
            }
        }

        public string displayTimetableDate
        {
            get
            {
                if (TDate != null)

                    return ((DateTime)TDate).Day + "/" + ((DateTime)TDate).Month + "/" + ((DateTime)TDate).Year;
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
