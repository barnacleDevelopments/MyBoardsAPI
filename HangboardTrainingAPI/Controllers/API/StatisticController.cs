using HangboardTrainingAPI.Enums;
using HangboardTrainingAPI.Models.StatisticModels;
using MyBoardsAPI.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Data;
using Microsoft.AspNetCore.Identity;
using MyBoardsAPI.Models;
using MyBoardsAPI.Models.Auth;


namespace MyBoardsAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly ILogger<WorkoutController> _logger;
        private readonly MyBoardsDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public StatisticController(MyBoardsDbContext db, ILogger<WorkoutController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("PerformedTime")]
        public async Task<IActionResult> GetTotalPerformedTime()
        {
            #region 
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var firstDayOfWeek = GetFirstDayOfWeek();
                double totalSeconds = 0;
                var sessions = await GetUserSessions(userId);
             
                totalSeconds = sessions
                    .Where(session => session.UserId == userId && session.DateCompleted >= firstDayOfWeek)
                    .SelectMany(session => session.PerformedSets)
                    .SelectMany(performedSet => performedSet.PerformedReps)
                    .Select(performedRep => performedRep.SecondsCompleted)
                    .Sum();

                return Ok(totalSeconds);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetTotalLoggedTimeAsync method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpGet("GripUsage/week")]
        public async Task<IActionResult> GetWeekGripUsage()
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userGripUsage = await GetGripUsageOf(userId, YearFraction.week);

                return Ok(userGripUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetWeekGripUsage method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");

            }
            #endregion
        }

        [HttpGet("GripUsage/month")]
        public async Task<IActionResult> GetMonthGripUsage()
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userGripUsage = await GetGripUsageOf(userId, YearFraction.month);

                return Ok(userGripUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetWeekGripUsage method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");

            }
            #endregion
        }

        [HttpGet("GripUsage/quarter")]
        public async Task<IActionResult> GetQuarterGripUsage()
        {
            #region
            try
            {
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userGripUsage = await GetGripUsageOf(userId, YearFraction.quarter);

                return Ok(userGripUsage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetWeekGripUsage method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        [HttpGet("PerformedTime/week")]
        public async Task<IActionResult> GetTrainingTime()
        {
            #region   
            try
            {
                var firstDayOfWeek = GetFirstDayOfWeek();
                var weekTrainTime = new List<DayTrainTime>();
                string userId = _userManager.GetUserId(User);

                if (String.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // get all the sessions and their performed repetitions
                var sessions = await GetUserSessions(userId);

                // get this week's sessions 
                var thisWeeksSessions = sessions
                    .Where(session => session.DateCompleted >= firstDayOfWeek);

                // agregate each day of training time for the week
                weekTrainTime = thisWeeksSessions.Select(session => new DayTrainTime
                {
                    Day = session.DateCompleted.DayOfWeek,
                    Seconds = session.PerformedSets
                    .SelectMany(performedSet => performedSet.PerformedReps)
                       .Select(performedRep => performedRep.SecondsCompleted)
                       .Sum(),
                    })
                    .GroupBy(dayTrainTime => dayTrainTime.Day)
                    .Select(group => group.Aggregate(new DayTrainTime(), (a, b) =>
                    {
                        a.Day = b.Day + 1;
                        a.Seconds = a.Seconds + b.Seconds;

                        return a;
                    })).ToList();

                return Ok(weekTrainTime);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the GetAllPerformedRepsAsync method inside the SessionController: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
            #endregion
        }

        private async Task<double> GetTimeSpentOnGrip(string userId, GripType gripType, YearFraction yearFraction)
        {
            #region
            double timeSpent = 0;
            
            try
            { 
                DateTime startDay = GetFirstDayOfWeek();

                switch (yearFraction)
                {
                    case YearFraction.week:
                        startDay = GetFirstDayOfWeek();
                        break;
                    case YearFraction.month:
                        startDay = GetFirstDayOfMonth();
                        break;
                    case YearFraction.quarter:
                        startDay = GetFirstDayOfQuarter();
                        break;
                }
                // get performed sets of set
                var sessions = await GetUserSessions(userId);

                timeSpent = sessions
                    .Where(session => session.DateCompleted >= startDay)
                     .SelectMany(session => session.PerformedSets)
                     .Where(performedSet =>
                         (performedSet.LeftGripType == gripType ||
                         performedSet.RightGripType == gripType)
                     )
                     .SelectMany(performedSet => performedSet.PerformedReps)
                     .Sum(performedRep => performedRep.SecondsCompleted);    
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside the getTimeSpentOnGrip method inside the SessionController: {ex}");
            }

            return timeSpent;
            #endregion
        }
        private DateTime GetFirstDayOfWeek()
        {
            #region
            var todaysDate = DateTime.Today;
        
            var dayofWeek = (int)todaysDate.DayOfWeek;

            if (dayofWeek >= 0)
            {
                var diff = 7 - dayofWeek;
                var daysToRemove = diff - 7;
                var firstDayOfWeek = todaysDate.AddDays(daysToRemove);

                return firstDayOfWeek;
            }

            return DateTime.Now;
            #endregion
        }

        private DateTime GetFirstDayOfMonth()
        {
            #region
            return DateTime.Now.AddDays(-DateTime.Now.Day);
            #endregion
        }

        private DateTime GetFirstDayOfQuarter()
        {
            #region
            // define quarter ranges 

            // Quarter 1
            var QuarterOneStart = DateTime.Today
                .AddMonths(-DateTime.Today.Month + 1)
                .AddDays(-DateTime.Today.Day + 1); // January 1st

            var QuarterOneLastMonth = QuarterOneStart
                .AddMonths(2).Month;

            var QuarterOneEnd = QuarterOneStart
                .AddMonths(2)
                .AddDays(DateTime.DaysInMonth(QuarterOneStart.Year, QuarterOneLastMonth) - 1); // March 31st


            // Quarter 2
            var QuarterTwoStart = QuarterOneEnd.AddDays(1); // April 1st

            var QuarterTwoLastMonth = QuarterTwoStart
                .AddMonths(2).Month;

            var QuarterTwoEnd = QuarterTwoStart
                .AddMonths(2)
                .AddDays(DateTime.DaysInMonth(QuarterTwoStart.Year, QuarterTwoLastMonth) - 1); // June 30th

           // Quarter 3
            var QuarterThreeStart = QuarterTwoEnd.AddDays(1); // July 1st 

            var QuarterThreeLastMonth = QuarterThreeStart
                .AddMonths(2).Month;

            var QuarterThreeEnd = QuarterThreeStart
                .AddMonths(2)
                .AddDays(DateTime.DaysInMonth(QuarterThreeStart.Year, QuarterThreeLastMonth) - 1); // September 30th

            // Quarter 4
            var QuarterFourStart = QuarterThreeEnd.AddDays(1); // January First

            var QuarterFourLastMonth = QuarterFourStart
                .AddMonths(2).Month;

            var QuarterFourEnd = QuarterFourStart
                .AddMonths(2)
                .AddDays(DateTime.DaysInMonth(QuarterFourStart.Year, QuarterFourLastMonth) - 1); // March 31st

            
            // get today's date
            var today = DateTime.Today;

            // find which quarter the date fits inside and return first day 

            if(today >= QuarterOneStart && today <= QuarterOneEnd)
            {
                return QuarterOneStart;
            }

            if(today >= QuarterTwoStart && today <= QuarterTwoEnd)
            {
                return QuarterTwoStart;
            }

            if(today >= QuarterThreeStart && today <= QuarterThreeEnd)
            {
                return QuarterThreeStart;
            }

            if(today >= QuarterFourStart && today <= QuarterFourEnd)
            {
                return QuarterFourStart;
            }

            return today;
            #endregion
        }

        private async Task<List<Session>> GetUserSessions(string userId)
        {
            #region
            return await _db.Sessions
                .Include(session => session.PerformedSets)
                    .ThenInclude(performedSet => performedSet.PerformedReps)
                .Where(session => session.UserId == userId)
                .ToListAsync();
            #endregion
        }

        private async Task<GripUsage> GetGripUsageOf(string userId, YearFraction yearFraction)
        {
            #region
            var userGripUsage = new GripUsage();

            // find time spent on each grip type
            double timeOnFullCrimp = await GetTimeSpentOnGrip(userId, GripType.FullCrimp, yearFraction);

            double timeOnHalfCrimp = await GetTimeSpentOnGrip(userId, GripType.HalfCrimp, yearFraction);

            double timeOnOpenHand = await GetTimeSpentOnGrip(userId, GripType.OpenHand, yearFraction);

            // find the percentage of use for each grip type overall
            var totalTime = timeOnFullCrimp + timeOnHalfCrimp + timeOnOpenHand;

            userGripUsage.FullCrimp = (timeOnFullCrimp / totalTime) * 100 >= 0 ? (timeOnFullCrimp / totalTime) * 100 : 0.0;

            userGripUsage.HalfCrimp = (timeOnHalfCrimp / totalTime) * 100 >= 0 ? (timeOnHalfCrimp / totalTime) * 100 : 0.0;

            userGripUsage.OpenHand = (timeOnOpenHand / totalTime) * 100 >= 0 ? (timeOnOpenHand / totalTime) * 100 : 0.0;

            return userGripUsage;
            #endregion
        }
    }
}
