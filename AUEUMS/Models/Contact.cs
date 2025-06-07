using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class Contact
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ContactID { get; set; }

		public string VenturesId { get; set; }
		public Address Address { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public NameValueMaster Gender { get; set; }
		public NameValueMaster Position { get; set; }
		public string ContactNumber { get; set; }
		public string Landline { get; set; }
		public string Extension { get; set; }
		public string POBox { get; set; }
		public string Website { get; set; }
		public bool? DeActivated { get; set; }
		public bool? isPrimary { get; set; }
		public DateTime? CreatedDate { get; set; }
		public User UserCreatedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public User UserModifiedBy { get; set; }
	}
}
