using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitnessAppAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring JWT authentication in the application.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds JWT authentication services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add authentication services to.</param>
        /// <param name="configuration">The application's configuration settings.</param>
        /// <returns>The service collection with the added authentication services.</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes("ThisIsAReallyLongSecretKey12345!"); // Secure this key

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}
