﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AUEUMS.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
        {
            public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
        }
   
}
