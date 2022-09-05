using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Models.Auth;

namespace HangboardTrainingAPI.Controllers.API;

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