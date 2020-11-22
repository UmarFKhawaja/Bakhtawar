namespace Bakhtawar.Models
{
    public class PostKeyword
    {
        public string Id { get; set; }
        
        public string PostId { get; set; }
        
        public Post Post { get; set; }
        
        public string KeywordId { get; set; }
        
        public Keyword Keyword { get; set; }
    }
}