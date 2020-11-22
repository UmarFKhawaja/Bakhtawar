using System;
using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class UserClaim
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public User User { get; set; }
    }
}