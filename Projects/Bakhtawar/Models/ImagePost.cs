namespace Bakhtawar.Models
{
    public class ImagePost : Post
    {
        public string ImageId { get; set; }
        
        public Image Image { get; set; }
    }
}