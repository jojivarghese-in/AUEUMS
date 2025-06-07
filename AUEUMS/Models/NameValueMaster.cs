using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class NameValueMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DisplayID { get; set; }
		public string TypeName { get; set; }
		public string DisplayValue { get; set; }
		public int DisplayOrder { get; set; }
		public bool DeActivated	{ get; set; }

		public DateTime CreatedDate { get; set; }
		public User UserCreatedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public User UserModifiedBy { get; set; }
	}
}
