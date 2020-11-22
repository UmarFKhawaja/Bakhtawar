using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Tag
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>();
    }
}