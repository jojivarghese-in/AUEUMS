using System;
using System.Collections.Generic;

namespace AUEUMS.Models
{
    public partial class Roles
    {
        public Roles()
        {
            User = new HashSet<User>();
        }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
