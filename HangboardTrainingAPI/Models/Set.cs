using HangboardTrainingAPI.Enums;
using MyBoardsAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyBoardsAPI.Models
{
    public class Set : BaseModel
    {
        [MaxLength(50)]
        public string Instructions { get; set; } = string.Empty; 
        
        public string InstructionAudioURL { get; set; } = string.Empty;
        
        public int HangTime { get; set; }
        
        public int RestTime { get; set; }
        
        public int Reps { get; set; }
        
        public int RestBeforeNextSet { get; set; }
        
        public int Weight { get; set; }
        public int IndexPosition { get; set; }
        public GripType LeftGripType { get; set; }
        public GripType RightGripType { get; set; }
        public int WorkoutId { get; set; }
        [JsonIgnore]
        public Workout? Workout { get; set; }
        public List<SetHold> SetHolds { get; set; } = new List<SetHold>();
    }
}
