using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Keyword
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public ICollection<PostKeyword> PostKeywords { get; set; } = new HashSet<PostKeyword>();
    }
}