using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessAppAPI.Data.Services.UserProfile;

namespace FitnessAppAPI.Data
{
    /// <summary>
    ///     User service class to implement IUserService interface.
    /// </summary>
    public class UserService: BaseService, IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUserProfileService _userProfileService;

        public UserService(UserManager<User> userManager, IUserStore<User> userStore, SignInManager<User> signInManager,
                       IConfiguration configuration, FitnessAppAPIContext DB,
                       IUserProfileService userProfileS) : base(DB)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _configuration = configuration;
            _userProfileService = userProfileS;
        }

        /// <summary>
        ///     Register the user
        /// </summary>
        /// <param name="email">
        ///     The user email
        /// </param>
        /// <param name="password">
        ///     The user password
        /// </param>
        public ServiceActionResult Register(string email, string password)
        {
            var userId = "";

            return ExecuteServiceAction(userId => {
                // Validate
                if (!Utils.IsValidEmail(email))
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_REG_FAIL_EMAIL);
                }

                // Create the user
                var user = CreateUser();
                _userStore.SetUserNameAsync(user, email, CancellationToken.None).Wait();
                _emailStore.SetEmailAsync(user, email, CancellationToken.None).Wait();
                var result = _userManager.CreateAsync(user, password).Result;

                if (!result.Succeeded)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Utils.UserErrorsToString(result.Errors));
                }

                // Create the default values for the user
                var createUserDefaultValuesResult = _userProfileService.AddUserDefaultValues(user.Id);
                if (!createUserDefaultValuesResult.IsSuccess())
                {
                    return new ServiceActionResult(createUserDefaultValuesResult.Code, createUserDefaultValuesResult.Message);
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_USER_REGISTER_SUCCESS);

            }, userId);
        }

        /// <summary>
        ///     Login the user
        /// </summary>
        /// <param name="email">
        ///     The user email
        /// </param>
        /// <param name="password">
        ///     The user password
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public TokenResponseModel Login(string email, string password)
        {
            var userId = "";
            var returnToken = "";
            var returnUserModel = ModelMapper.GetEmptyUserModel();

            var result = ExecuteServiceAction(userId => {
                // Login attempt
                var result = _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false).Result;

                // Process result
                if (!result.Succeeded)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_LOGIN_FAILED);
                }

                // Retrieve the logged in user
                var user = _userManager.FindByEmailAsync(email).Result;

                if (user == null || user.Email == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_LOGIN_FAILED);
                }

                // Generate JwtToken
                var token = GenerateJwtToken(user);
                if (token == "")
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_LOGIN_FAILED);
                }

                returnUserModel = CreateUserModel(user);
                returnToken = token;

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS);

            }, userId);

            
            return new TokenResponseModel
            {
                User = returnUserModel,
                Token = returnToken,
                Result = result
            };
        }

        /// <summary>
        ///     Log Out the User
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult Logout(string userId)
        {
            return ExecuteServiceAction(userId => {
                _signInManager.SignOutAsync().Wait();
                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS);
            }, userId);
        }

        /// <summary>
        ///     Change user passowrd
        /// </summary>
        public ServiceActionResult ChangePassword(string oldPassword, string password, string userId)
        {
            return ExecuteServiceAction(userId => {
                // Find the user by id
                var user = _userManager.FindByIdAsync(userId).Result;
                if (user == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
                }

                // Change the password
                var result = _userManager.ChangePasswordAsync(user, oldPassword, password).Result;

                if (!result.Succeeded)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Utils.UserErrorsToString(result.Errors));
                }

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_PASSWORD_CHANGED);
            }, userId); 
        }

        /// <summary>
        ///     Validate the token
        /// </summary>
        /// <param name="token">
        ///     The token to validate
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public TokenResponseModel ValidateToken(string token, string userId)
        {
            if (string.IsNullOrEmpty(userId)) {
                // Make sure the userId is set when validation token
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var returnToken = "";
            var returnUserModel = ModelMapper.GetEmptyUserModel();

            var result = ExecuteServiceAction(userId =>
            {
                var handler = new JwtSecurityTokenHandler();
                var keyString = _configuration["JwtSettings:SecretKey"];

                if (keyString == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_TOKEN_VALIDATION_FAILED);
                }

                var key = Encoding.ASCII.GetBytes(keyString);

                try
                {
                    handler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    if (RefreshJwtToken(validatedToken) && userId != "")
                    {
                        // If the token expires soon, generate new token
                        var user = DBAccess.Users.Where(u => u.Id == userId).FirstOrDefault();

                        if (user != null)
                        {
                            returnToken = GenerateJwtToken(user);
                            return new ServiceActionResult(Constants.ResponseCode.REFRESH_TOKEN, Constants.MSG_SUCCESS);
                        }
                    }

                    return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_SUCCESS);
                }
                catch
                {
                    return new ServiceActionResult(Constants.ResponseCode.TOKEN_EXPIRED, Constants.MSG_TOKEN_EXPIRED);
                }
            }, userId);

            return new TokenResponseModel
            {
                User = returnUserModel,
                Token = returnToken,
                Result = result
            };
        }

        /// <summary>
        ///     Generate JwtToken for the logged in user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                return "";
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddDays(14),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        ///     Return true if the current token expires within 2 days, false otherwse
        /// </summary>
        /// <param name="token">
        ///     The token to check    
        /// </param>
        /// <returns></returns>
        private static bool RefreshJwtToken(SecurityToken token)
        {
            // Extract the JWT token claims
            JwtSecurityToken? jwtToken = token as JwtSecurityToken;

            if (jwtToken == null)
            {
                // Refresh the token if parsing failed, it should not happen
                return true;
            }

            var expirationClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expirationClaim == null)
            {
                // Refresh the token if expiration date not found, it should not happen
                return true;
            }

            // Convert the expiration time to a DateTime
            var expirationUnixTime = long.Parse(expirationClaim.Value);
            var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(expirationUnixTime).DateTime;

            // Check if the token expires within 2 days
            if (expirationDateTime <= DateTime.UtcNow.AddDays(2))
            {
                return true;
            }

            // No refresh needed
            return false;
        }

        /// <summary>
        ///     Create a User instance using the Activator.CreateInstance.
        /// </summary>
        /// <returns>User object, if method fails returns InvalidOperationException.</returns>
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        /// <summary>
        ///     Check whether the backing user store supports user emails.
        /// </summary>
        /// <returns>UserStore object if user store supports user email, otherwise throws NotSupportedException.</returns>
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }

        /// <summary>
        ///     Create UserModel object
        /// </summary>
        /// <param name="user">
        ///     The user 
        /// </param>
        private UserModel? CreateUserModel(User user)
        {
            var defaultValues = GetUserDefaultValues(0, user.Id);

            if (defaultValues != null)
            {
                var weightUnit = DBAccess.WeightUnits.Where(w => w.Id == defaultValues.WeightUnitId).FirstOrDefault();
                return ModelMapper.MapToUserModel(user, defaultValues, weightUnit);
            }

            return null;
        }
    }
}
