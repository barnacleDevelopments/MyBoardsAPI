using System.ComponentModel.DataAnnotations.Schema;

namespace HangboardTrainingAPI.Models.StatisticModels
{
    [NotMapped]
    public class DayTrainTime
    {
        public DayOfWeek Day { get; set; }
        public double Seconds { get; set; }
    }
}
