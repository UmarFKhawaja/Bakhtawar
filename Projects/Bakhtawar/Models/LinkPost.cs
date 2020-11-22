namespace Bakhtawar.Models
{
    public class LinkPost : Post
    {
        public string LinkId { get; set; }
        
        public Link Link { get; set; }
    }
}