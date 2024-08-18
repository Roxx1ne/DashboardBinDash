using CRUD.Models;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin, Staff")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserVM>();

            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserVM
                {
                    Id = user.Id,
                    UserName = user.Name,
                    Email = user.Email,
                    Role = role.FirstOrDefault() ?? "No Role"
                });
            }

            return View(userRoles);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRole = await _userManager.GetRolesAsync(user);
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.Name,
                Email = user.Email,
                Role = userRole.FirstOrDefault(),
                Roles = roles
            };

            return View(model);
        }


        // POST: User/Edit/5
        // POST: User/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;

                var userRole = await _userManager.GetRolesAsync(user);
                var currentRole = userRole.FirstOrDefault();

                if (currentRole != null && currentRole != model.Role)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                if (currentRole != model.Role)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index), new { selectedRole = model.Role }); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Menyusun model untuk konfirmasi penghapusan
            var model = new DeleteUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email

            };

            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Delete", new DeleteUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

    }
}
