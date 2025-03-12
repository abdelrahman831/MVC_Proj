using Demo.BLL.DTOS.Departments;
using Demo.DAL.Entities.Departments;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Departments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Services.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        #region Index
        public IEnumerable<DepartmentToReturnDto> GetAllDepartments()
        {
            //var departments = _departmentRepository.GetAll(); //IEnumerable of Department ==>IEnumerable of DepartmentDto

            //foreach (var department in departments)
            //{
            //    yield return new DepartmentToReturnDto()
            //    {
            //        Description = department.Description,
            //        CreationDate = department.CreationDate,
            //        Code = department.Code,
            //        Id = department.Id,
            //        Name = department.Name

            //    };
            //}
            var departments = _departmentRepository.GetAllQuarable().Where(D => !D.IsDeleted).Select(department => new DepartmentToReturnDto
            {
                //Description = department.Description,
                CreationDate = department.CreationDate,
                Code = department.Code,
                Id = department.Id,
                Name = department.Name
            }).AsNoTracking().ToList();
            return departments;
        } 
        #endregion

        #region Details
        public DepartmentDetailsToReturnDto? GetDepartmentsById(int id)
        {
            var department = _departmentRepository.GetById(id);
            if (department is not null)
            {
                return new DepartmentDetailsToReturnDto()
                {
                    Id = department.Id,
                    Name = department.Name,
                    Code = department.Code,
                    CreatedBy = department.CreatedBy,
                    CreatedOn = department.CreatedOn,
                    CreationDate = department.CreationDate,
                    LastModifiedBy = department.LastModifiedBy,
                    LastModifiedOn = department.LastModifiedOn,
                    Description = department.Description,
                    IsDeleted = department.IsDeleted
                };

            }
            return null;
        } 
        #endregion

        #region Create
        public int CreateDepartment(DepartmentToCreateDto department)
        {
            var departmentCreated = new Department()
            {
                Code = department.Code,
                Description = department.Description,
                Name = department.Name,
                CreationDate = department.CreationDate,
                LastModifiedBy = 1, //UserId
                CreatedBy = 1,
                LastModifiedOn = DateTime.UtcNow

            };
            return _departmentRepository.AddT(departmentCreated);
        }
        #endregion

        #region Update
        public int UpdateDepartment(DepartmentToUpdateDto department)
        {
            var departmentUpdated = new Department()
            {
                Id = department.Id,
                Code = department.Code,
                Description = department.Description,
                Name = department.Name,
                CreationDate = department.CreationDate,
                LastModifiedBy = 1, //UserId
                CreatedBy = 1,
                LastModifiedOn = DateTime.UtcNow

            };
            return _departmentRepository.UpdateT(departmentUpdated);  //Rows Affected 
        } 
        #endregion

        #region Delete
        public bool DeleteDepartment(int id)
        {
            var department = _departmentRepository.GetById(id);
            if (department is not null)
            {
                //int RowsAffected = _departmentRepository.DeleteDepartment(department);
                //return RowsAffected > 0;
                return _departmentRepository.DeleteT(department) > 0;  //bool
            }
            return false;
        } 
        #endregion
    }
}