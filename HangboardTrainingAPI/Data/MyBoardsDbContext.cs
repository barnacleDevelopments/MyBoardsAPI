using HangboardTrainingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBoardsAPI.Models;
using MyBoardsAPI.Models.Auth;

namespace MyBoardsAPI.Data
{
    public class MyBoardsDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyBoardsDbContext(DbContextOptions<MyBoardsDbContext> options)
        : base (options){
            

        }

        public DbSet<Workout> Workouts { get; set; } = null!;
        public DbSet<Hold> Holds { get; set; } = null!;
        public DbSet<Set> Sets { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Hangboard> Hangboards { get; set; } = null!;
        public DbSet<PerformedRep> PerformedReps { get; set; } = null!;
        public DbSet<PerformedSet> PerformedSets { get; set; } = null!;
        public DbSet<SetHold> SetHolds { get; set; } = null!;

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker
              .Entries()
              .Where(e => e.Entity is BaseModel && (
                      e.State == EntityState.Added
                      || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is Session)
                    ((BaseModel)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hangboard>()
                .HasMany(hangboard => hangboard.Workouts)
                .WithOne(workout => workout.Hangboard)
                .HasForeignKey(workout => workout.HangboardId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SetHold>()
                .HasKey(hs => new { hs.SetId, hs.HoldId });

            modelBuilder.Entity<SetHold>()
                .HasOne(hs => hs.Hold)
                .WithMany(b => b.SetHolds)
                .HasForeignKey(hs => hs.HoldId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<SetHold>()
                .HasOne(hs => hs.Set)
                .WithMany(s => s.SetHolds)
                .HasForeignKey(hs => hs.SetId)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
