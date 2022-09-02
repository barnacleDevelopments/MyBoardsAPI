using Microsoft.AspNetCore.Identity;

namespace MyBoardsAPI.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public bool HasCreatedFirstWorkout { get; set; } = false;
        public bool HasCreatedFirstHangboard { get; set; } = false;
    }
}
