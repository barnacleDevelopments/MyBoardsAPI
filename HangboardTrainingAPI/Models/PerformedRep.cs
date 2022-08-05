using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HangboardTrainingAPI.Models
{
    public class PerformedRep : BaseModel
    {
        public int PercentageCompleted { get; set; }
        public double SecondsCompleted { get; set; }
        public int RepIndex { get; set; }
        public int PerformedSetId { get; set; }
        [JsonIgnore]
        public PerformedSet? PerformedSet { get; set;}
        [NotMapped]
        public int SetId { get; set; }
   
    }

}
