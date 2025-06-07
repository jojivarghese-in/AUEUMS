using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AUEUMS.Models
{
    public class Company
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int CompanyID { get; set; }
		public string CompanyCode { get; set; }
		public string CompanyName { get; set; }
		public Address Address { get; set; }
		public Contact ContactPrimary { get; set; }
		public string LegalRegNumber { get; set; }
		public string VatNumber { get; set; }
		public NameValueMaster Type { get; set; }
		public NameValueMaster Size { get; set; }
		public DateTime? CreatedDate { get; set; }
		//public User UserCreatedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		//public User UserModifiedBy { get; set; }
		public bool? deleted { get; set; }
	}
}
