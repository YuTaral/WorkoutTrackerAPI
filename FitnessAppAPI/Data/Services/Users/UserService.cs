using Azure;
using Azure.Communication.Email;
using FitnessAppAPI.Common;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services.SystemLogs;
using FitnessAppAPI.Data.Services.UserProfiles;
using FitnessAppAPI.Data.Services.Users.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static FitnessAppAPI.Common.Constants;

namespace FitnessAppAPI.Data.Services.Users
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
        private readonly ISystemLogService systemLogService;

        public UserService(UserManager<User> userManagerObj, IUserStore<User> userStoreObj, SignInManager<User> signInManagerObj,
                       IConfiguration configurationObj, FitnessAppAPIContext DB,
                       IUserProfileService userProfileS, ISystemLogService systemLogS) : base(DB)
        {
            userManager = userManagerObj;
            userStore = userStoreObj;
            emailStore = GetEmailStore();
            singInManager = signInManagerObj;
            configuration = configurationObj;
            userProfileService = userProfileS;
            systemLogService = systemLogS;
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
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_EMAIL);
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

        public async Task<TokenResponseModel> GoogleSignIn(Dictionary<string, string> requestData)
        {
            ServiceActionResult<UserModel> serviceActionResult;
            try
            {
                /// Check if username and password are provided
                if (!requestData.TryGetValue("idToken", out string? idToken))
                {
                    serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, GOOGLE_SIGN_IN_FAILED);
                    return new TokenResponseModel("", serviceActionResult);
                }

                // Validate the token
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [configuration["Authentication:Google:ClientId"]]
                });

                if (payload.JwtId == null || payload.Email == null)
                {
                    serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, GOOGLE_SIGN_IN_FAILED);
                    return new TokenResponseModel("", serviceActionResult);
                }

                // TODO: Register the user and auto log in
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, GOOGLE_SIGN_IN_FAILED);
                return new TokenResponseModel("", serviceActionResult);

            }
            catch (InvalidJwtException)
            {
                serviceActionResult = new ServiceActionResult<UserModel>(HttpStatusCode.BadRequest, GOOGLE_SIGN_IN_FAILED);
                return new TokenResponseModel("", serviceActionResult);
            }
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
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_PASSWORD_NOT_PROVIDED);
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

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_PASSWORD_RESET_SUCCESS);
        }

        public async Task<ServiceActionResult<BaseModel>> ResetPassword(Dictionary<string, string> requestData)
        {
            // Check if email is provided
            if (!requestData.TryGetValue("email", out string? email))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_EMAIL);
            }

            // Check if new pass is provided
            if (!requestData.TryGetValue("password", out string? password))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_PASSWORD_NOT_PROVIDED);
            }


            // Find the user by id
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.NotFound, MSG_USER_DOES_NOT_EXISTS);
            }

            // Change the password
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, Utils.UserErrorsToString(result.Errors));
            }

            // Delete any reset password codes for this user
            await DeleteResetPasswordRecords(user.Id);

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_PASSWORD_RESET_SUCCESS);
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
                var profile = await DBAccess.UserProfiles.Where(p => p.UserId == user.Id).FirstOrDefaultAsync();

                return ModelMapper.MapToUserModel(user, defaultValues, profile);
            }

            return ModelMapper.GetEmptyUserModel();
        }

        public async Task<ServiceActionResult<BaseModel>> SendCode(Dictionary<string, string> requestData)
        {
            /// Validate email
            if (!requestData.TryGetValue("email", out string? email) || !Utils.IsValidEmail(email))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_EMAIL);
            }

            string code = new Random().Next(100000, 1000000).ToString();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // Although the user with this email does not exist, return success message to avoid email enumeration
                return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_CHECK_EMAIL);
            }

            if (! await SendEmail(code, email))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.InternalServerError, MSG_UNEXPECTED_ERROR_WHILE_SENDING_EMAIL);
            }

            // Create new record
            var record = new PasswordResetCode
            {
                UserId = user.Id,
                HashedCode = HashCode(code),
                ExpirationDate = DateTime.UtcNow.AddMinutes(10)
            };

            await DBAccess.PasswordResetCodes.AddAsync(record);
            await DBAccess.SaveChangesAsync();

            return new ServiceActionResult<BaseModel>(HttpStatusCode.OK, MSG_CHECK_EMAIL);
        }

        public async Task<ServiceActionResult<BaseModel>> VerifyCode(Dictionary<string, string> requestData)
        {
            /// Validate
            if (!requestData.TryGetValue("code", out string? code))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_CODE);
            }

            if (!requestData.TryGetValue("email", out string? email))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_EMAIL);
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Return invalid code message to avoid email enumeration
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_CODE);
            }

            var record = await DBAccess.PasswordResetCodes
                .Where(r => r.UserId == user.Id && r.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(r => r.ExpirationDate)
                .FirstOrDefaultAsync();

            if (record == null)
            {
                // Return invalid code message
                return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_CODE);
            }

            if (CompareCodeHash(code, record.HashedCode))
            {
                return new ServiceActionResult<BaseModel>(HttpStatusCode.OK);
            }

            return new ServiceActionResult<BaseModel>(HttpStatusCode.BadRequest, MSG_INVALID_CODE);
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
                expires: DateTime.UtcNow.AddDays(10),
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

        /// <summary>
        ///     Hash the 6 digits code for password reset
        /// </summary>
        /// <param name="code">
        ///     The 6 digits code to hash
        /// </param>
        public static string HashCode(string code)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(
                password: Encoding.UTF8.GetBytes(code),
                salt: RandomNumberGenerator.GetBytes(16),
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256
            );

            var salt = deriveBytes.Salt;
            var key = deriveBytes.GetBytes(32); 
            return Convert.ToBase64String(salt.Concat(key).ToArray());
        }

        /// <summary>
        ///     Compare the code against the specified hash code
        /// </summary>
        /// <param name="inputCode">
        ///     The non hashed code
        /// </param>
        /// <param name="hashedCode">
        ///     The hashed code
        /// </param>
        public bool CompareCodeHash(string inputCode, string hashedCode)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedCode);
            byte[] salt = hashBytes[..16];       
            byte[] key = hashBytes[16..];       

            using var deriveBytes = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(inputCode),
                salt,
                100_000,
                HashAlgorithmName.SHA256
            );
            byte[] inputKey = deriveBytes.GetBytes(32);

            return inputKey.SequenceEqual(key);
        }

        /// <summary>
        ///     Send email with the code to the recipient and return true if it was successfull
        /// </summary>
        /// <param name="code">
        ///     The code to send
        /// </param>
        /// <param name="recipient">
        ///     The recipient email
        /// </param>
        private async Task<bool> SendEmail(string code, string recipient)
        {
            string? connectionString = configuration["Email:ConnectionString"];
            string? sender = configuration["Email:Sender"];

            if (connectionString == null || sender == null)
            {
                await systemLogService.Add(new Exception("Email connection string or sender not configured"), "");
                return false;
            }

            var emailClient = new EmailClient(connectionString);
            var subject = "WorkoutTracker - Password Reset Code";
            var message = $@"
                            <html>
                              <body style='font-family: Arial, sans-serif;'>
                                <h2 style='color:#2E86C1;'>WorkoutTracker Password Reset</h2>
                                <p>Hello,</p>
                                <p>We received a request to reset your password for your <strong>WorkoutTracker</strong> account.</p>
                                <p>Please use the following code to reset your password:</p>
                                <h1 style='color:#D35400;'>{code}</h1>
                                <p>This code will expire in 5 minutes. If you did not request a password reset, please ignore this email.</p>
                                <br />
                                <p>Stay strong,</p>
                                <p><em>The WorkoutTracker Team</em></p>
                              </body>
                            </html>";

            var emailMessage = new EmailMessage(
                senderAddress: sender,
                content: new EmailContent(subject)
                {
                    PlainText = $"Your WorkoutTracker password reset code is: {code}\nThis code will expire in 5 minutes.",
                    Html = message
                },
                recipients: new EmailRecipients(
                [
                    new EmailAddress(recipient)
                ])
            );

            try
            {
                EmailSendOperation emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                if (emailSendOperation.Value.Status == EmailSendStatus.Failed)
                {
                    if (emailSendOperation.Value.Status == EmailSendStatus.Failed)
                    {
                        await systemLogService.Add(new Exception($"Failed to send code to email: {recipient}"), "");
                        return false;
                    }
                }
                 
            }
            catch (Exception e)
            {
                await systemLogService.Add(e, "");
                return false;
            }
           
            return true;
        }

        /// <summary>
        ///     Delete any password reset records for this user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        private async Task<bool> DeleteResetPasswordRecords(string userId)
        {
            var previous = await DBAccess.PasswordResetCodes.Where(c => c.UserId == userId).ToListAsync();
            DBAccess.PasswordResetCodes.RemoveRange(previous);
            await DBAccess.SaveChangesAsync();

            return true;
        }
    }
}
