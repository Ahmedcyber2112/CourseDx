using CourseDx.Data;
using CourseDx.Models;
using CourseDx.Models.Admin; // Make sure this is using the updated model
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System; // Required for GroupBy month/year
using System.Collections.Generic; // Required for List

namespace CourseDx.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly CourseDxContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, CourseDxContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> AdminHome()
        {
            // Course Enrollment Data (Students per Course)
            var courseEnrollmentData = await _context.CourseEnrollment
                .Include(e => e.CourseDetals)
                .GroupBy(e => e.CourseDetals.Title)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = g.Count()
                })
                .OrderByDescending(x => x.Value) // Show most popular courses first
                .ToListAsync();

            // Create the model and pass the data to the view
            var model = new HomeStatisticsModel
            {
                CourseEnrollmentData = courseEnrollmentData,

                // Total counts
                EnrollmentsCount = await _context.CourseEnrollment.CountAsync(),
                InstractorsCount = await _context.Instractor.CountAsync(),
                StudentsCount = await _context.Students.CountAsync(),
                TotalRevenue = await _context.CourseEnrollment
                    .Include(e => e.CourseDetals)
                    .SumAsync(e => (decimal?)e.CourseDetals.Price) ?? 0
            };

            // Make sure the view name is AdminHome.cshtml
            return View(model);
        }

        public IActionResult Index()
        {
            var data = _userManager.Users.ToList();
            return View(data);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user);
                }

                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsAdmin = user.IsAdmin
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    // Assign Admin role if the checkbox was set
                    if (user.IsAdmin)
                    {
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));

                        await _userManager.AddToRoleAsync(appUser, "Admin");
                    }

                    return RedirectToAction("Index");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Update(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var vm = new CourseDx.Models.Admin.UpdateUserViewModel
                {
                    Id = user.Id,
                    ConcurrencyStamp = user.ConcurrencyStamp ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin
                };
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(CourseDx.Models.Admin.UpdateUserViewModel updatedUser)
        {
            AppUser? user = await _userManager.FindByIdAsync(updatedUser.Id);
            
            if (user == null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View(updatedUser);
            }

            // Defensive: capture current password hash so we don't accidentally overwrite it
            var originalPasswordHash = user.PasswordHash;

            var existingUserWithEmail = await _userManager.FindByEmailAsync(updatedUser.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
            {
                ModelState.AddModelError(nameof(AppUser.Email), "This email is already in use by another user.");
                return View(updatedUser);
            }

            user.Email = updatedUser.Email;
            user.UserName = updatedUser.UserName;
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.IsAdmin = updatedUser.IsAdmin;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                // Use Identity's reset flow so password is validated and hashed correctly.
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, updatedUser.Password);
                if (!resetResult.Succeeded)
                {
                    foreach (var err in resetResult.Errors)
                        ModelState.AddModelError(string.Empty, err.Description);
                    return View(updatedUser);
                }
            }
            else
            {
                // Ensure the PasswordHash remains unchanged when no new password is provided.
                user.PasswordHash = originalPasswordHash;
            }

            IdentityResult result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Sync role membership for Admin flag
                if (updatedUser.IsAdmin)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));

                    if (!await _userManager.IsInRoleAsync(user, "Admin"))
                        await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                }

                return RedirectToAction("Index");
            }

            Errors(result);
            return View(updatedUser);
        }
        public async Task<IActionResult> Delete(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            AppUser? user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // If user is in Admin role, ensure we are not deleting the last admin
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    var admins = await _userManager.GetUsersInRoleAsync("Admin");
                    if (admins.Count <= 1)
                    {
                        ModelState.AddModelError("", "Cannot delete the last Admin user.");
                        return View(user);
                    }
                }

                // Remove from roles before deleting
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }

                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                Errors(result);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View("Index", _userManager.Users.ToList());
        }

        private void Errors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}