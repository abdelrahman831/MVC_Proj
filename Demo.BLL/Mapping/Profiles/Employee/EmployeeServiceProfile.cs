using AutoMapper;
using Demo.BLL.DTOS.Employees;
using Demo.DAL.Entities.Departments;
using Demo.DAL.Entities.Employees;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Mapping.Profiles.Employees
{


    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeToCreateDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => 0)) // Imposta un valore predefinito
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastModifiedBy, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.LastModifiedOn, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => (Department?)null)); // Assicura che sia nullo

            CreateMap<Employee, EmployeeDetailsDto>();
            CreateMap<EmployeeToUpdateDto, Employee>();
            CreateMap<Employee, EmployeeToReturnDto>();
        }
    }

}
