using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AUEUMS.Models
{
    public class UserCompanyAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserCompanyAccessID { get; set; }
        public User user { get; set; }
        public Company company { get; set; }        
    }
}