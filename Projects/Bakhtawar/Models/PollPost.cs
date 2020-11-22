namespace Bakhtawar.Models
{
    public class PollPost : Post
    {
        public string PollId { get; set; }
        
        public Poll Poll { get; set; }
    }
}