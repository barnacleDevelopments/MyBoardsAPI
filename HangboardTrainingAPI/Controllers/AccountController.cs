using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models.Auth;
using System.Text.Encodings.Web;

namespace MyBoardsAPI.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    
    public AccountController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return View("Error");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        
        return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
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
    /* TODO: Remove For Production [ValidateAntiForgeryToken]*/
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
}