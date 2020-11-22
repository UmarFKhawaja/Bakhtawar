using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Option
    {
        public string Id { get; set; }
        
        public string PollId { get; set; }

        public Poll Poll { get; set; }
        
        public ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
    }
}