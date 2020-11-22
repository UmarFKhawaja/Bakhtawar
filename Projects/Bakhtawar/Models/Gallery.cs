using System.Collections.Generic;

namespace Bakhtawar.Models
{
    public class Gallery
    {
        public string Id { get; set; }
        
        public GalleryPost GalleryPost { get; set; }

        public ICollection<Image> Images { get; set; } = new HashSet<Image>();
    }
}