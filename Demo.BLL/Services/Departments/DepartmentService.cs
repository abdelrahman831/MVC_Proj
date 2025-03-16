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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region Index
        public async Task<IEnumerable<DepartmentToReturnDto>> GetAllDepartmentsAsync()
        {

            var departments = await _unitOfWork.DepartmentRepository.GetAllQuarableAsync().Where(D => !D.IsDeleted).Select(department => _mapper.Map<DepartmentToReturnDto>(department)).AsNoTracking().ToListAsync();
            return departments;
        } 
        #endregion

        #region Details
        public async Task<DepartmentDetailsToReturnDto?> GetDepartmentsByIdAsync(int id)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
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
        public async Task<int> CreateDepartmentAsync(DepartmentToCreateDto department)
        {
            var departmentCreated = _mapper.Map<Department>(department);

              _unitOfWork.DepartmentRepository.AddTAsync(departmentCreated);
            return await _unitOfWork.CompleteAsync();

        }
        #endregion

        #region Update
        public async Task<int> UpdateDepartmentAsync(DepartmentToUpdateDto department)
        {
            var departmentUpdated = _mapper.Map<Department>(department);
 
             _unitOfWork.DepartmentRepository.UpdateTAsync(departmentUpdated);  //Rows Affected 
            return await _unitOfWork.CompleteAsync();

        }
        #endregion

        #region Delete
        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department =  await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
            if (department is not null)
            {
                //int RowsAffected = _departmentRepository.DeleteDepartment(department);
                //return RowsAffected > 0;
                 _unitOfWork.DepartmentRepository.DeleteTAsync(department);  //bool
                return await _unitOfWork.CompleteAsync()>0;

            }
            return false;
        } 
        #endregion
    }
}