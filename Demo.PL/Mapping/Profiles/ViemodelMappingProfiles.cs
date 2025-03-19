using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Departments;
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

            //        CreateMap<EmployeeViewModel, EmployeeToUpdateDto>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));


            CreateMap<EmployeeToCreateDto, EmployeeViewModel>();

            CreateMap<EmployeeToCreateDto, Employee>();

            CreateMap<EmployeeDetailsDto, EmployeeViewModel>()
    .ForSourceMember(src => src.CreatedBy, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.CreatedOn, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.LastModifiedBy, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.LastModifiedOn, opt => opt.DoNotValidate());





        }




    }
}
