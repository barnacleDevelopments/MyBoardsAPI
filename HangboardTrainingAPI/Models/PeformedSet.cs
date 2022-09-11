using HangboardTrainingAPI.Enums;
namespace MyBoardsAPI.Models
{
    public class PerformedSet : BaseModel
    {
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public GripType LeftGripType { get; set; }
        public GripType RightGripType { get; set; }
        public List<PerformedRep> PerformedReps { get; set; } = new List<PerformedRep>();
    }
}
