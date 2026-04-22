using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Identity.Controllers
{
    [Area(SD.IDENTITY_AREA)]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            // Automapper, mappster

            var result = user.Adapt<ApplicationUserVM>();

            //var result = new ApplicationUserVM()
            //{
            //    Address = user.Address,
            //    Email = user.Email,
            //    FirstName = user.FName,
            //    LastName = user.LName,
            //    PhoneNumber = user.PhoneNumber,
            //    Id = user.Id
            //};

            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var vm = currentUser.Adapt<ApplicationUserVM>();
                return View("Index", vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            user.Email = applicationUserVM.Email;
            user.FirstName = applicationUserVM.FirstName;
            user.LastName = applicationUserVM.LastName;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;
            await _userManager.UpdateAsync(user);

            TempData["success-notification"] = "Update Profile Successully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM updatePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var vm = currentUser.Adapt<ApplicationUserVM>();
                return View("Index", vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordVM.OldPassword, updatePasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                var userData = await _userManager.GetUserAsync(User);
                var vmData = userData.Adapt<ApplicationUserVM>();
                TempData["error-notification"] = "Password update failed";

                return View("Index", vmData);
            }

            TempData["success-notification"] = "Password updated successfully";
            return RedirectToAction("Index");
        }
    }

    
}