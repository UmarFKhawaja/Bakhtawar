namespace Bakhtawar.Models
{
    public class Image
    {
        public string Id { get; set; }
        
        public ImagePost ImagePost { get; set; }

        public string GalleryId { get; set; }
        
        public Gallery Gallery { get; set; }
    }
}