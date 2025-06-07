using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class Department
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DepartmentID { get; set; }
		public string DepartmentCode { get; set; }
		public string DepartmentName { get; set; }
		public int? DepartmentOwner { get; set; }
		public int? HODEmployeeCode { get; set; }
		public Company Company { get; set; }
		public bool? IsActive { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
	}
}
