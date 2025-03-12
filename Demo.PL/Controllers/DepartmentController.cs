using AutoMapper;
using Demo.BLL.DTOS.Departments;
using Demo.BLL.Services.Departments;
using Demo.PL.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        //Action ==> Master Action

        //ViewStorage ==> ViewData , ViewBag ==>Deal with the same storage
        //Dictionary
        //Extra data

        // 1] Send data from action in controller to view
        // 2] Send data from view to partial view
        // 3] Send data from view to layout

        //View data ==> .net 3.5
        //ViewBag ==> .net 4.0
        //Tempdata ==> Send data from request to another request ==> From Action to another Action

        #region Service //DependancyInjection 
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        public DepartmentController(IDepartmentService departmentService, ILogger<DepartmentController> logger, IWebHostEnvironment environment,IMapper mapper)
        {
            _departmentService = departmentService;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
        }
        #endregion

        #region Index
     
        
        [HttpGet] //As Default
        public IActionResult Index()
        {
        //ViewData["Message"] = "Hello in Departments From View Data!";
        //ViewBag.Message= "Hello in Departments From View Bag!";

            var departments = _departmentService.GetAllDepartments();
            return View(departments);
        } 
        #endregion

        #region Create
        [HttpGet]
        //show the form
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public IActionResult Create(DepartmentViewModel departmentVM)  //new
        {
            if (!ModelState.IsValid)

                return View(departmentVM);
            var message = string.Empty;
            try
            {
                var Result = _departmentService.CreateDepartment(_mapper.Map<DepartmentToCreateDto>(departmentVM) ); //new
                //{
                //    Code = departmentVM.Code,
                //    CreationDate = departmentVM.CreationDate,
                //    Description = departmentVM.Description,
                //    Name = departmentVM.Name

                //});
                if (Result > 0) 
                {
                    TempData["Message"] = "Congratolations! , Department is created";
                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Department is not created";
                    TempData["Message"] = message;

                    ModelState.AddModelError(string.Empty, message);
                    return View(departmentVM);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View(departmentVM);
                }
                else
                {
                    message = "An error occurred while creating the department";
                    return View("Error", message);


                }

            }
        }
        #endregion

        #region Details
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var department = _departmentService.GetDepartmentsById(id.Value);
            if (department is null)
            {
                return NotFound();
            }
            return View(department);

        }
        #endregion

        #region Update
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var department = _departmentService.GetDepartmentsById(id.Value);
            if (department is null)
            {
                return NotFound();
            }
            return View(_mapper.Map<DepartmentViewModel>(department));
            //{
            //    Code = department.Code,
            //    CreationDate = department.CreationDate,
            //    Description = department.Description,
            //    Name = department.Name


            //});
        }
        

        
        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public IActionResult Edit(DepartmentViewModel departmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(departmentViewModel);
            }
            var message = string.Empty;
            try
            {
                var Result = _departmentService.UpdateDepartment(_mapper.Map<DepartmentToUpdateDto>(departmentViewModel));
                //{
                //    Id = id,
                //    Code = departmentViewModel.Code,
                //    CreationDate = departmentViewModel.CreationDate,
                //    Description = departmentViewModel.Description,
                //    Name = departmentViewModel.Name
                //});
                if (Result > 0)
                {
                    TempData["Message"] = "Congratolations! , Department is Updated";

                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Department is not updated Ya Man !";
                    TempData["Message"]= message;
                    ModelState.AddModelError(string.Empty, message);
                    return View(departmentViewModel);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View(departmentViewModel);
                }
                else
                {
                    message = "An error occurred while updating the department";
                    return View("Error", message);

                }

            }

        }
        #endregion

        #region Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id is null)
                return BadRequest();
            var department = _departmentService.GetDepartmentsById(id.Value);
            if (department is null)
            {
                return NotFound();
            }
            return View(department);



        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Action Filter

        public IActionResult Delete(int id)
        {
            var message = string.Empty;
            try
            {
                var Result = _departmentService.DeleteDepartment(id);

                if (Result == true)
                {
                    TempData["Message"] = "Sure! , Department is Removed";
                    return RedirectToAction("Index");
                }
                else
                {
                    message = "Department is not deleted Ya Man!";
                    ModelState.AddModelError(string.Empty, message);
                    return View();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (_environment.IsDevelopment())
                {
                    message = ex.Message;
                    return View();
                }
                else
                {
                    message = "An error occurred while deleting the department";
                    return View("Error", message);

                }

            }

        } 
        #endregion

    }
}
