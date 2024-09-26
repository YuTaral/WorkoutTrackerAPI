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
    public DbSet<MuscleGroupToWorkout> MuscleGroupsToWorkout { get; init; }

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

        // MuscleGroupToWorkout -> Workout relation via MuscleGroupToWorkout.WorkoutId
        modelBuilder.Entity<MuscleGroupToWorkout>()
               .HasNoKey()
               .HasOne<Workout>()
               .WithMany()
               .HasForeignKey(m => m.WorkoutId)
               .OnDelete(DeleteBehavior.Cascade);

        // MuscleGroupToWorkout -> MuscleGroup relation via MuscleGroupToWorkout.MuscleGroupId
        modelBuilder.Entity<MuscleGroupToWorkout>()
               .HasNoKey()
               .HasOne<MuscleGroup>()
               .WithMany()
               .HasForeignKey(m => m.MuscleGroupId)
               .OnDelete(DeleteBehavior.Cascade);

        // Define the many-to-many relationship between Workout and MuscleGroup through MuscleGroupToWorkout
        modelBuilder.Entity<MuscleGroupToWorkout>()
            .HasKey(mg => new { mg.WorkoutId, mg.MuscleGroupId }); // Composite primary key

        // MuscleGroupToWorkout -> Workout relation via MuscleGroupToWorkout.WorkoutId
        // The table contains 2 relations
        // Manually delete record when Workout is deleted, cannot set DeleteBehavior.Cascade,
        // because it creates "Multiple Cascade paths" - occurs when a single table is involved
        // in multiple foreign key relationships, and multiple cascade actions (such as DELETE or UPDATE)
        // could be triggered on the same table. The database cannot determine how to handle such cascading
        // actions because it can lead to conflicts or ambiguity.
        modelBuilder.Entity<MuscleGroupToWorkout>()
            .HasOne<Workout>()
            .WithMany()
            .HasForeignKey(mg => mg.WorkoutId)
            .OnDelete(DeleteBehavior.NoAction);

        // MuscleGroupToWorkout -> MuscleGroup relation via MuscleGroupToWorkout.MuscleGroupId
        // The table contains 2 relations
        // Manually delete record when Workout is deleted, cannot set DeleteBehavior.Cascade,
        // because it creates "Multiple Cascade paths" - occurs when a single table is involved
        // in multiple foreign key relationships, and multiple cascade actions (such as DELETE or UPDATE)
        // could be triggered on the same table. The database cannot determine how to handle such cascading
        // actions because it can lead to conflicts or ambiguity.
        modelBuilder.Entity<MuscleGroupToWorkout>()
            .HasOne<MuscleGroup>()
            .WithMany()
            .HasForeignKey(mg => mg.MuscleGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        // Add the default Muscle Groups
        modelBuilder.Entity<MuscleGroup>().HasData(
            new MuscleGroup { Id = 1, Name = "Full Body"},
            new MuscleGroup { Id = 2, Name = "Upper Body"},
            new MuscleGroup { Id = 3, Name = "Lower Body"},
            new MuscleGroup { Id = 4, Name = "Legs" },
            new MuscleGroup { Id = 5, Name = "Back" },
            new MuscleGroup { Id = 6, Name = "Chest" },
            new MuscleGroup { Id = 7, Name = "Shoulders" },
            new MuscleGroup { Id = 8, Name = "Arms" }
        );
    }
}
