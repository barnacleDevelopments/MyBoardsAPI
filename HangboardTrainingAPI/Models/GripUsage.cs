using System.ComponentModel.DataAnnotations;

namespace MyBoardsAPI.Models
{
    public class GripUsage : BaseModel
    {
        [Range(0, 100)]
        public double HalfCrimp { get; set; }
        [Range(0, 100)]
        public double OpenHand { get; set; }
        [Range(0, 100)]
        public double FullCrimp { get; set; }
    }
}
