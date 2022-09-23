using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models.Auth;

namespace MyBoardsAPI.Controllers.API;

[Authorize]
[ApiController]
[Area("api")]
[Route("[area]/[controller]")]
public class ApiAccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApiAccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    } 

    [HttpGet("user-info")]
    public async Task<IActionResult> GetCurrentAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        
        return Ok(new
        {
            user.HasCreatedFirstHangboard,
            user.HasCreatedFirstWorkout,
            user.UserName
        });
    }
}