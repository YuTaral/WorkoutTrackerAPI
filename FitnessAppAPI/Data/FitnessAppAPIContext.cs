using FitnessAppAPI.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data;

/// <summary>
/// Class for the Entity Framework DB Context used for identity.
/// </summary>
public class FitnessAppAPIContext(DbContextOptions<FitnessAppAPIContext> options) : IdentityDbContext<User>(options)
{
    public required DbSet<Workout> Workouts { get; init; }
    public required DbSet<Exercise> Exercises { get; init; }
    public required DbSet<Set> Sets { get; init; }
    public required DbSet<MuscleGroup> MuscleGroups { get; init; }
    public required DbSet<MGExercise> MGExercises { get; init; }
    public required DbSet<SystemLog> SystemLogs { get; init; }
    public required DbSet<UserDefaultValue> UserDefaultValues { get; init; }
    public required DbSet<WeightUnit> WeightUnits { get; init; }
    public required DbSet<UserProfile> UserProfiles { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exercise -> Workout relation via Exercise.WorkoutId
        modelBuilder.Entity<Exercise>()
               .HasOne<Workout>()
               .WithMany()
               .HasForeignKey(e => e.WorkoutId)
               .OnDelete(DeleteBehavior.Cascade);

        // Exercise -> MuscleGroup relation via Exercise.MuscleGroupId
        // Do not delete Exercise which is part of workout when MuscleGroup is deleted,
        // Manually set the foreign key to null, so the Exercise is not removed from the Workout
        // and SQL does not throw "multiple cascade paths for ON DELETE action"
        modelBuilder.Entity<Exercise>()
               .HasOne<MuscleGroup>()
               .WithMany()
               .HasForeignKey(e => e.MuscleGroupId)
               .OnDelete(DeleteBehavior.NoAction);

        // Exercise -> MGExercise relation via Exercise.MGExerciseId
        // Do not delete Exercise which is part of workout when MGExercise is deleted,
        // set the foreign key to null, so the Exercise is not removed from the Workout
        // and SQL does not throw "multiple cascade paths for ON DELETE action"
        modelBuilder.Entity<Exercise>()
               .HasOne<MGExercise>()
               .WithMany()
               .HasForeignKey(e => e.MGExerciseId)
               .OnDelete(DeleteBehavior.NoAction);

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

        // MuscleGroupExercise -> MuscleGroup relation via MuscleGroupExercise.MuscleGroupId
        modelBuilder.Entity<MGExercise>()
               .HasOne<MuscleGroup>()
               .WithMany()
               .HasForeignKey(m => m.MuscleGroupId)
               .OnDelete(DeleteBehavior.Cascade);

        // MuscleGroupExercise -> User relation via MuscleGroupId.UserId
        // Manually delete MuscleGroupExercise when User is deleted to avoid
        // "multiple cascade paths" on delete
        modelBuilder.Entity<MGExercise>()
               .HasOne<User>()
               .WithMany()
               .HasForeignKey(m => m.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        // ExerciseDefaultValue -> User relation via ExerciseDefaultValue.UserId
        // Manually delete ExerciseDefaultValue when User is deleted to avoid
        // "multiple cascade paths" on delete
        modelBuilder.Entity<UserDefaultValue>()
               .HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        // ExerciseDefaultValue -> MGExercise relation via ExerciseDefaultValue.MGExeciseId
        // Do not create the foreign key as the user default value has MGExerciseId = 0
        // Handle the delete mannually
        //modelBuilder.Entity<UserDefaultValue>()
        //       .HasOne<MGExercise>()
        //       .WithMany()
        //       .HasForeignKey(e => e.MGExeciseId)
        //       .OnDelete(DeleteBehavior.Cascade);

        // UserDefaultValue -> MGExeWeightUnitcise relation via WeightUnit.WeightUnitId
        modelBuilder.Entity<UserDefaultValue>()
               .HasOne<WeightUnit>()
               .WithMany()
               .HasForeignKey(u => u.WeightUnitId)
               .OnDelete(DeleteBehavior.Cascade);

        // UserProfile -> User relation via UserProfile.UserId
        modelBuilder.Entity<UserProfile>()
               .HasOne<User>()
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Create a unique index on UserId
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasIndex(u => u.UserId).IsUnique();
        });

        // Add the default Weight Units
        modelBuilder.Entity<WeightUnit>().HasData(
            new WeightUnit { Id = 1, Text = Common.Constants.DBConstants.KG },
            new WeightUnit { Id = 2, Text = Common.Constants.DBConstants.LB }
        );

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
           new MuscleGroup { Id = 9, Name = "Quadriceps", ImageName = "icon_mg_quadriceps", Default = "Y" },
           new MuscleGroup { Id = 10, Name = "Shoulders", ImageName = "icon_mg_shoulders", Default = "Y" },
           new MuscleGroup { Id = 11, Name = "Triceps", ImageName = "icon_mg_triceps", Default = "Y" }
       );

        // Add the default Muscle Group Exercises
        modelBuilder.Entity<MGExercise>().HasData(
           // Abs exercises
           new MGExercise { Id = 1, Name = "Chrunches", Description = "Lie on your back with knees bent and feet flat on the floor, hands behind your head or crossed on your chest.\r\nEngage your abs and lift your shoulders a few inches off the ground, exhaling as you crunch up. Keep your neck relaxed.\r\nHold briefly, then slowly lower back down. Repeat for your desired number of reps.", MuscleGroupId = 1 },
           new MGExercise { Id = 2, Name = "Leg raises", Description = "Lie flat on your back with your legs straight and hands placed under your hips or by your sides for support.\r\nEngage your core and lift your legs up together until they’re at a 90-degree angle or as high as you can without lifting your lower back off the floor.\r\nLower your legs back down slowly, stopping just before they touch the ground. Repeat for your desired number of reps, keeping control throughout the movement.", MuscleGroupId = 1 },

           // Back exercises
           new MGExercise { Id = 3, Name = "Wide pull up", Description = "Grip the Bar\r\nStand under the pull-up bar and grab it with both hands wider than shoulder-width apart. Use an overhand grip (palms facing away from you). Engage your core and let your body hang with arms fully extended.\r\nPull Yourself Up\r\nPull your body upward by squeezing your back and shoulder muscles, focusing on bringing your chest up toward the bar. Avoid swinging or using momentum; keep the movement controlled.\r\nLower Back Down\r\nSlowly lower yourself back down until your arms are fully extended. Repeat for your desired number of reps, usually aiming for controlled, smooth movements to maximize back engagement.", MuscleGroupId = 2 },
           new MGExercise { Id = 4, Name = "Dumbbell row", Description = "Get Into Position\r\nStand with feet hip-width apart and hold a dumbbell in one hand. Slightly bend your knees and hinge forward at the hips, keeping your back flat. Support yourself by placing your free hand on a bench or sturdy surface, and let the arm holding the dumbbell hang straight down.\r\nRow the Dumbbell Up\r\nPull the dumbbell upward by squeezing your back and shoulder blades together, bringing your elbow up and back until it’s at about a 90-degree angle. Keep your elbow close to your body, and avoid rotating your torso.\r\nLower Back Down\r\nSlowly lower the dumbbell back down to the starting position. Repeat for the desired number of reps, then switch to the other arm.", MuscleGroupId = 2 },

           // Biceps exercises
           new MGExercise { Id = 5, Name = "Chin up", Description = "Grip the Bar\r\nStand under the pull-up bar and grab it with a shoulder-width or narrower supinated (underhand) grip, palms facing you. Engage your core and let your body hang with arms fully extended.\r\nPull Yourself Up\r\nPull your body upward by bending your elbows and focusing on your biceps. Keep your elbows close to your torso and your motion controlled. Aim to bring your chin above the bar.\r\nLower Back Down\r\nSlowly lower yourself back down until your arms are fully extended, maintaining tension in your biceps. Repeat for your desired number of reps, emphasizing a slow, controlled eccentric phase to maximize bicep activation.", MuscleGroupId = 3 },
           new MGExercise { Id = 6, Name = "Barbell curl", Description = "Grip the Bar\r\nStand tall with your feet shoulder-width apart. Hold a barbell with an underhand (supinated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs, keeping your elbows close to your torso.\r\nCurl the Bar Up\r\nEngage your biceps and curl the bar upward in a smooth, controlled motion. Focus on keeping your elbows stationary and avoiding momentum from your back or shoulders. Bring the bar close to your chest or until your biceps are fully contracted.\r\nLower the Bar\r\nSlowly lower the bar back down to the starting position, fully extending your arms but keeping tension on your biceps. Avoid letting the bar drop quickly to maintain control.", MuscleGroupId = 3 },

           // Calves exercises
           new MGExercise { Id = 7, Name = "Smith machine calf raises", Description = "Set Up the Machine\r\nStand on a platform with the balls of your feet on the edge and your heels hanging off. Position the Smith machine bar across your upper traps and shoulders, then unlock it.\r\nRaise Your Heels\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.", MuscleGroupId = 4 },
           new MGExercise { Id = 8, Name = "Seated calf raises", Description = "Set Up the Machine\r\nSit on the seated calf raise machine with the balls of your feet on the foot platform and your heels hanging off. Position the padded bar securely across your thighs.\r\nRaise Your Heels\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.", MuscleGroupId = 4 },

           // Chest exercises
           new MGExercise { Id = 9, Name = "Flat bench press", Description = "Set Up the Bench\r\nLie flat on a bench with your feet firmly planted on the floor. Grip the barbell with hands slightly wider than shoulder-width, keeping your wrists straight.\r\nLower the Bar\r\nUnrack the bar and slowly lower it to your chest, keeping your elbows at a 45-degree angle to your body. Stop when the bar lightly touches your chest.\r\nPush the Bar Up\r\nPress the bar upward in a controlled motion until your arms are fully extended, keeping your chest engaged. Repeat for your desired number of reps.", MuscleGroupId = 5 },
           new MGExercise { Id = 10, Name = "Dips", Description = "Set Up for Dips\r\nGrasp the parallel bars with your hands shoulder-width apart. Lift yourself up so your arms are fully extended and your body is upright.\r\nLower Your Body\r\nSlowly lower yourself by bending your elbows, keeping them close to your sides. Lower until your upper arms are parallel to the floor or slightly deeper.\r\nPush Back Up\r\nPress through your palms to lift your body back up to the starting position, fully extending your arms. Repeat for your desired number of reps.", MuscleGroupId = 5 },

           // Forearms exercises
           new MGExercise { Id = 11, Name = "Reverse barbell curl", Description = "Grip the Bar\r\nStand tall and hold a barbell with an overhand (pronated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs.\r\nCurl the Bar Up\r\nEngage your forearms and biceps to lift the bar upward, keeping your elbows close to your body. Stop when your forearms are fully contracted.\r\nLower the Bar\r\nSlowly lower the bar back to the starting position, fully extending your arms while maintaining control. Repeat for your desired number of reps.", MuscleGroupId = 6 },
           new MGExercise { Id = 12, Name = "Wrist curl", Description = "Set Up the Exercise\r\nSit on a bench and hold a barbell with an underhand (supinated) grip. Rest your forearms on your thighs with your wrists hanging just past your knees.\r\nCurl the Bar Up\r\nUse your wrists to curl the barbell upward, squeezing your forearms at the top of the movement. Keep your forearms stationary on your thighs.\r\nLower the Bar\r\nSlowly lower the barbell back down, allowing your wrists to extend fully for a good stretch. Repeat for your desired number of reps.", MuscleGroupId = 6 },

           // Glutes exercises
           new MGExercise { Id = 13, Name = "Barbel hip thrust", Description = "Set Up the Exercise\r\nSit on the floor with your upper back against a bench, and roll a barbell over your hips. Your feet should be flat on the floor, about hip-width apart, and positioned so that your knees are bent at 90 degrees. Optionally, use a pad or towel on the barbell for comfort.\r\nLift Your Hips\r\nDrive through your heels and thrust your hips upward, fully extending your hips at the top. Squeeze your glutes hard at the peak of the movement. Keep your chest open and avoid arching your lower back excessively.\r\nLower Back Down\r\nSlowly lower your hips back down until your glutes are just above the floor, maintaining control throughout. Repeat for your desired number of reps, focusing on a slow, controlled movement and squeezing your glutes.", MuscleGroupId = 7 },
           new MGExercise { Id = 14, Name = "Dumbell sumo squat", Description = "Set Up the Exercise\r\nStand with your feet wider than shoulder-width apart and your toes pointing outward at a 45-degree angle. Hold a dumbbell with both hands, letting it hang between your legs. Keep your chest up and your back straight.\r\nLower Your Hips\r\nPush your hips back and bend your knees, lowering your body towards the floor. Keep the dumbbell close to your body as you descend. Go as low as your flexibility allows, ideally until your thighs are parallel to the floor.\r\nPush Back Up\r\nPress through your heels and engage your glutes to lift your body back up to the starting position, fully extending your legs. Squeeze your glutes at the top of the movement and repeat for your desired number of reps.", MuscleGroupId = 7 },

           // Hamstrigs exercises
           new MGExercise { Id = 15, Name = "Deadlift", Description = "Set Up the Exercise\r\nStand with your feet hip-width apart, with the barbell over the middle of your feet. Grip the bar with your hands just outside your knees, using either an overhand or mixed grip. Keep your back straight, chest up, and core engaged.\r\nLift the Bar\r\nHinge at your hips (not your waist), lowering your torso while keeping the bar close to your body. Push your hips back, not down, and feel the stretch in your hamstrings as you lower the bar. Your knees should bend slightly.\r\nReturn to Standing\r\nDrive through your heels and push your hips forward to return to a standing position, fully extending your hips and knees. Squeeze your glutes and hamstrings at the top, then lower the bar back down with control. Repeat for your desired number of reps.", MuscleGroupId = 8 },
           new MGExercise { Id = 16, Name = "Reverse leg extension", Description = "Set Up the Machine\r\nPosition yourself on the reverse leg extension machine (also known as a leg curl machine), lying face down with your knees just off the edge of the pad. Adjust the machine so that the ankle pads are placed just above your heels. Grip the handles for stability and ensure your body is aligned properly.\r\nCurl Your Legs Up\r\nContract your hamstrings and curl your legs upward, bringing your heels toward your glutes. Focus on squeezing your hamstrings at the top of the movement.\r\nLower Back Down\r\nSlowly extend your legs back to the starting position, maintaining control over the movement. Avoid letting the weight drop quickly. Repeat for your desired number of reps, keeping the movement controlled and focusing on the hamstrings throughout.", MuscleGroupId = 8 },

           // Quadtriceps exercises
           new MGExercise { Id = 17, Name = "Squat", Description = "Set Up the Exercise\r\nStand with your feet shoulder-width apart and the barbell resting on your upper traps (for barbell squats). Keep your chest up, shoulders back, and your core engaged. Your toes can point slightly outward, but your knees should track over your toes throughout the movement.\r\nLower Your Body\r\nPush your hips back and bend your knees, lowering your body into a squat. Keep your torso upright, and focus on driving your knees forward, towards your toes, while maintaining balance. Lower yourself until your thighs are parallel to the floor or deeper if your flexibility allows.\r\nPush Back Up\r\nPress through your heels and the balls of your feet to push your body back up to the starting position, straightening your legs and engaging your quads at the top. Repeat for your desired number of reps, maintaining control and proper form throughout.\r\nTip:\r\nTo emphasize the quads more, focus on keeping your torso more upright and allow your knees to travel forward over your toes during the squat.", MuscleGroupId = 9 },
           new MGExercise { Id = 18, Name = "Bulgarian split squat", Description = "Set Up the Exercise\r\nStand a few feet away from a bench or elevated surface. Place one foot on the bench behind you, ensuring your back knee is bent and not touching the floor. Keep your front foot flat on the ground, and your chest upright.\r\nLower Your Body\r\nBend your front knee to lower your body toward the floor, keeping your torso as upright as possible. Your front knee should track over your toes, and your back knee should lower toward the floor. Lower until your front thigh is parallel to the floor or slightly deeper, feeling a stretch in your quads.\r\nPush Back Up\r\nPress through your front heel to return to the starting position, straightening your front leg and engaging your quads. Repeat for the desired number of reps, then switch legs.\r\nTip\r\nTo emphasize the quads more, focus on keeping your torso upright and avoid leaning forward, which can shift the focus to the glutes and hamstrings.", MuscleGroupId = 9 },

           // Shoulders exercises
           new MGExercise { Id = 19, Name = "Dumbbell shoulder press", Description = "Set Up the Exercise\r\nSit on a bench with back support or stand with your feet shoulder-width apart. Hold a dumbbell in each hand at shoulder height with your palms facing forward and elbows bent at a 90-degree angle. Keep your core engaged and your back straight.\r\nPress the Dumbbells Up\r\nPress the dumbbells upward until your arms are fully extended above your head, without locking your elbows. Exhale as you press the weights up, maintaining control throughout the movement.\r\nLower the Dumbbells\r\nSlowly lower the dumbbells back down to shoulder height, inhaling as you return to the starting position. Keep your elbows at a slight angle and avoid letting your shoulders shrug up toward your ears. Repeat for your desired number of reps.\r\nTip\r\nTo avoid straining your lower back, engage your core and avoid leaning back during the press.", MuscleGroupId = 10 },
           new MGExercise { Id = 20, Name = "Dumbbell fron raises", Description = "Set Up the Exercise\r\nStand with your feet shoulder-width apart, holding a dumbbell in each hand with your arms fully extended in front of you, palms facing down. Keep your core engaged and your chest up.\r\nRaise the Dumbbells\r\nLift both dumbbells in front of you, keeping your arms straight (with a slight bend in the elbows) and raising them to shoulder height. Exhale as you raise the dumbbells, focusing on using your shoulders to lift.\r\nLower the Dumbbells\r\nSlowly lower the dumbbells back to the starting position with control, inhaling as you lower them. Avoid swinging the weights or using momentum. Repeat for your desired number of reps.\r\nTip\r\nTo avoid straining your shoulders, keep the movement slow and controlled, and don't raise the dumbbells higher than shoulder height to prevent excessive shoulder stress.", MuscleGroupId = 10 },

           // Triceps exercises
           new MGExercise { Id = 21, Name = "Triceps pushdown", Description = "Set Up the Exercise\r\nStand facing the cable machine with a rope, bar, or V-bar attachment at the high pulley. Grip the attachment with both hands, palms facing down (pronated grip). Step back slightly so there's tension in the cable, and stand with your feet shoulder-width apart, keeping your elbows at your sides and your upper arms stationary.\r\nPush the Attachment Down\r\nPush the rope or bar down by extending your elbows, fully contracting your triceps at the bottom of the movement. Keep your forearms parallel to the floor, and your elbows should stay close to your torso throughout the movement.\r\nReturn Slowly\r\nSlowly allow the attachment to return to the starting position by bending your elbows, controlling the weight on the way up. Repeat for your desired number of reps.\r\nTip\r\nAvoid using momentum or leaning forward to help push the weight down. Keep your core engaged and focus on squeezing your triceps at the bottom of the movement.", MuscleGroupId = 11 },
           new MGExercise { Id = 22, Name = "EZ bar skull crusher", Description = "Set Up the Exercise\r\nLie on a flat bench with an EZ bar in your hands, gripping it with an overhand (pronated) grip. Your hands should be about shoulder-width apart, and the bar should be directly over your chest with your arms fully extended. Keep your feet flat on the floor, and your core engaged to stabilize your body.\r\nLower the Bar\r\nSlowly lower the EZ bar toward your forehead by bending your elbows, keeping your upper arms stationary. Keep your elbows pointed forward, and lower the bar until your forearms are parallel to the ground or slightly beyond, feeling a stretch in your triceps.\r\nPush the Bar Back Up\r\nPress the EZ bar back up by extending your elbows, fully contracting your triceps at the top of the movement. Be sure to keep control of the weight and avoid letting your elbows flare out. Repeat for your desired number of reps.\r\nTip\r\nTo avoid stress on your elbows or shoulders, keep your movements slow and controlled. Focus on using your triceps to lift the weight, and avoid arching your back.", MuscleGroupId = 11 }
        );
    }
}
