using FitnessAppAPI.Middlewares;

namespace FitnessAppAPI.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring middleware and HTTP request pipeline in the application.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Configures Swagger middleware for API documentation in the application.
        /// </summary>
        /// <param name="app">The web application to configure Swagger for.</param>
        /// <returns>The configured web application.</returns>
        public static WebApplication ConfigureSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            return app;
        }

        /// <summary>
        /// Configures middleware for routing, authentication, and authorization in the application.
        /// </summary>
        /// <param name="app">The web application to configure middleware for.</param>
        /// <returns>The configured web application.</returns>
        public static WebApplication ConfigureMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
