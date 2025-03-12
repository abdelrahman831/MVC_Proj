using AutoMapper;
using Demo.BLL.DTOS.Departments;
using Demo.DAL.Entities.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Mapping.Profiles.Departments
{
    public class DepartmentServiceMapping : Profile
    {
        public DepartmentServiceMapping()
        {
            CreateMap<DepartmentToCreateDto, Department>();

            CreateMap<DepartmentDetailsToReturnDto, Department>();
            CreateMap<Department, DepartmentDetailsToReturnDto>();

            CreateMap<DepartmentToUpdateDto, Department>();
            CreateMap<Department, DepartmentToReturnDto>();
        }
    }
}
