namespace Bakhtawar.Models
{
    public class GalleryPost : Post
    {
        public string GalleryId { get; set; }
        
        public Gallery Gallery { get; set; }
    }
}