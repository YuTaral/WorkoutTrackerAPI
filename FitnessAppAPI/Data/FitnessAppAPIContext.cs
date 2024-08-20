using FitnessAppAPI.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessAppAPI.Data;

/// <summary>
/// Class for the Entity Framework DB Context used for identity.
/// </summary>
public class FitnessAppAPIContext(DbContextOptions<FitnessAppAPIContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Workout> Workouts { get; init; }
    public DbSet<Exercise> Exercises { get; init; }
    public DbSet<Set> Sets { get; init; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exercise -> Workout relation via Set.WorkoutId
        modelBuilder.Entity<Exercise>()
               .HasOne<Workout>()
               .WithMany()
               .HasForeignKey(s => s.WorkoutId)
               .OnDelete(DeleteBehavior.Cascade);

        // Set -> Exercise relation via Set.ExerciseId
        modelBuilder.Entity<Set>()
               .HasOne<Exercise>()  
               .WithMany()
               .HasForeignKey(s => s.ExerciseId)
               .OnDelete(DeleteBehavior.Cascade);
    }

}
