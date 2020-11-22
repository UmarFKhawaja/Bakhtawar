using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Post
    {
        public string Id { get; set; }
        
        public string PersonaId { get; set; }
        
        public Persona Persona { get; set; }
        
        public ICollection<PostTag> PostTags { get; set; } = new HashSet<PostTag>(); 

        public ICollection<PostKeyword> PostKeywords { get; set; } = new HashSet<PostKeyword>(); 
    }
}