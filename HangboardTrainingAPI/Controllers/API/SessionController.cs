using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MyBoardsAPI.Models;
using MyBoardsAPI.Models.Auth;

namespace MyBoardsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ILogger<WorkoutController> _logger;
        private readonly MyBoardsDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public SessionController(ILogger<WorkoutController> logger, MyBoardsDbContext db, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> LogSessionAsync(Session session)
        {
            #region
            try
             {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // create session 
                var newSession = new Session()
                {
                    DateCompleted = DateTime.Now,
                    WorkoutId = session.WorkoutId,
                    UserId = userId
                };

                await _db.Sessions.AddAsync(newSession);

                await _db.SaveChangesAsync();

                // get the associated workout 
                var workout = await _db.Workouts
                    .Include(workout => workout.Sets)
                    .FirstOrDefaultAsync(workout => workout.Id == session.WorkoutId);

                // loop over the sets and create performed sets 
                if (workout != null && workout.Sets != null)
                {
                    newSession.PerformedSets = workout.Sets.Select(set => new PerformedSet
                    {
                        SessionId = newSession.Id,
                        LeftGripType = set.LeftGripType,
                        RightGripType = set.RightGripType,
                        PerformedReps = session.RepLog
                            .Where(performedRep => performedRep.SetId == set.Id)
                            .ToList(),
                    }).ToList();
                }

                _db.Sessions.Update(newSession);

                await _db.SaveChangesAsync();

                return Ok();

            } catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the LogSessionAsync method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }
    }
}
