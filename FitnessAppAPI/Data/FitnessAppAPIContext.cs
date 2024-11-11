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
    public DbSet<MuscleGroup> MuscleGroups { get; init; }

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

        // Workout -> User relation via Workout.UserId
        modelBuilder.Entity<Workout>()
               .HasOne<User>()
               .WithMany()
               .HasForeignKey(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // MuscleGroup -> User relation via MuscleGroup.UserId
        modelBuilder.Entity<MuscleGroup>()
               .HasOne<User>()
               .WithMany()
               .HasForeignKey(m => m.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Add the default Muscle Groups
        modelBuilder.Entity<MuscleGroup>().HasData(
           new MuscleGroup { Id = 1, Name = "Abs", ImageName = "icon_mg_abs", Default = "Y" },
           new MuscleGroup { Id = 2, Name = "Back", ImageName = "icon_mg_back", Default = "Y" },
           new MuscleGroup { Id = 3, Name = "Biceps", ImageName = "icon_mg_biceps", Default = "Y" },
           new MuscleGroup { Id = 4, Name = "Calves", ImageName = "icon_mg_calves", Default = "Y" },
           new MuscleGroup { Id = 5, Name = "Chest", ImageName = "icon_mg_chest", Default = "Y" },
           new MuscleGroup { Id = 6, Name = "Forearms", ImageName = "icon_mg_forearms", Default = "Y" },
           new MuscleGroup { Id = 7, Name = "Glutes", ImageName = "icon_mg_glutes", Default = "Y" },
           new MuscleGroup { Id = 8, Name = "Hamstrigs", ImageName = "icon_mg_hamstrings", Default = "Y" },
           new MuscleGroup { Id = 9, Name = "Quadtriceps", ImageName = "icon_mg_quadtriceps", Default = "Y" },
           new MuscleGroup { Id = 10, Name = "Shoulders", ImageName = "icon_mg_shoulders", Default = "Y" },
           new MuscleGroup { Id = 11, Name = "Triceps", ImageName = "icon_mg_triceps", Default = "Y" }
       );

    }
}
