using System.Text.Json.Serialization;

namespace MyBoardsAPI.Models
{
    public class Hangboard : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public double BoardWidth { get; set; }
        public double BoardHeight { get; set; }
        public List<Hold> Holds { get; set; } = new List<Hold>();
        [JsonIgnore]
        public List<Workout>? Workouts { get; set; } = new List<Workout>();
        public bool IsDefaultBoard { get; set; }

    }
}
