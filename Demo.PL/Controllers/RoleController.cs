using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Identity;
using Demo.PL.ViewModels.Employee;
using Demo.PL.ViewModels.Role;
using Demo.PL.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{

    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _userRole;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _userRole = roleManager;
            _userManager = userManager;
        }

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = _userRole.Roles.AsQueryable();
            var rolesVm = await roles.Select(u => new RoleViewModel
            {   
                Name = u.Name,
                Id = u.Id

            }).ToListAsync();




            return View(rolesVm);
        }
        #endregion

        #region Edit Get
        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id is null)
                return BadRequest();

            try
            {
                var role = await _userRole.FindByIdAsync(id);
                var users = await _userManager.Users.ToListAsync();



                return role == null ? NotFound() : View(new RoleViewModel
                {
                    Name = role.Name,
                    Id = role.Id,
                    users = users.Select(User => new UserRoleViewModel
                    {
                        UserName = User.UserName,
                        UserId = User.Id,
                        IsSelected = _userManager.IsInRoleAsync(User, role.Name).Result
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

        }
        #endregion

        #region Edit Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel roleVm)
        {

            if (!ModelState.IsValid)
            {

                return View(roleVm);
            }

            try
            {
                if (roleVm.Id == null || roleVm is null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update Role.");
                    return View(roleVm);
                }
                else
                {
                    var role = await _userRole.FindByIdAsync(roleVm.Id);

                    if (role is null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update role.");
                        return View(role);
                    }
                    else
                    {
                        role.Name = roleVm.Name;
                        var result = await _userRole.UpdateAsync(role);

                        foreach(var userRole in roleVm.users)
                        {
                            var user = await _userManager.FindByIdAsync(userRole.UserId);
                            if (userRole.IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                            {
                                await _userManager.AddToRoleAsync(user, role.Name);
                            }
                            else if (!userRole.IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                            {
                                await _userManager.RemoveFromRoleAsync(user, role.Name);
                            }
                        }


                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Failed to update roleVm.");
                return View(roleVm);
            }
            catch (Exception ex)
            {

                return View("Error", "An error occurred while updating the roleVm.");
            }
        }
        #endregion


        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            if (id is null)
                return BadRequest();

            var role = await _userRole.FindByIdAsync(id);
            var roleVm = new RoleViewModel
            {
                Name = role.Name,
                Id = role.Id
            };
            return roleVm == null ? NotFound() : View(roleVm);
        }
        #endregion


        #region Delete GEt
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id is null)
                return BadRequest();

            var role = await _userRole.FindByIdAsync(id);
            if (role is not null)
            {

                var roleVm = new RoleViewModel
                {
                    Name = role.Name,
                    Id = role.Id,
                };
                return roleVm == null ? NotFound() : View(roleVm);


            }
            return View("Error", "An error occurred while deleting the roleVm.");

        }
        #endregion


        #region Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleViewModel roleVm)
        {
            try
            {
                var role = await _userRole.FindByIdAsync(roleVm.Id);
                if (role is null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete role.");
                    return View("Index");
                }
                else if (role is not null)
                {
                    var result = await _userRole.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        TempData["Message"] = "role deleted successfully!";
                        return RedirectToAction("Index");
                    }
                    return View(role);
                }

                ModelState.AddModelError(string.Empty, "Failed to delete role.");
                return View("Index");
            }
            catch (Exception ex)
            {
                return View("Error", "An error occurred while deleting the role.");
            }
        }
        #endregion


        #region SearchRoles
        [HttpGet]
        public async Task<IActionResult> SearchRoles(string searchValue)
        {

            var role = _userRole.Roles.AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.ToLower();
                role = role
                    .Where(e => e.Name.ToLower().Contains(searchValue));
            }
            var usersVm = await role.Select(u => new RoleViewModel
            {
                Name = u.Name,
                Id = u.Id,
            }).ToListAsync();

            return PartialView("~/Views/Role/Partials/_RoleTablePartial.cshtml", usersVm);

        }
        #endregion


        #region CREATE

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (!ModelState.IsValid)
            {

                return View(roleViewModel);
            }

            try
            {
                var role = new IdentityRole
                {
                    Name = roleViewModel.Name,
                };
                var result = await _userRole.CreateAsync(role);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Role created successfully!";
                    return RedirectToAction("Index");
                }


                ModelState.AddModelError(string.Empty, "Failed to create role.");
                return View(roleViewModel);
            }
            catch (Exception ex)
            {

                return View("Error", "An error occurred while creating the roleViewModel.");
            }
        }
        #endregion
    }
}
