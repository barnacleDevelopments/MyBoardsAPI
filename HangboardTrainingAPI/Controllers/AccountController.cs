using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models.Auth;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using MyBoardsAPI.Services;

namespace MyBoardsAPI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly MailService _mail;
    
    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        MailService mail)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _mail = mail;
    }

    public IActionResult SuccessRegistration()
    {
        return View("SuccessRegistration");
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        var model = new PasswordReset { Token = token, Email = email };
        return View("ResetPassword", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(PasswordReset passwordReset)
    {
        if (!ModelState.IsValid)
            return View(passwordReset);
        
        var user = await _userManager.FindByEmailAsync(passwordReset.Email);
        
        if (user == null)
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        
        var resetPassResult = await _userManager.ResetPasswordAsync(user, passwordReset.Token, passwordReset.Password);
        
        if(!resetPassResult.Succeeded)
        {
            foreach (var error in resetPassResult.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View();
        }
        
        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }
    
    public IActionResult ResetPasswordConfirmation()
    {
        return View("SuccessPasswordReset");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return View("ConfirmEmailFail");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        
        return View("ConfirmEmailSuccess");
    }
}