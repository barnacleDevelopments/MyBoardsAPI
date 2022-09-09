using System.ComponentModel.DataAnnotations;

namespace MyBoardsAPI.Models;

public class Email
{
    [Required]
    public string Subject { get; set; } = String.Empty;
    [Required]
    public string Message { get; set; } = String.Empty;

}