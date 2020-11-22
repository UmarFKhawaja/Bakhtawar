using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Poll
    {
        public string Id { get; set; }
        
        public PollPost PollPost { get; set; }

        public ICollection<Option> Options { get; set; } = new HashSet<Option>();
    }
}