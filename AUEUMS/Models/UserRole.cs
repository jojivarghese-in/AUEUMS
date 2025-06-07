using System.ComponentModel.DataAnnotations.Schema;

namespace AUEUMS.Models
{
    public class UserRole
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Roles Role { get; set; }
    }
}
