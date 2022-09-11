namespace MyBoardsAPI.Models
{
    public class Session : BaseModel
    {
        public DateTime DateCompleted { get; set; }
        public List<PerformedRep> RepLog { get; set; } = new List<PerformedRep>();
        public List<PerformedSet> PerformedSets { get; set; } = new List<PerformedSet>();
        public int? WorkoutId { get; set; }
        public Workout? Workout { get; set; }
        public string? UserId { get; set; }
    }
}
