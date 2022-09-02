using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models.Auth;

namespace MyBoardsAPI.Controllers.API;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    } 

    [HttpGet]
    public async Task<IActionResult> GetCurrentAsync()
    {
        return Ok(await _userManager.GetUserAsync(User));
    }
}