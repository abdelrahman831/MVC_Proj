using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Employees;
using Demo.PL.ViewModels.Employee;

namespace Demo.PL.Mapping.Profiles
{
    public class ViemodelMappingProfiles : Profile
    {
        public ViemodelMappingProfiles()
        {

            CreateMap<EmployeeViewModel, EmployeeToCreateDto>();
            CreateMap<EmployeeViewModel, EmployeeToUpdateDto>();




            CreateMap<EmployeeToCreateDto, Employee>();

            CreateMap<EmployeeDetailsDto, EmployeeViewModel>();


        }


    }
}
