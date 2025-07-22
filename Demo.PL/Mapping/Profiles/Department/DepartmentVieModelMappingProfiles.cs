using AutoMapper;
using Demo.BLL.DTOS.Departments;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Departments;
using Demo.PL.ViewModels.Department;
using Demo.PL.ViewModels.Employee;

namespace Demo.PL.Mapping.Profiles.Departments
{
    public class DepartmentVieModelMappingProfiles : Profile
    {
        public DepartmentVieModelMappingProfiles()
        {


            CreateMap<DepartmentViewModel, DepartmentToCreateDto>();
            CreateMap<DepartmentViewModel, DepartmentToUpdateDto>();
            CreateMap<DepartmentToCreateDto, Department>();
            CreateMap<DepartmentDetailsToReturnDto,DepartmentViewModel >();

        }
    }
}



