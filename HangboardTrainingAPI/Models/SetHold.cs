using HangboardTrainingAPI.Enums;
using HangboardTrainingAPI.Models;
using System.Text.Json.Serialization;

namespace MyBoardsAPI.Models
{
    public class SetHold
    {
        public int SetId { get; set; }
        [JsonIgnore]
        public Set? Set { get; set; }
        public int HoldId { get; set; }
        public Hold? Hold { get; set; }
        public Hand Hand { get; set; }
    }
}
