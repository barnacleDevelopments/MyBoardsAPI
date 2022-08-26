using HangboardTrainingAPI.Models;

namespace HangboardTrainingAPI.Models
{
    public class Feature : BaseModel
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
    }
}