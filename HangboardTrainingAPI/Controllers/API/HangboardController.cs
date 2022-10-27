using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Data;
using System.Text.Json;
using MyBoardsAPI.Models.Auth;
using Microsoft.AspNetCore.Identity;
using MyBoardsAPI.Models;
using MyBboardsAPI.Services;

namespace MyBoardsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Area("api")]
    [Route("[area]/[controller]")]
    public class HangboardController : ControllerBase
    {
        private readonly ILogger<WorkoutController> _logger;
        private readonly ImageService _imageService;
        private readonly MyBoardsDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HangboardController(MyBoardsDbContext db, ILogger<WorkoutController> logger, ImageService imageService, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _imageService = imageService;
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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

               var hangboards = await _db.Hangboards
                    .Include(hangboard => hangboard.Holds)
                    .Where(hangboard => hangboard.UserId == userId)
                    .OrderBy(hangboard => hangboard.Name)
                    .ToListAsync();

                return Ok(hangboards);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the GetAllAsync method inside the HangboardController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpGet("{hangboardId}")]
        public async Task<IActionResult> GetAsync(int hangboardId)
        {
            #region
            string userId = _userManager.GetUserId(User);
            
            if (String.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var hangboard = await _db.Hangboards
                    .Include(hangboard => hangboard.Holds)
                    .Where(hangboard => hangboard.UserId == userId)
                    .FirstOrDefaultAsync(hangboard => hangboard.Id == hangboardId);

                return Ok(hangboard);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the GetAsync method inside the HangboardController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }

            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            #region
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }

                // parse hangboard from request body
                var hangboard = GetEntityFromRequest<Hangboard>("hangboard");
                
                if(hangboard == null)
                {
                    return BadRequest();
                }

                // Create hangboard
                hangboard.UserId = user.Id;

                await _db.Hangboards.AddAsync(hangboard);

                await _db.SaveChangesAsync();

                if (!user.HasCreatedFirstHangboard)
                {
                    user.HasCreatedFirstHangboard = true;
                    await _userManager.UpdateAsync(user);
                }

                hangboard.ImageURL = await GetImageURIFromRequest(hangboard.Id);

                _db.Hangboards.Update(hangboard);

                await _db.SaveChangesAsync();

                return Ok(hangboard);

            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the PostAsync method inside the HangboardController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }         
            #endregion
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync()
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var hangboard = GetEntityFromRequest<Hangboard>("hangboard");

                if(hangboard == null)
                {
                    return BadRequest();
                }

                if (Request.Form.Files.Any())
                {
                    hangboard.ImageURL = await GetImageURIFromRequest(hangboard.Id);
                }

                // delete any workouts that have been created using hangboard
                await DeleteWorkoutsOfHangboard(hangboard.Id);

                // delete all the old holds
                await DeleteHangboardHolds(hangboard.Id);

                // create all the new holds
                foreach (var hold in hangboard.Holds)
                {
                    hold.Id = 0;
                }

                hangboard.UserId = userId;

                _db.Hangboards.Update(hangboard);

                await _db.SaveChangesAsync();

                return Ok(hangboard);

            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the PutAsync method inside the HangboardController: {ex}");
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
                if(id == 0)
                {
                    return BadRequest();
                }

                await DeleteWorkoutsOfHangboard(id);

                await DeleteHoldsOfHangboard(id);
                
                await DeleteHanboard(id);

                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the DeleteAsync method inside the HangboardController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }

            return Ok();
            #endregion
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            #region
            int count = 0;

            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                count = await _db.Hangboards
                    .Where(hangboard => hangboard.UserId == userId)
                    .CountAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"[500 Error] Something went wrong inside the GetCount method inside the HangboardController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }

            return Ok(count);
            #endregion
        }

        private dynamic? GetEntityFromRequest<T>(string entityName)
        {
            #region
            // parse hangboard from request body
            var hangboardJson = Request.Form[entityName]
                .ToArray()
                .First()
                .ToString();

            var result = JsonSerializer.Deserialize<T>(hangboardJson);

            return result;
            #endregion
        }

        private async Task DeleteWorkoutsOfHangboard(int hangboardId)
        {
            #region
            try
            {
                var workoutsToDelete = await _db.Workouts
                    .Include(workout => workout.Sets)
                        .ThenInclude(set => set.SetHolds)
                    .Include(workout => workout.Sessions)
                    .Where(workout => workout.HangboardId == hangboardId)
                    .ToListAsync();

                workoutsToDelete.ForEach(workout => workout.Sessions.Clear());

                _db.Workouts.RemoveRange(workoutsToDelete);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteWorkoutsOfHangboard method inside the HangboardController: {ex}");
            }
            #endregion
        }

        private async Task DeleteHangboardHolds(int hanboardId)
        {
            #region
            try
            {
                var holds = await _db.Holds
                    .Where(hold => hold.HangboardId == hanboardId)
                    .ToListAsync();

                _db.RemoveRange(holds);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteHangboardHolds method inside the HangboardController: {ex}");
            }
            #endregion
        }

        private async Task DeleteHoldsOfHangboard(int hangboardId)
        {
            #region
            try
            {
                var holds = await _db.Holds
                    .Where(hold => hold.HangboardId == hangboardId)
                    .ToListAsync();

                _db.Holds.RemoveRange(holds);

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteHoldsOfHangboard method inside the HangboardController: {ex}");
            }
            #endregion
        }

        private async Task DeleteHanboard(int hangboardId)
        {
            #region
            try
            {
                var hangboardToDelete = await _db.Hangboards
                    .FirstOrDefaultAsync(hangboard => hangboard.Id == hangboardId);

                if(hangboardToDelete != null)
                {
                    _db.Hangboards.Remove(hangboardToDelete);
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteHanboard method inside the HangboardController: {ex}");
            }
            #endregion
        }

        private async Task<string> GetImageURIFromRequest(int hangboardId)
        {
            #region
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                return await _imageService.UploadImage(file, hangboardId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetImageFromRequest method inside the HangboardController: {ex}");
                throw;
            }
            #endregion
        }

    }
}
