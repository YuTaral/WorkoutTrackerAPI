﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedExercisesDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Description",
                value: "Lie on your back with knees bent and feet flat on the floor, hands behind your head or crossed on your chest.\r\nEngage your abs and lift your shoulders a few inches off the ground, exhaling as you crunch up. Keep your neck relaxed.\r\nHold briefly, then slowly lower back down. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Description",
                value: "Lie flat on your back with your legs straight and hands placed under your hips or by your sides for support.\r\nEngage your core and lift your legs up together until they’re at a 90-degree angle or as high as you can without lifting your lower back off the floor.\r\nLower your legs back down slowly, stopping just before they touch the ground. Repeat for your desired number of reps, keeping control throughout the movement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Description",
                value: "Grip the Bar\r\nStand under the pull-up bar and grab it with both hands wider than shoulder-width apart. Use an overhand grip (palms facing away from you). Engage your core and let your body hang with arms fully extended.\r\nPull Yourself Up\r\nPull your body upward by squeezing your back and shoulder muscles, focusing on bringing your chest up toward the bar. Avoid swinging or using momentum; keep the movement controlled.\r\nLower Back Down\r\nSlowly lower yourself back down until your arms are fully extended. Repeat for your desired number of reps, usually aiming for controlled, smooth movements to maximize back engagement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Description",
                value: "Get Into Position\r\nStand with feet hip-width apart and hold a dumbbell in one hand. Slightly bend your knees and hinge forward at the hips, keeping your back flat. Support yourself by placing your free hand on a bench or sturdy surface, and let the arm holding the dumbbell hang straight down.\r\nRow the Dumbbell Up\r\nPull the dumbbell upward by squeezing your back and shoulder blades together, bringing your elbow up and back until it’s at about a 90-degree angle. Keep your elbow close to your body, and avoid rotating your torso.\r\nLower Back Down\r\nSlowly lower the dumbbell back down to the starting position. Repeat for the desired number of reps, then switch to the other arm.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Description",
                value: "Grip the Bar\r\nStand under the pull-up bar and grab it with a shoulder-width or narrower supinated (underhand) grip, palms facing you. Engage your core and let your body hang with arms fully extended.\r\nPull Yourself Up\r\nPull your body upward by bending your elbows and focusing on your biceps. Keep your elbows close to your torso and your motion controlled. Aim to bring your chin above the bar.\r\nLower Back Down\r\nSlowly lower yourself back down until your arms are fully extended, maintaining tension in your biceps. Repeat for your desired number of reps, emphasizing a slow, controlled eccentric phase to maximize bicep activation.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Description",
                value: "Grip the Bar\r\nStand tall with your feet shoulder-width apart. Hold a barbell with an underhand (supinated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs, keeping your elbows close to your torso.\r\nCurl the Bar Up\r\nEngage your biceps and curl the bar upward in a smooth, controlled motion. Focus on keeping your elbows stationary and avoiding momentum from your back or shoulders. Bring the bar close to your chest or until your biceps are fully contracted.\r\nLower the Bar\r\nSlowly lower the bar back down to the starting position, fully extending your arms but keeping tension on your biceps. Avoid letting the bar drop quickly to maintain control.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Description",
                value: "Set Up the Machine\r\nStand on a platform with the balls of your feet on the edge and your heels hanging off. Position the Smith machine bar across your upper traps and shoulders, then unlock it.\r\nRaise Your Heels\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Description",
                value: "Set Up the Machine\r\nSit on the seated calf raise machine with the balls of your feet on the foot platform and your heels hanging off. Position the padded bar securely across your thighs.\r\nRaise Your Heels\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 9L,
                column: "Description",
                value: "Set Up the Bench\r\nLie flat on a bench with your feet firmly planted on the floor. Grip the barbell with hands slightly wider than shoulder-width, keeping your wrists straight.\r\nLower the Bar\r\nUnrack the bar and slowly lower it to your chest, keeping your elbows at a 45-degree angle to your body. Stop when the bar lightly touches your chest.\r\nPush the Bar Up\r\nPress the bar upward in a controlled motion until your arms are fully extended, keeping your chest engaged. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Description",
                value: "Set Up for Dips\r\nGrasp the parallel bars with your hands shoulder-width apart. Lift yourself up so your arms are fully extended and your body is upright.\r\nLower Your Body\r\nSlowly lower yourself by bending your elbows, keeping them close to your sides. Lower until your upper arms are parallel to the floor or slightly deeper.\r\nPush Back Up\r\nPress through your palms to lift your body back up to the starting position, fully extending your arms. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Description",
                value: "Grip the Bar\r\nStand tall and hold a barbell with an overhand (pronated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs.\r\nCurl the Bar Up\r\nEngage your forearms and biceps to lift the bar upward, keeping your elbows close to your body. Stop when your forearms are fully contracted.\r\nLower the Bar\r\nSlowly lower the bar back to the starting position, fully extending your arms while maintaining control. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 12L,
                column: "Description",
                value: "Set Up the Exercise\r\nSit on a bench and hold a barbell with an underhand (supinated) grip. Rest your forearms on your thighs with your wrists hanging just past your knees.\r\nCurl the Bar Up\r\nUse your wrists to curl the barbell upward, squeezing your forearms at the top of the movement. Keep your forearms stationary on your thighs.\r\nLower the Bar\r\nSlowly lower the barbell back down, allowing your wrists to extend fully for a good stretch. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 13L,
                column: "Description",
                value: "Set Up the Exercise\r\nSit on the floor with your upper back against a bench, and roll a barbell over your hips. Your feet should be flat on the floor, about hip-width apart, and positioned so that your knees are bent at 90 degrees. Optionally, use a pad or towel on the barbell for comfort.\r\nLift Your Hips\r\nDrive through your heels and thrust your hips upward, fully extending your hips at the top. Squeeze your glutes hard at the peak of the movement. Keep your chest open and avoid arching your lower back excessively.\r\nLower Back Down\r\nSlowly lower your hips back down until your glutes are just above the floor, maintaining control throughout. Repeat for your desired number of reps, focusing on a slow, controlled movement and squeezing your glutes.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 14L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand with your feet wider than shoulder-width apart and your toes pointing outward at a 45-degree angle. Hold a dumbbell with both hands, letting it hang between your legs. Keep your chest up and your back straight.\r\nLower Your Hips\r\nPush your hips back and bend your knees, lowering your body towards the floor. Keep the dumbbell close to your body as you descend. Go as low as your flexibility allows, ideally until your thighs are parallel to the floor.\r\nPush Back Up\r\nPress through your heels and engage your glutes to lift your body back up to the starting position, fully extending your legs. Squeeze your glutes at the top of the movement and repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 15L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand with your feet hip-width apart, with the barbell over the middle of your feet. Grip the bar with your hands just outside your knees, using either an overhand or mixed grip. Keep your back straight, chest up, and core engaged.\r\nLift the Bar\r\nHinge at your hips (not your waist), lowering your torso while keeping the bar close to your body. Push your hips back, not down, and feel the stretch in your hamstrings as you lower the bar. Your knees should bend slightly.\r\nReturn to Standing\r\nDrive through your heels and push your hips forward to return to a standing position, fully extending your hips and knees. Squeeze your glutes and hamstrings at the top, then lower the bar back down with control. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 16L,
                column: "Description",
                value: "Set Up the Machine\r\nPosition yourself on the reverse leg extension machine (also known as a leg curl machine), lying face down with your knees just off the edge of the pad. Adjust the machine so that the ankle pads are placed just above your heels. Grip the handles for stability and ensure your body is aligned properly.\r\nCurl Your Legs Up\r\nContract your hamstrings and curl your legs upward, bringing your heels toward your glutes. Focus on squeezing your hamstrings at the top of the movement.\r\nLower Back Down\r\nSlowly extend your legs back to the starting position, maintaining control over the movement. Avoid letting the weight drop quickly. Repeat for your desired number of reps, keeping the movement controlled and focusing on the hamstrings throughout.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 17L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand with your feet shoulder-width apart and the barbell resting on your upper traps (for barbell squats). Keep your chest up, shoulders back, and your core engaged. Your toes can point slightly outward, but your knees should track over your toes throughout the movement.\r\nLower Your Body\r\nPush your hips back and bend your knees, lowering your body into a squat. Keep your torso upright, and focus on driving your knees forward, towards your toes, while maintaining balance. Lower yourself until your thighs are parallel to the floor or deeper if your flexibility allows.\r\nPush Back Up\r\nPress through your heels and the balls of your feet to push your body back up to the starting position, straightening your legs and engaging your quads at the top. Repeat for your desired number of reps, maintaining control and proper form throughout.\r\nTip:\r\nTo emphasize the quads more, focus on keeping your torso more upright and allow your knees to travel forward over your toes during the squat.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 18L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand a few feet away from a bench or elevated surface. Place one foot on the bench behind you, ensuring your back knee is bent and not touching the floor. Keep your front foot flat on the ground, and your chest upright.\r\nLower Your Body\r\nBend your front knee to lower your body toward the floor, keeping your torso as upright as possible. Your front knee should track over your toes, and your back knee should lower toward the floor. Lower until your front thigh is parallel to the floor or slightly deeper, feeling a stretch in your quads.\r\nPush Back Up\r\nPress through your front heel to return to the starting position, straightening your front leg and engaging your quads. Repeat for the desired number of reps, then switch legs.\r\nTip\r\nTo emphasize the quads more, focus on keeping your torso upright and avoid leaning forward, which can shift the focus to the glutes and hamstrings.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 19L,
                column: "Description",
                value: "Set Up the Exercise\r\nSit on a bench with back support or stand with your feet shoulder-width apart. Hold a dumbbell in each hand at shoulder height with your palms facing forward and elbows bent at a 90-degree angle. Keep your core engaged and your back straight.\r\nPress the Dumbbells Up\r\nPress the dumbbells upward until your arms are fully extended above your head, without locking your elbows. Exhale as you press the weights up, maintaining control throughout the movement.\r\nLower the Dumbbells\r\nSlowly lower the dumbbells back down to shoulder height, inhaling as you return to the starting position. Keep your elbows at a slight angle and avoid letting your shoulders shrug up toward your ears. Repeat for your desired number of reps.\r\nTip\r\nTo avoid straining your lower back, engage your core and avoid leaning back during the press.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 20L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand with your feet shoulder-width apart, holding a dumbbell in each hand with your arms fully extended in front of you, palms facing down. Keep your core engaged and your chest up.\r\nRaise the Dumbbells\r\nLift both dumbbells in front of you, keeping your arms straight (with a slight bend in the elbows) and raising them to shoulder height. Exhale as you raise the dumbbells, focusing on using your shoulders to lift.\r\nLower the Dumbbells\r\nSlowly lower the dumbbells back to the starting position with control, inhaling as you lower them. Avoid swinging the weights or using momentum. Repeat for your desired number of reps.\r\nTip\r\nTo avoid straining your shoulders, keep the movement slow and controlled, and don't raise the dumbbells higher than shoulder height to prevent excessive shoulder stress.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 21L,
                column: "Description",
                value: "Set Up the Exercise\r\nStand facing the cable machine with a rope, bar, or V-bar attachment at the high pulley. Grip the attachment with both hands, palms facing down (pronated grip). Step back slightly so there's tension in the cable, and stand with your feet shoulder-width apart, keeping your elbows at your sides and your upper arms stationary.\r\nPush the Attachment Down\r\nPush the rope or bar down by extending your elbows, fully contracting your triceps at the bottom of the movement. Keep your forearms parallel to the floor, and your elbows should stay close to your torso throughout the movement.\r\nReturn Slowly\r\nSlowly allow the attachment to return to the starting position by bending your elbows, controlling the weight on the way up. Repeat for your desired number of reps.\r\nTip\r\nAvoid using momentum or leaning forward to help push the weight down. Keep your core engaged and focus on squeezing your triceps at the bottom of the movement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 22L,
                column: "Description",
                value: "Set Up the Exercise\r\nLie on a flat bench with an EZ bar in your hands, gripping it with an overhand (pronated) grip. Your hands should be about shoulder-width apart, and the bar should be directly over your chest with your arms fully extended. Keep your feet flat on the floor, and your core engaged to stabilize your body.\r\nLower the Bar\r\nSlowly lower the EZ bar toward your forehead by bending your elbows, keeping your upper arms stationary. Keep your elbows pointed forward, and lower the bar until your forearms are parallel to the ground or slightly beyond, feeling a stretch in your triceps.\r\nPush the Bar Back Up\r\nPress the EZ bar back up by extending your elbows, fully contracting your triceps at the top of the movement. Be sure to keep control of the weight and avoid letting your elbows flare out. Repeat for your desired number of reps.\r\nTip\r\nTo avoid stress on your elbows or shoulders, keep your movements slow and controlled. Focus on using your triceps to lift the weight, and avoid arching your back.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Description",
                value: "Lie on your back with knees bent and feet flat on the floor, hands behind your head or crossed on your chest.\r\n\r\nEngage your abs and lift your shoulders a few inches off the ground, exhaling as you crunch up. Keep your neck relaxed.\r\n\r\nHold briefly, then slowly lower back down. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Description",
                value: "Lie flat on your back with your legs straight and hands placed under your hips or by your sides for support.\r\n\r\nEngage your core and lift your legs up together until they’re at a 90-degree angle or as high as you can without lifting your lower back off the floor.\r\n\r\nLower your legs back down slowly, stopping just before they touch the ground. Repeat for your desired number of reps, keeping control throughout the movement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Description",
                value: "Grip the Bar\r\nStand under the pull-up bar and grab it with both hands wider than shoulder-width apart. Use an overhand grip (palms facing away from you). Engage your core and let your body hang with arms fully extended.\r\n\r\nPull Yourself Up\r\nPull your body upward by squeezing your back and shoulder muscles, focusing on bringing your chest up toward the bar. Avoid swinging or using momentum; keep the movement controlled.\r\n\r\nLower Back Down\r\nSlowly lower yourself back down until your arms are fully extended. Repeat for your desired number of reps, usually aiming for controlled, smooth movements to maximize back engagement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Description",
                value: "Get Into Position\r\nStand with feet hip-width apart and hold a dumbbell in one hand. Slightly bend your knees and hinge forward at the hips, keeping your back flat. Support yourself by placing your free hand on a bench or sturdy surface, and let the arm holding the dumbbell hang straight down.\r\n\r\nRow the Dumbbell Up\r\nPull the dumbbell upward by squeezing your back and shoulder blades together, bringing your elbow up and back until it’s at about a 90-degree angle. Keep your elbow close to your body, and avoid rotating your torso.\r\n\r\nLower Back Down\r\nSlowly lower the dumbbell back down to the starting position. Repeat for the desired number of reps, then switch to the other arm.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Description",
                value: "Here’s a summarized version focused on targeting the biceps during chin-ups:\r\nGrip the Bar\r\n\r\nStand under the pull-up bar and grab it with a shoulder-width or narrower supinated (underhand) grip, palms facing you. Engage your core and let your body hang with arms fully extended.\r\nPull Yourself Up\r\n\r\nPull your body upward by bending your elbows and focusing on your biceps. Keep your elbows close to your torso and your motion controlled. Aim to bring your chin above the bar.\r\nLower Back Down\r\n\r\nSlowly lower yourself back down until your arms are fully extended, maintaining tension in your biceps. Repeat for your desired number of reps, emphasizing a slow, controlled eccentric phase to maximize bicep activation.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Description",
                value: "Grip the Bar\r\n\r\nStand tall with your feet shoulder-width apart. Hold a barbell with an underhand (supinated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs, keeping your elbows close to your torso.\r\nCurl the Bar Up\r\n\r\nEngage your biceps and curl the bar upward in a smooth, controlled motion. Focus on keeping your elbows stationary and avoiding momentum from your back or shoulders. Bring the bar close to your chest or until your biceps are fully contracted.\r\nLower the Bar\r\n\r\nSlowly lower the bar back down to the starting position, fully extending your arms but keeping tension on your biceps. Avoid letting the bar drop quickly to maintain control.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Description",
                value: "Set Up the Machine\r\n\r\nStand on a platform with the balls of your feet on the edge and your heels hanging off. Position the Smith machine bar across your upper traps and shoulders, then unlock it.\r\nRaise Your Heels\r\n\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\n\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Description",
                value: "Set Up the Machine\r\n\r\nSit on the seated calf raise machine with the balls of your feet on the foot platform and your heels hanging off. Position the padded bar securely across your thighs.\r\nRaise Your Heels\r\n\r\nPush through the balls of your feet to lift your heels as high as possible, squeezing your calves at the top.\r\nLower Back Down\r\n\r\nSlowly lower your heels below the platform for a full stretch. Repeat for your desired number of reps, keeping the movement controlled.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 9L,
                column: "Description",
                value: "Set Up the Bench\r\n\r\nLie flat on a bench with your feet firmly planted on the floor. Grip the barbell with hands slightly wider than shoulder-width, keeping your wrists straight.\r\nLower the Bar\r\n\r\nUnrack the bar and slowly lower it to your chest, keeping your elbows at a 45-degree angle to your body. Stop when the bar lightly touches your chest.\r\nPush the Bar Up\r\n\r\nPress the bar upward in a controlled motion until your arms are fully extended, keeping your chest engaged. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 10L,
                column: "Description",
                value: "Set Up for Dips\r\n\r\nGrasp the parallel bars with your hands shoulder-width apart. Lift yourself up so your arms are fully extended and your body is upright.\r\nLower Your Body\r\n\r\nSlowly lower yourself by bending your elbows, keeping them close to your sides. Lower until your upper arms are parallel to the floor or slightly deeper.\r\nPush Back Up\r\n\r\nPress through your palms to lift your body back up to the starting position, fully extending your arms. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 11L,
                column: "Description",
                value: "Grip the Bar\r\n\r\nStand tall and hold a barbell with an overhand (pronated) grip, hands shoulder-width apart. Let the bar rest at arm’s length in front of your thighs.\r\nCurl the Bar Up\r\n\r\nEngage your forearms and biceps to lift the bar upward, keeping your elbows close to your body. Stop when your forearms are fully contracted.\r\nLower the Bar\r\n\r\nSlowly lower the bar back to the starting position, fully extending your arms while maintaining control. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 12L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nSit on a bench and hold a barbell with an underhand (supinated) grip. Rest your forearms on your thighs with your wrists hanging just past your knees.\r\nCurl the Bar Up\r\n\r\nUse your wrists to curl the barbell upward, squeezing your forearms at the top of the movement. Keep your forearms stationary on your thighs.\r\nLower the Bar\r\n\r\nSlowly lower the barbell back down, allowing your wrists to extend fully for a good stretch. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 13L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nSit on the floor with your upper back against a bench, and roll a barbell over your hips. Your feet should be flat on the floor, about hip-width apart, and positioned so that your knees are bent at 90 degrees. Optionally, use a pad or towel on the barbell for comfort.\r\nLift Your Hips\r\n\r\nDrive through your heels and thrust your hips upward, fully extending your hips at the top. Squeeze your glutes hard at the peak of the movement. Keep your chest open and avoid arching your lower back excessively.\r\nLower Back Down\r\n\r\nSlowly lower your hips back down until your glutes are just above the floor, maintaining control throughout. Repeat for your desired number of reps, focusing on a slow, controlled movement and squeezing your glutes.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 14L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand with your feet wider than shoulder-width apart and your toes pointing outward at a 45-degree angle. Hold a dumbbell with both hands, letting it hang between your legs. Keep your chest up and your back straight.\r\nLower Your Hips\r\n\r\nPush your hips back and bend your knees, lowering your body towards the floor. Keep the dumbbell close to your body as you descend. Go as low as your flexibility allows, ideally until your thighs are parallel to the floor.\r\nPush Back Up\r\n\r\nPress through your heels and engage your glutes to lift your body back up to the starting position, fully extending your legs. Squeeze your glutes at the top of the movement and repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 15L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand with your feet hip-width apart, with the barbell over the middle of your feet. Grip the bar with your hands just outside your knees, using either an overhand or mixed grip. Keep your back straight, chest up, and core engaged.\r\nLift the Bar\r\n\r\nHinge at your hips (not your waist), lowering your torso while keeping the bar close to your body. Push your hips back, not down, and feel the stretch in your hamstrings as you lower the bar. Your knees should bend slightly.\r\nReturn to Standing\r\n\r\nDrive through your heels and push your hips forward to return to a standing position, fully extending your hips and knees. Squeeze your glutes and hamstrings at the top, then lower the bar back down with control. Repeat for your desired number of reps.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 16L,
                column: "Description",
                value: "Set Up the Machine\r\n\r\nPosition yourself on the reverse leg extension machine (also known as a leg curl machine), lying face down with your knees just off the edge of the pad. Adjust the machine so that the ankle pads are placed just above your heels. Grip the handles for stability and ensure your body is aligned properly.\r\nCurl Your Legs Up\r\n\r\nContract your hamstrings and curl your legs upward, bringing your heels toward your glutes. Focus on squeezing your hamstrings at the top of the movement.\r\nLower Back Down\r\n\r\nSlowly extend your legs back to the starting position, maintaining control over the movement. Avoid letting the weight drop quickly. Repeat for your desired number of reps, keeping the movement controlled and focusing on the hamstrings throughout.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 17L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand with your feet shoulder-width apart and the barbell resting on your upper traps (for barbell squats). Keep your chest up, shoulders back, and your core engaged. Your toes can point slightly outward, but your knees should track over your toes throughout the movement.\r\nLower Your Body\r\n\r\nPush your hips back and bend your knees, lowering your body into a squat. Keep your torso upright, and focus on driving your knees forward, towards your toes, while maintaining balance. Lower yourself until your thighs are parallel to the floor or deeper if your flexibility allows.\r\nPush Back Up\r\n\r\nPress through your heels and the balls of your feet to push your body back up to the starting position, straightening your legs and engaging your quads at the top. Repeat for your desired number of reps, maintaining control and proper form throughout.\r\nTip:\r\n\r\nTo emphasize the quads more, focus on keeping your torso more upright and allow your knees to travel forward over your toes during the squat.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 18L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand a few feet away from a bench or elevated surface. Place one foot on the bench behind you, ensuring your back knee is bent and not touching the floor. Keep your front foot flat on the ground, and your chest upright.\r\nLower Your Body\r\n\r\nBend your front knee to lower your body toward the floor, keeping your torso as upright as possible. Your front knee should track over your toes, and your back knee should lower toward the floor. Lower until your front thigh is parallel to the floor or slightly deeper, feeling a stretch in your quads.\r\nPush Back Up\r\n\r\nPress through your front heel to return to the starting position, straightening your front leg and engaging your quads. Repeat for the desired number of reps, then switch legs.\r\nTip\r\n\r\nTo emphasize the quads more, focus on keeping your torso upright and avoid leaning forward, which can shift the focus to the glutes and hamstrings.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 19L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nSit on a bench with back support or stand with your feet shoulder-width apart. Hold a dumbbell in each hand at shoulder height with your palms facing forward and elbows bent at a 90-degree angle. Keep your core engaged and your back straight.\r\nPress the Dumbbells Up\r\n\r\nPress the dumbbells upward until your arms are fully extended above your head, without locking your elbows. Exhale as you press the weights up, maintaining control throughout the movement.\r\nLower the Dumbbells\r\n\r\nSlowly lower the dumbbells back down to shoulder height, inhaling as you return to the starting position. Keep your elbows at a slight angle and avoid letting your shoulders shrug up toward your ears. Repeat for your desired number of reps.\r\nTip\r\n\r\nTo avoid straining your lower back, engage your core and avoid leaning back during the press.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 20L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand with your feet shoulder-width apart, holding a dumbbell in each hand with your arms fully extended in front of you, palms facing down. Keep your core engaged and your chest up.\r\nRaise the Dumbbells\r\n\r\nLift both dumbbells in front of you, keeping your arms straight (with a slight bend in the elbows) and raising them to shoulder height. Exhale as you raise the dumbbells, focusing on using your shoulders to lift.\r\nLower the Dumbbells\r\n\r\nSlowly lower the dumbbells back to the starting position with control, inhaling as you lower them. Avoid swinging the weights or using momentum. Repeat for your desired number of reps.\r\nTip\r\n\r\nTo avoid straining your shoulders, keep the movement slow and controlled, and don't raise the dumbbells higher than shoulder height to prevent excessive shoulder stress.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 21L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nStand facing the cable machine with a rope, bar, or V-bar attachment at the high pulley. Grip the attachment with both hands, palms facing down (pronated grip). Step back slightly so there's tension in the cable, and stand with your feet shoulder-width apart, keeping your elbows at your sides and your upper arms stationary.\r\nPush the Attachment Down\r\n\r\nPush the rope or bar down by extending your elbows, fully contracting your triceps at the bottom of the movement. Keep your forearms parallel to the floor, and your elbows should stay close to your torso throughout the movement.\r\nReturn Slowly\r\n\r\nSlowly allow the attachment to return to the starting position by bending your elbows, controlling the weight on the way up. Repeat for your desired number of reps.\r\nTip\r\n\r\nAvoid using momentum or leaning forward to help push the weight down. Keep your core engaged and focus on squeezing your triceps at the bottom of the movement.");

            migrationBuilder.UpdateData(
                table: "MuscleGroupExercises",
                keyColumn: "Id",
                keyValue: 22L,
                column: "Description",
                value: "Set Up the Exercise\r\n\r\nLie on a flat bench with an EZ bar in your hands, gripping it with an overhand (pronated) grip. Your hands should be about shoulder-width apart, and the bar should be directly over your chest with your arms fully extended. Keep your feet flat on the floor, and your core engaged to stabilize your body.\r\nLower the Bar\r\n\r\nSlowly lower the EZ bar toward your forehead by bending your elbows, keeping your upper arms stationary. Keep your elbows pointed forward, and lower the bar until your forearms are parallel to the ground or slightly beyond, feeling a stretch in your triceps.\r\nPush the Bar Back Up\r\n\r\nPress the EZ bar back up by extending your elbows, fully contracting your triceps at the top of the movement. Be sure to keep control of the weight and avoid letting your elbows flare out. Repeat for your desired number of reps.\r\nTip\r\n\r\nTo avoid stress on your elbows or shoulders, keep your movements slow and controlled. Focus on using your triceps to lift the weight, and avoid arching your back.");
        }
    }
}
