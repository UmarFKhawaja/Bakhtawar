using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Persona
    {
        public string Id { get; set; }
        
        public string UserId { get; set; }
        
        public User User { get; set; }
        
        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        
        public ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
    }
}