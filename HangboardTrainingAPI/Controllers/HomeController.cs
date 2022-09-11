using HangboardTrainingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MyBoardsAPI.Data;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Models;
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

        // get all current features
        var currentFeatures = new List<Feature>();
        
        currentFeatures.Add(new Feature()
        {
            Name = "Custom Hangboards",
            Description = "Train with your own hangboard setup or choose from a veriaty of pre-configured boards."
        });
        
        currentFeatures.Add(new Feature()
        {
            Name = "Custom Workouts",
            Description = "Develop and share workouts using a granular editor."
        });
        
        currentFeatures.Add(new Feature()
        {
            Name = "Training Overview",
            Description = "Monitor your training on a week, month, and quarterly basis."
        });
        
        // get all upcoming features
        var upcomingFeatures = new List<Feature>();
        
        upcomingFeatures.Add(new Feature()
        {
            Name = "Auto Generated Workouts",
            Description = "Train with your own hangboard setup or choose from a veriaty of pre-configured boards."
        });
        
        upcomingFeatures.Add(new Feature()
        {
            Name = "Sharable Boards",
            Description = "Develop and share workouts using a granular editor."
        });
        
        upcomingFeatures.Add(new Feature()
        {
            Name = "Pain Monitoring",
            Description = "Monitor your training on a week, month, and quarterly basis."
        });

        var ViewModel = new IndexView()
        {
            HangboardCount = boardCount,
            WorkoutCount = workoutCount,
            HoldCount = holdCount,
            CurrentFeatures = currentFeatures,
            UpcomingFeatures = upcomingFeatures
        };

        return View("Index", ViewModel);
    }

    public IActionResult Contact()
    {
        return View("Contact");
    }

    public IActionResult Roadmap()
    {
        return View("Roadmap");
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