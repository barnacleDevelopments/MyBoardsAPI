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

        [HttpGet("GroupedByMonth/{year}")]
        public async Task<IActionResult> GetSessionGroupedByMonth(int year)
        {
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
                var sessions = await _db.Sessions
                    .Where(s => s.UserId == userId && s.DateCompleted.Year == year)
                    .OrderBy(s => s.DateCompleted).ToListAsync();

                var sessionsByMonth = sessions.GroupBy(s => s.DateCompleted.Month)
                    .Select(g => new
                    {
                        Year = year,
                        Month = g.Key,
                        Sessions = g
                    }).ToList();

                return Ok(sessionsByMonth);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetSessionGroupedByMonth method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        
        [HttpGet("Month/{month}")]
        public async Task<IActionResult> GetSessionsByMonth(int month)
        {
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
                var sessions = await _db.Sessions
                    .Where(s => s.UserId == userId && s.DateCompleted.Month == month)
                    .OrderBy(s => s.DateCompleted).ToListAsync();

                return Ok(sessions);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetSessionsByMonth method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                
                var sessions = await _db.Sessions
                    .Where(s => s.UserId == userId).FirstOrDefaultAsync(s => s.Id == id);

                return Ok(sessions);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetSession method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            
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
                        SetId = set.Id
                    }).ToList();
                }

                _db.Sessions.Update(newSession);

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    SessionId = newSession.Id
                });

            } catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the LogSessionAsync method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpPost("LogRepetition/{sessionId}")]
        public async Task<IActionResult> LogRepetition(int sessionId, PerformedRep rep)
        {
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var session = await _db.Sessions
                    .Include(s => s.PerformedSets)
                    .ThenInclude(s => s.PerformedReps)
                    .Include(s => s.RepLog)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (session == null)
                {
                    return StatusCode(500);
                }

                // get the associated workout 
                var workout = await _db.Workouts
                    .Include(workout => workout.Sets)
                    .FirstOrDefaultAsync(workout => workout.Id == session.WorkoutId);

                // loop over the sets and create performed sets 
                // TODO: how to add new performed rep to existing performed sets.
                session.PerformedSets = workout.Sets.Select(set => 
                {
                    var performedSet = new PerformedSet
                    {
                        SessionId = session.Id,
                        LeftGripType = set.LeftGripType,
                        RightGripType = set.RightGripType,
                        PerformedReps = new List<PerformedRep> { rep }
                    };

                    var existingPerformedSet = session.PerformedSets
                        .Where(ps => ps.SetId == rep.SetId)
                        .FirstOrDefault();

                    if (existingPerformedSet == null) return performedSet;

                    performedSet = existingPerformedSet;

                    performedSet.PerformedReps.Add(rep);

                    return performedSet;

                }).ToList();

                return Ok();

            } catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside the LogRepetition method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
           
        }
    }
}
