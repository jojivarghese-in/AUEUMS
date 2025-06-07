using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class Address
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long AddressID { get; set; }
		public NameValueMaster Country { get; set; }
		public NameValueMaster Emirate { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public NameValueMaster Location { get; set; }
		public string POBox { get; set; }
		public string Website { get; set; }
		public DateTime? CreatedDate { get; set; }
		public User UserCreatedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public User UserModifiedBy { get; set; }
		public bool? DeActivated { get; set; }
		public string AddressText { get; set; }
		public string Area { get; set; }
	}
}
