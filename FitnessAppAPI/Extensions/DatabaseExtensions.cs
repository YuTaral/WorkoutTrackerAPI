using Microsoft.EntityFrameworkCore;
using FitnessAppAPI.Data;

namespace FitnessAppAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring the database context in the application.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Adds the database context to the service collection and configures it to use SQL Server.
        /// </summary>
        /// <param name="services">The service collection to add the database context to.</param>
        /// <param name="configuration">The application's configuration settings.</param>
        /// <returns>The service collection with the added database context.</returns>
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("FitnessAppAPIContextConnection")
                ?? throw new InvalidOperationException("Connection string 'FitnessAppAPIContextConnection' not found.");

            services.AddDbContext<FitnessAppAPIContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
