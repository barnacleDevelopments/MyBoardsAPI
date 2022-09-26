using HangboardTrainingAPI.Enums;
using System.Text.Json.Serialization;

namespace MyBoardsAPI.Models
{
    public class PerformedSet : BaseModel
    {
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public GripType LeftGripType { get; set; }
        public GripType RightGripType { get; set; }
        public List<PerformedRep> PerformedReps { get; set; } = new List<PerformedRep>();
        public int SetId { get; set; }
        [JsonIgnore]
        public Set? Set { get; set; }
    }
}
