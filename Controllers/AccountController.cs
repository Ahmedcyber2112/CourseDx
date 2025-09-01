using CourseDx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq; // Added for SelectMany in error display snippet

namespace CourseDx.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            var login = new Login();
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByNameAsync(login.UserName) ?? await _userManager.FindByEmailAsync(login.UserName);

                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(appUser, login.Password, login.Remember, false);

                    if (result.Succeeded)
                    {
                        if (appUser.IsAdmin)
                        {
                            return RedirectToAction("AdminHome", "Admin");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            return View(login);
        }

        public async Task<IActionResult> Signup()
        {
            var vm = new Models.SignupViewModel();
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(Models.SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingByEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return View(model);
            }

            var existingByUserName = await _userManager.FindByNameAsync(model.Username);
            if (existingByUserName != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already taken.");
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsAdmin = false
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                foreach (var err in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        [Authorize] // Ensure user is logged in to access profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // This shouldn't happen with [Authorize], but for safety
                return RedirectToAction("Login");
            }

            var vm = new ProfileViewModel
            {
                Username = user.UserName!, // Null-forgiving operator as UserName is generally not null for an authenticated user
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty, // Handle potential nulls
                LastName = user.LastName ?? string.Empty
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Ensure user is logged in to submit profile changes
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            bool attemptingPasswordChange =
                !string.IsNullOrEmpty(model.CurrentPassword) ||
                !string.IsNullOrEmpty(model.NewPassword) ||
                !string.IsNullOrEmpty(model.ConfirmNewPassword);

            // --- 1. Handle Basic Profile Data Update (FirstName, LastName, Username, Email) ---
            if (!attemptingPasswordChange)
            {
                // If not attempting password change, remove errors related to password fields from ModelState.
                // This allows non-password updates to proceed even if password fields were partially filled/invalid.
                ModelState.Remove(nameof(ProfileViewModel.CurrentPassword));
                ModelState.Remove(nameof(ProfileViewModel.NewPassword));
                ModelState.Remove(nameof(ProfileViewModel.ConfirmNewPassword));
            }

            // Perform initial ModelState validation for non-password fields.
            // If there are errors here, return the view immediately.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool anyProfileDataChanged = false; // Tracks changes to FirstName, LastName

            // Update FirstName
            if (user.FirstName != model.FirstName)
            {
                user.FirstName = model.FirstName;
                anyProfileDataChanged = true;
            }

            // Update LastName
            if (user.LastName != model.LastName)
            {
                user.LastName = model.LastName;
                anyProfileDataChanged = true;
            }

            // Update Username if changed
            if (user.UserName != model.Username)
            {
                var existing = await _userManager.FindByNameAsync(model.Username);
                if (existing != null && existing.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(ProfileViewModel.Username), "Username is already taken.");
                    return View(model);
                }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Username);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var err in setUserNameResult.Errors)
                        ModelState.AddModelError(nameof(ProfileViewModel.Username), err.Description);
                    return View(model);
                }
                // setUserNameResult.Succeeded implies that the user object has been updated and saved internally.
                // So, no need to set anyProfileDataChanged for this one.
            }

            // Update Email if changed
            if (user.Email != model.Email)
            {
                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null && existingEmail.Id != user.Id)
                {
                    ModelState.AddModelError(nameof(ProfileViewModel.Email), "Email is already in use.");
                    return View(model);
                }
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var err in setEmailResult.Errors)
                        ModelState.AddModelError(nameof(ProfileViewModel.Email), err.Description);
                    return View(model);
                }
                user.EmailConfirmed = true; // Email confirmed as user successfully updated it
                // setEmailResult.Succeeded implies that the user object has been updated and saved internally.
            }

            // If only FirstName or LastName changed (Username/Email handled by Set*Async),
            // then we need to explicitly call UpdateAsync to persist these changes.
            // If Username or Email also changed, Set*Async would have already saved the user.
            // Calling UpdateAsync here again for FirstName/LastName is safe.
            if (anyProfileDataChanged)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var err in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, err.Description);
                    return View(model);
                }
            }

            // --- 2. Handle Password Change ---
            if (attemptingPasswordChange)
            {
                // Manual validation for password fields if they are explicitly being changed.
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError(nameof(ProfileViewModel.CurrentPassword), "Current password is required to change your password.");
                }
                if (string.IsNullOrEmpty(model.NewPassword))
                {
                    ModelState.AddModelError(nameof(ProfileViewModel.NewPassword), "New password is required.");
                }
                if (string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    ModelState.AddModelError(nameof(ProfileViewModel.ConfirmNewPassword), "Confirm new password is required.");
                }
                // The [Compare] attribute on ConfirmNewPassword handles matching if NewPassword is not empty.

                // If no manual validation errors for password fields, proceed with change.
                if (ModelState.IsValid)
                {
                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
                    if (!changePasswordResult.Succeeded)
                    {
                        foreach (var err in changePasswordResult.Errors)
                            ModelState.AddModelError(string.Empty, err.Description);
                    }
                }
            }

            // Final check for any ModelState errors (from password change or other late issues)
            if (!ModelState.IsValid)
            {
                // If we reach here, basic profile changes (if any) have been saved.
                // We return the view to display the password-related errors.
                return View(model);
            }

            // Re-sign in to refresh claims if username/email changed or password changed.
            // This is important for security and to ensure the user context is up-to-date.
            await _signInManager.RefreshSignInAsync(user);

            ViewData["SuccessMessage"] = "Profile updated successfully!";
            // Return the view to show the success message and updated data.
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            await _signInManager.SignOutAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> MyCourses()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}