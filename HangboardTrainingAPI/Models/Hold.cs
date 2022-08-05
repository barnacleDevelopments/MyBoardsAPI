using HangboardTrainingAPI.Enums;
using MyBoardsAPI.Models;
using System.Text.Json.Serialization;

namespace HangboardTrainingAPI.Models
{
    public class Hold : BaseModel
    {
        public int FingerCount { get; set; }
        public int DepthMM { get; set; }
        public double BaseUIXCoord { get; set; }
        public double BaseUIYCoord { get; set; }
        public int HangboardId { get; set; }
        [JsonIgnore]
        public Hangboard? Hangboard { get; set; }
        public int Index { get; set; }
        [JsonIgnore]
        public List<SetHold> SetHolds { get; set; } = new List<SetHold>();
    }
}
