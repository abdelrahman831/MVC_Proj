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

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var usersVm = new List<UsersViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersVm.Add(new UsersViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FName = user.FName,
                    LName = user.LName,
                    Roles = roles.ToList() // Assicurati che sia una lista di stringhe
                });
            }

            return View(usersVm);
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
                var user = await _userManager.FindByIdAsync(id);


                var userVm = new UsersViewModel
                {
                    Email = user.Email,
                    FName = user.FName,
                    LName = user.LName,
                    Id = user.Id
                };


                return userVm == null ? NotFound() : View(userVm);
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
        public async Task<IActionResult> Edit(UsersViewModel userVm)
        {

            if (!ModelState.IsValid)
            {

                return View(userVm);
            }

            try
            {
                if (userVm.Id == null || userVm is null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
                    return View(userVm);
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(userVm.Id);

                    if (user is null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to update employee.");
                        return View(userVm);
                    }
                    else
                    {
                        user.FName = userVm.FName;
                        user.LName = userVm.LName;
                        user.UserName = userVm.Email;
                        var result = await _userManager.UpdateAsync(user);
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

                ModelState.AddModelError(string.Empty, "Failed to update employee.");
                return View(userVm);
            }
            catch (Exception ex)
            {

                return View("Error", "An error occurred while updating the employee.");
            }
        } 
        #endregion


        #region Details
        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            if (id is null)
                return BadRequest();

            var users = await _userManager.FindByIdAsync(id);
            var user = new UsersViewModel
            {
                Email = users.Email,
                FName = users.FName,
                LName = users.LName,
                Id = users.Id
            };
            return users == null ? NotFound() : View(user);
        }
        #endregion


        #region Delete GEt
        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id is null)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {

                var userVm = new UsersViewModel
                {
                    Email = user.Email,
                    FName = user.FName,
                    LName = user.LName,
                    Id = user.Id
                };
                return userVm == null ? NotFound() : View(userVm);


            }
            else
            {
                return View("Error", "An error occurred while deleting the employee.");
            }
            return View();
        } 
        #endregion


        #region Delete Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UsersViewModel userVm)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userVm.Id);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete employee.");
                    return View("Index");
                }
                else if (user is not null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        TempData["Message"] = "Employee deleted successfully!";
                        return RedirectToAction("Index");
                    }
                    return View(user);
                }

                ModelState.AddModelError(string.Empty, "Failed to delete employee.");
                return View("Index");
            }
            catch (Exception ex)
            {
                return View("Error", "An error occurred while deleting the employee.");
            }
        }
        #endregion


        #region SearchUsers

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchValue)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchValue))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Email.Contains(searchValue) ||
                    u.FName.Contains(searchValue) ||
                    u.LName.Contains(searchValue));
            }

            var users = await usersQuery.ToListAsync();
            var usersVm = new List<UsersViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersVm.Add(new UsersViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FName = user.FName,
                    LName = user.LName,
                    Roles = roles.ToList()
                });
            }
            return PartialView("~/Views/User/Partials/_UserTablePartial.cshtml", usersVm);

        }

        #endregion

    }

}
