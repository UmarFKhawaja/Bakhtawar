namespace Bakhtawar.Models
{
    public class VideoPost : Post
    {
        public string VideoId { get; set; }
        
        public Video Video { get; set; }
    }
}