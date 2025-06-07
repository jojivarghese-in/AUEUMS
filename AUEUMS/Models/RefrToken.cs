using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AUEUMS.Models
{
    public class RefrToken
    {
        [Key]
        public string Token { get; protected set; }
        public long Expiration { get; protected set; }
    }
}
