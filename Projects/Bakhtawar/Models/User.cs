using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Bakhtawar.Models
{
    public class User : IdentityUser
    {
        // public string Id { get; set; }

        // public string UserName { get; set; }

        // public string NormalizedUserName { get; set; }

        // public string Email { get; set; }

        // public string NormalizedEmail { get; set; }

        // public bool EmailConfirmed { get; set; }

        // public string PasswordHash { get; set; }

        // public string PhoneNumber { get; set; }

        // public bool PhoneNumberConfirmed { get; set; }

        // public bool TwoFactorEnabled { get; set; }

        // public DateTime LockoutEnd { get; set; }

        // public bool LockoutEnabled { get; set; }

        // public int AccessFailedCount { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        public ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();

        public ICollection<UserLogin> UserLogins { get; set; } = new HashSet<UserLogin>();
        
        public ICollection<Persona> Personas { get; set; } = new HashSet<Persona>();
    }
}