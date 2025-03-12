using AutoMapper;
using Demo.BLL.DTOS.Departments;
using Demo.DAL.Entities.Departments;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Departments;
using Demo.DAL.Presistance.UnitOfWork;
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


        public IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region Index
        public IEnumerable<DepartmentToReturnDto> GetAllDepartments()
        {

            var departments = _unitOfWork.DepartmentRepository.GetAllQuarable().Where(D => !D.IsDeleted).Select(department => _mapper.Map<DepartmentToReturnDto>(department)).AsNoTracking().ToList();
            return departments;
        } 
        #endregion

        #region Details
        public DepartmentDetailsToReturnDto? GetDepartmentsById(int id)
        {
            var department = _unitOfWork.DepartmentRepository.GetById(id);
            if (department is not null)
            {
                return _mapper.Map<DepartmentDetailsToReturnDto>(department);
                //{
                //    Id = department.Id,
                //    Name = department.Name,
                //    Code = department.Code,
                //    CreatedBy = department.CreatedBy,
                //    CreatedOn = department.CreatedOn,
                //    CreationDate = department.CreationDate,
                //    LastModifiedBy = department.LastModifiedBy,
                //    LastModifiedOn = department.LastModifiedOn,
                //    Description = department.Description,
                //    IsDeleted = department.IsDeleted
                //};

            }
            return null;
        } 
        #endregion

        #region Create
        public int CreateDepartment(DepartmentToCreateDto department)
        {
            var departmentCreated = _mapper.Map<Department>(department);

             _unitOfWork.DepartmentRepository.AddT(departmentCreated);
            return _unitOfWork.Complete();

        }
        #endregion

        #region Update
        public int UpdateDepartment(DepartmentToUpdateDto department)
        {
            var departmentUpdated = _mapper.Map<Department>(department);
 
            _unitOfWork.DepartmentRepository.UpdateT(departmentUpdated);  //Rows Affected 
            return _unitOfWork.Complete();

        }
        #endregion

        #region Delete
        public bool DeleteDepartment(int id)
        {
            var department = _unitOfWork.DepartmentRepository.GetById(id);
            if (department is not null)
            {
                //int RowsAffected = _departmentRepository.DeleteDepartment(department);
                //return RowsAffected > 0;
                _unitOfWork.DepartmentRepository.DeleteT(department);  //bool
                return _unitOfWork.Complete()>0;

            }
            return false;
        } 
        #endregion
    }
}