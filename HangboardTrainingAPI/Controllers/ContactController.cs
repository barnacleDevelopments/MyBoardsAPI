using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models;
using MyBoardsAPI.Models.Auth;
using MyBoardsAPI.Services;

namespace MyBoardsAPI.Controllers;
public class ContactController : Controller
{
    private readonly MailService _mail;
    private readonly IConfiguration _configuration;

    public ContactController(MailService mail, IConfiguration configuration)
    {
        _mail = mail;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Contact(Email? email)
    {
        return View("Contact", email);
    }
    
    [HttpPost]
    public async Task<IActionResult> PostForm(Email email)
    {
        try
        {
            /*await _mail.SendMail(
                _configuration["MailSettings:SenderAddress"],
                $"MyBoards - {email.Subject}",
                email.Message);*/
            return Redirect(nameof(SuccessMessage));
        }
        catch
        {
            return View("ContactFail", email);
        }
    }

    [HttpGet]
    public IActionResult SuccessMessage(Email email)
    {
        return View("ContactSuccess");
    }
}