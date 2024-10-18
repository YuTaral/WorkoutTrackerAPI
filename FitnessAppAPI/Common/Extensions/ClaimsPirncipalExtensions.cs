using System.Security.Claims;

namespace FitnessAppAPI.Common.Extensions
{
    /// <summary>
    ///     User Claims Principal extension
    /// </summary>
    public static class ClaimsPirncipalExtensions
    {
        /// <summary>
        ///     Returns the logged in user id
        /// </summary>
        public static string GetId(this ClaimsPrincipal user)
        {
            var foundUser = user.FindFirst(ClaimTypes.NameIdentifier);

            if (foundUser != null) {
                return foundUser.Value;
            }

            return "";

        }
    }
}
