using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AUEUMS.Models
{
    public class Status
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Module { get; set; }
		public string StatusName { get; set; }
		public int DisplayOrder { get; set; }

	}

}
