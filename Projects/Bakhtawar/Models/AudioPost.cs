namespace Bakhtawar.Models
{
    public class AudioPost : Post
    {
        public string AudioId { get; set; }
        
        public Audio Audio { get; set; }
    }
}