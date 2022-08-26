using HangboardTrainingAPI.Models;

namespace MyBoardsAPI.Models.ViewModels;

public class IndexView
{
    public int HangboardCount { get; set; }
    public int WorkoutCount { get; set; }
    public int HoldCount { get; set; }
    public List<Feature> CurrentFeatures { get; set; } = new List<Feature>();
    public List<Feature> UpcomingFeatures { get; set; } = new List<Feature>();
}