using Microsoft.AspNetCore.Identity;
using FitnessAppAPI.Data.Models;
using FitnessAppAPI.Data.Services;
using FitnessAppAPI.Data.Services.User.Models;
using FitnessAppAPI.Common;

namespace FitnessAppAPI.Data
{
    /// <summary>
    ///     User service class to implement IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        ///     Class constructor.
        /// </summary>
        public UserService(UserManager<User> userManager, IUserStore<User> userStore, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
        }

        /// <summary>
        ///     Register the user.
        /// </summary>
        /// <param name="email">
        ///     The user email.
        /// </param>
        /// <param name="password">
        ///     The user password.
        /// </param>
        public async Task<String> Register(string email, string password)
        {
            // Validate 
            if (!Utils.IsValidEmail(email))
            {
                return Constants.MSG_REG_FAIL_EMAIL;
            }

            // Create the user
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Utils.UserErrorsToString(result.Errors);
            } 
            
            return Constants.MSG_SUCCESS;
        }

        /// <summary>
        ///     Login the user.
        /// </summary>
        /// <param name="email">
        ///     The user email.
        /// </param>
        /// <param name="password">
        ///     The user password.
        /// </param>
        public async Task<UserModel?> Login(string email, string password)
        {
            // Login attempt
            var result = await _signInManager.PasswordSignInAsync(email, password, true, lockoutOnFailure: false);

            // Process result
            if (!result.Succeeded)
            {
                return null;
            }

            // Retrieve the logged in user
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.Email == null) { 
                return null; 
            }

            var userData = new UserModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return userData;
        }


        /// <summary>
        /// Log Out the User
        /// </summary>
        public async Task<Boolean> Logout() {
            await _signInManager.SignOutAsync();
            return true;
        }

        /// <summary>
        ///     Create a User instance using the Activator.CreateInstance.
        /// </summary>
        /// <returns>
        ///     User object, if method fails returns InvalidOperationException.
        /// </returns>
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
        /// <returns>
        ///     UserStore object if user store supports user email, otherwise throws NotSupportedException.
        /// </returns>
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>) _userStore;
        }
    }
}
