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

    public async Task<IActionResult> SuccessRegistration()
    {
        return View("SuccessRegistration");
    }
}