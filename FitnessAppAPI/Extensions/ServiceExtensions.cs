using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.MuscleGroups;
using FitnessAppAPI.Data;
using FitnessAppAPI.Data.Services.WorkoutTemplates;
using FitnessAppAPI.Data.Services.UserProfile;
using FitnessAppAPI.Data.Services.SystemLogs;

namespace FitnessAppAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for registering application-specific services in the service collection.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds application services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection with the added application services.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddScoped<GlobalExceptionFilter>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWorkoutService, WorkoutService>();
            services.AddTransient<IExerciseService, ExerciseService>();
            services.AddTransient<IMuscleGroupService, MuscleGroupService>();
            services.AddTransient<IWorkoutTemplateService, WorkoutTemplateService>();
            services.AddTransient<IUserProfileService, UserProfileService>();
            services.AddTransient<ISystemLogService, SystemLogService>();

            return services;
        }
    }
}
