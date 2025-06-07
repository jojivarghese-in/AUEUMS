using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace AUEUMS.Models
{
    public class UsrForgotPassword
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public string errormessage { get; set; }
    }
}
