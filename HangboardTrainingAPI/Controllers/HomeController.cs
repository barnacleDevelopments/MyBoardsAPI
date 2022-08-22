using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Data;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Models.ViewModels;

namespace MyBoardsAPI.Controllers;

public class HomeController : Controller
{
    private readonly MyBoardsDbContext _db;

    public HomeController(MyBoardsDbContext db)
    {
        _db = db;
    }
    
    
    // GET
    public async Task<IActionResult> Index()
    {
        // boards created 
        var hangboards = await _db.Hangboards.ToListAsync();
        var boardCount = hangboards.Count();
        
        // workouts created 
        var workouts = await _db.Workouts.ToListAsync();
        var workoutCount = workouts.Count();
        
        // pins placed 
        var holds = await _db.Holds.ToListAsync();
        var holdCount = holds.Count();

        var ViewModel = new IndexView()
        {
            HangboardCount = boardCount,
            WorkoutCount = workoutCount,
            HoldCOunt = holdCount
        };

        return View("Index", ViewModel);
    }
    
    public IActionResult PrivacyPolicy()
    {
        return View("Privacy");
    }

    public IActionResult EULA()
    {
        return View("EndUserLisenseAgreement");
    }
}