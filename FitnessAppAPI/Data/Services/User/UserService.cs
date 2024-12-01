using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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

        public UserService(UserManager<User> userManager, IUserStore<User> userStore, SignInManager<User> signInManager,
                       IConfiguration configuration, FitnessAppAPIContext DB) : base(DB)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _configuration = configuration;
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

                CreateDefaultValues(user.Id);

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
        public LoginResponseModel Login(string email, string password)
        {
            var userId = "";
            var returnToken = "";
            var returnUserModel = new UserModel { 
                Id = "",
                Email = "",
                DefaultValues = new UserDefaultValuesModel { 
                    Id = 0,
                    Sets = 0,
                    Reps = 0,
                    Weight = 0,
                    Completed = false,
                    WeightUnitText = ""
                }
            };

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

            
            return new LoginResponseModel
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
        ///     Change user default values
        /// </summary>
        /// <param name="data">
        ///     The new default values
        /// </param>
        /// <param name="userId">
        ///     The user id
        /// </param>
        public ServiceActionResult ChangeUserDefaultValues(UserDefaultValuesModel data, string userId)
        {
            return ExecuteServiceAction(userId =>
            {
                var existing = GetUserDefaultValues(userId);
                if (existing == null)
                {
                    return new ServiceActionResult(Constants.ResponseCode.FAIL, Constants.MSG_USER_DOES_NOT_EXISTS);
                }

                // Find the unit record and set the code, the model contains the Text column
                var unitRecord = DBAccess.WeightUnits.Where(w => w.Text == data.WeightUnitText).FirstOrDefault();
                var unitCode = "";

                if (unitRecord == null) {
                    unitCode = existing.WeightUnitCode;
                } 
                else
                {
                    unitCode = unitRecord.Code;
                }

                // Change the record
                existing.Sets = data.Sets;
                existing.Reps = data.Reps;
                existing.Weight = data.Weight;
                existing.Completed = data.Completed;
                existing.WeightUnitCode = unitCode;

                DBAccess.Entry(existing).State = EntityState.Modified;
                DBAccess.SaveChanges();

                return new ServiceActionResult(Constants.ResponseCode.SUCCESS, Constants.MSG_DEF_VALUES_UPDATED, 
                    [ModelMapper.MapToUserDefaultValuesModel(existing, DBAccess)]);

            }, userId);
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
        ///     Create record in ExerciseDefaultValue with the default values for the user
        /// </summary>
        /// <param name="userId">
        ///     The user id
        /// </param>
        private void CreateDefaultValues(string userId)
        {
            var kg = DBAccess.WeightUnits.Where(w => w.Code == "KG").FirstOrDefault();
            if (kg == null) { 
                // Must NOT happen
                return;
            }

            // Create ExerciseDefaultValue record for the user
            var defaultValues = new UserDefaultValue
            {
                Sets = 0,
                Reps = 0,
                Weight = 0,
                WeightUnitCode = kg.Code,
                Completed = false,
                UserId = userId
            };

            DBAccess.UserDefaultValues.Add(defaultValues);
            DBAccess.SaveChanges();
        }

        /// <summary>
        ///     Create UserModel object
        /// </summary>
        /// <param name="user">
        ///     The user 
        /// </param>
        private UserModel CreateUserModel(User user)
        {
            var defaultValues = GetUserDefaultValues(user.Id);
            var weightUnit = "";

            if (defaultValues != null)
            {
                weightUnit = DBAccess.WeightUnits.Where(w => w.Code == defaultValues.WeightUnitCode)
                                                    .Select(w => w.Text)
                                                    .FirstOrDefault();
            }

            weightUnit ??= "";

            return ModelMapper.MapToUserModel(user, defaultValues, weightUnit);
        }


        /// <summary>
        ///     Return the user default values for exercises
        /// </summary>
        /// <param name="userId">
        ///     The user Id
        /// </param>
        private UserDefaultValue? GetUserDefaultValues(string userId)
        {
            // There must be only one record for the user with exercise id = null
            // This record represents the default values for all exercise
            return DBAccess.UserDefaultValues.Where(u => u.UserId == userId && u.MGExeciseId == null).FirstOrDefault();
        }
    }
}
