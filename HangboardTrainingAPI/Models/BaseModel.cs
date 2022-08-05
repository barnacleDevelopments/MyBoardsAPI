using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HangboardTrainingAPI.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        [Display(AutoGenerateField = false), JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [Display(AutoGenerateField = false), JsonIgnore]
        public DateTime UpdatedAt { get; set; }

    }
}
