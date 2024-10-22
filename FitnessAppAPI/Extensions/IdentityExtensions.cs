using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data;

namespace FitnessAppAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring Identity services in the application.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Adds Identity services to the service collection, configuring user and role management.
        /// </summary>
        /// <param name="services">The service collection to add Identity services to.</param>
        /// <returns>The service collection with the added Identity services.</returns>
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<FitnessAppAPIContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
