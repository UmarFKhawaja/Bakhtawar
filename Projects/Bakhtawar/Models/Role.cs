using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Bakhtawar.Models
{
    public class Role : IdentityRole
    {
        // public string Id { get; set; }

        // public string Name { get; set; }

        // public string NormalizedName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        public ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
    }
}