namespace Bakhtawar.Models
{
    public class Vote
    {
        public string Id { get; set; }
        
        public string PersonaId { get; set; }
        
        public Persona Persona { get; set; }
        
        public string OptionId { get; set; }
        
        public Option Option { get; set; }
    }
}