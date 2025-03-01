using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.Workouts;
using FitnessAppAPI.Data.Services.Exercises;
using FitnessAppAPI.Data.Services.MuscleGroups;
using FitnessAppAPI.Data;
using FitnessAppAPI.Data.Services.WorkoutTemplates;
using FitnessAppAPI.Data.Services.UserProfile;
using FitnessAppAPI.Data.Services.SystemLogs;
using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.Teams;
using FitnessAppAPI.Data.Services.Notifications;

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
            // DbContext in .NET is registered as scoped by default (when using AddDbContext<T>),
            // so services that depend on it should also be scoped
            // to ensure they share the same DbContext instance within a request.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWorkoutService, WorkoutService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IMuscleGroupService, MuscleGroupService>();
            services.AddScoped<IWorkoutTemplateService, WorkoutTemplateService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ISystemLogService, SystemLogService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
