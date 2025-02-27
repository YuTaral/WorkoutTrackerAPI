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
using Microsoft.EntityFrameworkCore;
using System.Net;
using static FitnessAppAPI.Common.Constants;


namespace FitnessAppAPI.Data
{
    /// <summary>
    ///     User service class to implement IUserService interface.
    /// </summary>
    public class UserService: BaseService, IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IUserStore<User> userStore;
        private readonly IUserEmailStore<User> emailStore;
        private readonly SignInManager<User> singInManager;
        private readonly IConfiguration configuration;
        private readonly IUserProfileService userProfileService;

        public UserService(UserManager<User> userManagerObj, IUserStore<User> userStoreObj, SignInManager<User> signInManagerObj,
                       IConfiguration configurationObj, FitnessAppAPIContext DB,
                       IUserProfileService userProfileS) : base(DB)
        {
            userManager = userManagerObj;
            userStore = userStoreObj;
            emailStore = GetEmailStore();
            singInManager = signInManagerObj;
            configuration = configurationObj;
            userProfileService = userProfileS;
        }

        public async Task<ServiceActionResult<BaseModel>> Register(Dictionary<string, string> requestData)
        {
            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_REG_FAIL);
            }

            // Validate
            if (!Utils.IsValidEmail(email))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_REG_FAIL_EMAIL);
            }

            // Create the user
            var user = CreateUser();
            userStore.SetUserNameAsync(user, email, CancellationToken.None).Wait();
            emailStore.SetEmailAsync(user, email, CancellationToken.None).Wait();
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Utils.UserErrorsToString(result.Errors));
            }

            // Create the default values for the user
            var createUserDefaultValuesResult = await userProfileService.AddUserDefaultValues(user.Id);
            if (!createUserDefaultValuesResult.IsSuccess())
            {
                return new ServiceActionResult<BaseModel>((HttpStatusCode)createUserDefaultValuesResult.Code, createUserDefaultValuesResult.Message);
            }

            // Create the user profile
            var createUserProfile = await userProfileService.CreateUserProfile(user.Id, email);
            if (!createUserProfile.IsSuccess())
            {
                return new ServiceActionResult<BaseModel>((HttpStatusCode)createUserDefaultValuesResult.Code, createUserDefaultValuesResult.Message);
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.Created);

        }

        public async Task<TokenResponseModel> Login(Dictionary<string, string> requestData)
        {
            ServiceActionResult<UserModel> serviceActionResult;

            /// Check if username and password are provided
            if (!requestData.TryGetValue("email", out string? email) || !requestData.TryGetValue("password", out string? password))
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, MSG_LOGIN_FAILED);
                return new TokenResponseModel("", serviceActionResult); 
            }

            // Login attempt
            var result = await singInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);

            // Process result
            if (!result.Succeeded)
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, MSG_LOGIN_FAILED);
                return new TokenResponseModel("", serviceActionResult);
            }

            // Retrieve the logged in user
            var user = await userManager.FindByEmailAsync(email);

            if (user == null || user.Email == null)
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, MSG_LOGIN_FAILED);
                return new TokenResponseModel("", serviceActionResult);
            }

            // Generate JwtToken
            var token = GenerateJwtToken(user);
            if (token == "")
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, MSG_LOGIN_FAILED);
            } 
            else
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.OK, MSG_SUCCESS, [await GetUserModel(email)]);
            }

            return new TokenResponseModel(token, serviceActionResult);
        }
        public async Task<ServiceActionResult<BaseModel>> Logout()
        {
            await singInManager.SignOutAsync();
            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<ServiceActionResult<BaseModel>> ChangePassword(Dictionary<string, string> requestData, string userId)
        {
            /// Check if new pass is provided
            if (!requestData.TryGetValue("oldPassword", out string? oldPassword) || !requestData.TryGetValue("password", out string? password))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_CHANGE_PASS_FAIL);
            }

            // Find the user by id
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_USER_DOES_NOT_EXISTS);
            }

            // Change the password
            var result = await userManager.ChangePasswordAsync(user, oldPassword, password);

            if (!result.Succeeded)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Utils.UserErrorsToString(result.Errors));
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
        }

        public async Task<TokenResponseModel> ValidateToken(string token, string userId)
        {
            var returnToken = "";
            ServiceActionResult<UserModel> serviceActionResult;

            if (string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token)) {
                // Make sure the userId is set when validation token
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                userId = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var handler = new JwtSecurityTokenHandler();
            var keyString = configuration["JwtSettings:SecretKey"];

            if (keyString == null)
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.InternalServerError, MSG_TOKEN_VALIDATION_FAILED);
                return new TokenResponseModel("", serviceActionResult);
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
                    var user = await DBAccess.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        returnToken = GenerateJwtToken(user);
                        serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.Unauthorized, MSG_TOKEN_EXPIRED);

                        return new TokenResponseModel(returnToken, serviceActionResult);
                    }
                }

                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.OK);
            }
            catch
            {
                serviceActionResult =  new ServiceActionResult<UserModel>(HttpStatusCode.Unauthorized, MSG_TOKEN_EXPIRED);
            }

            return new TokenResponseModel(returnToken, serviceActionResult);
        }

        public async Task<UserModel> GetUserModel(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ModelMapper.GetEmptyUserModel();
            }

            var defaultValues = await GetUserDefaultValues(0, user.Id);

            if (defaultValues != null)
            {
                var weightUnit = await DBAccess.WeightUnits.Where(w => w.Id == defaultValues.WeightUnitId).FirstOrDefaultAsync();
                var profile = await DBAccess.UserProfiles.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();

                return ModelMapper.MapToUserModel(user, defaultValues, weightUnit, profile);
            }

            return ModelMapper.GetEmptyUserModel();
        }

        /// <summary>
        ///     Generate JwtToken for the logged in user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];

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
                expires: DateTime.Now.AddDays(10),
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
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)userStore;
        }
    }
}
