using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Data;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Models;
using Microsoft.AspNetCore.Identity;
using MyBoardsAPI.Models.Auth;

namespace MyBoardsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly ILogger<WorkoutController> _logger;
        private readonly MyBoardsDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public WorkoutController(MyBoardsDbContext db, ILogger<WorkoutController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger; 
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            #region           
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var workouts = await _db.Workouts
                    .Include(workout => workout.Hangboard)
                        .ThenInclude(hangboard => hangboard.Holds)
                    .Where(workout => workout.UserId == userId)
                    .OrderBy(workout => workout.Name)
                    .ToListAsync();

                if (workouts == null)
                {
                    return NotFound();
                }

                return Ok(workouts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetAllAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }                            
            #endregion
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);
                
                if(String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var workout = await _db.Workouts
                        .Include(workout => workout.Sets.OrderBy(set => set.IndexPosition))
                            .ThenInclude(set => set.SetHolds)
                                .ThenInclude(set => set.Hold)
                        .Include(workout => workout.Hangboard)
                            .ThenInclude(hangboard => hangboard.Holds)
                        .Where(workout => workout.UserId == userId)
                        .FirstOrDefaultAsync(workout => workout.Id == id);
                
                if (workout == null)
                {
                    return NotFound();
                }

                return Ok(workout);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpGet("{id}/sets")]
        public async Task<IActionResult> GetSetsAsync(int id)
        {
            #region
            try
            {
                var workoutSets = await _db.Sets
                    .Include(set => set.SetHolds)
                        .ThenInclude(setHold => setHold.Hold)
                    .Where(set => set.WorkoutId == id)
                    .OrderBy(set => set.IndexPosition)
                    .ToListAsync();

                return Ok(workoutSets);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetSetsAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Workout workout)
        {
            #region
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }

                // remove navigation props from each sets setHolds to prevent insertion of existing entities.
                foreach (var set in workout.Sets)
                {
                    set.SetHolds = RemoveSetHoldNavigationProperties(set.SetHolds);
                }

                workout.UserId = user.Id;

                await _db.Workouts.AddAsync(workout);

                await _db.SaveChangesAsync();
                
                if (!user.HasCreatedFirstWorkout)
                {
                    user.HasCreatedFirstWorkout = true;
                    await _userManager.UpdateAsync(user);
                }

                workout.Hangboard = await _db.Hangboards
                        .Include(w => w.Holds)
                        .FirstOrDefaultAsync(h => h.Id == workout.HangboardId);

                return Ok(workout);
            }
            catch(Exception ex) {
                _logger.LogError($"Something went wrong inside the PostAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Workout workout)
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var existingWorkout = await _db.Workouts
                    .Where(w => w.Id == workout.Id && workout.UserId == userId)
                    .Include(w => w.Sets)
                        .ThenInclude(s => s.SetHolds)
                    .Include(w => w.Hangboard)
                        .ThenInclude(h => h.Holds)
                    .FirstOrDefaultAsync();

                if(existingWorkout == null)
                {
                    return NotFound();
                }

                _db.Entry(existingWorkout).CurrentValues.SetValues(workout);

                // delete all removed sets
                var removedSets = existingWorkout.Sets
                    .Where(set => !workout.Sets.Select(set => set.Id).Contains(set.Id))
                    .ToList();

                if(removedSets.Any())
                {
                    _db.Sets.RemoveRange(removedSets);
                }

                // update or create any new sets
                foreach(var set in workout.Sets)
                {
                    var existingSet = existingWorkout.Sets
                        .Where(existingSet => existingSet.Id == set.Id && existingSet.Id != 0)
                        .FirstOrDefault();

                    if(existingSet != null)
                    {
                        _db.Entry(existingSet).CurrentValues.SetValues(set);

                        _db.SetHolds.RemoveRange(existingSet.SetHolds);
                        _db.SetHolds.AddRange(set.SetHolds.Select(setHold => new SetHold()
                        {
                            HoldId = setHold.HoldId,
                            SetId = setHold.SetId,
                            Hand = setHold.Hand
                        }));

                    } else
                    {
                        set.SetHolds = set.SetHolds.Select(setHold => new SetHold()
                        {
                            HoldId = setHold.HoldId,
                            SetId = setHold.SetId,
                            Hand = setHold.Hand
                        }).ToList();

                        existingWorkout.Sets.Add(set);
                    }
                }

                await _db.SaveChangesAsync();

                return Ok(existingWorkout);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the UpdateAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if(String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var workout = await _db.Workouts
                    .Include(workout => workout.Sets)
                        .ThenInclude(set => set.SetHolds)
                    .Include(workout => workout.Sessions)
                    .FirstOrDefaultAsync(workout => workout.Id == id);

                if(workout == null)
                {
                    return NotFound();
                }

                // remove relationship with its sessions 
                workout.Sessions.Clear();

                await _db.SaveChangesAsync();

                _db.Workouts.Remove(workout);

                await _db.SaveChangesAsync();

                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteAsync method inside the WorkoutController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        private List<SetHold> RemoveSetHoldNavigationProperties(List<SetHold> setHolds)
        {
            #region
            return setHolds.Select(setHold => new SetHold()
            {
                HoldId = setHold.HoldId,
                SetId = setHold.SetId,
                Hand = setHold.Hand
            }).ToList();
            #endregion
        }
    }
}
