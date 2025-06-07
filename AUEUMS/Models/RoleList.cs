using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace AUEUMS.Models
{
    public class Role
    {  
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string brCode { get; set; }
        public ICollection<UserRole> UsersRole { get; set; } = new Collection<UserRole>();
    }
}
