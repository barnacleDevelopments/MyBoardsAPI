using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models;
using MyBoardsAPI.Models.Auth;
using MyBoardsAPI.Services;

namespace MyBoardsAPI.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class ContactController : Controller
{
    private readonly MailService _mail;
    private readonly IConfiguration _configuration;

    public ContactController(MailService mail, IConfiguration configuration)
    {
        _mail = mail;
        _configuration = configuration;
    }
    
    // GET
    [HttpPost]
    public async Task<IActionResult> PostForm(Email email)
    {
        try
        {
            /*await _mail.SendMail(
                _configuration["MailSettings:SenderAddress"],
                $"MyBoards - {email.Subject}",
                email.Message);*/
            return Ok();
        }
        catch 
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { 
                Status = "Error", 
                Message = "There was an error sending your email. Please try again." 
            });
        }
    } 
}