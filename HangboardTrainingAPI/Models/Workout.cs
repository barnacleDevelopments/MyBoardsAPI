namespace MyBoardsAPI.Models
{
    public class Workout : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        public int HangboardId { get; set; }    

        public string? UserId { get; set; } = string.Empty;
        
        public List<Set> Sets { get; set; } = new List<Set>();

        public Hangboard? Hangboard { get; set; }

        public List<Session> Sessions { get; set; } = new List<Session>();
        
    }
}
