namespace Bakhtawar.Models
{
    public class PostTag
    {
        public string Id { get; set; }
        
        public string PostId { get; set; }
        
        public Post Post { get; set; }
        
        public string TagId { get; set; }
        
        public Tag Tag { get; set; }
    }
}