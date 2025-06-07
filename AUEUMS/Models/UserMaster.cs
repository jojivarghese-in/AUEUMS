using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class UserMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string UserManager { get; set; }
        public string PhoneNo { get; set; }
        public string Designation { get; set; }
        public string Extension { get; set; }
        public Department Department { get; set; }
        public bool? DeActivated { get; set; }
        public string AuthToken { get; set; }
        public string ImgUrl { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; }


    }
}
