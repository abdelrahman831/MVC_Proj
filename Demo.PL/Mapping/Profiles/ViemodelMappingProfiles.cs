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




            CreateMap<EmployeeToCreateDto, Employee>();

            CreateMap<EmployeeDetailsDto, EmployeeViewModel>()
    .ForSourceMember(src => src.CreatedBy, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.CreatedOn, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.LastModifiedBy, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.LastModifiedOn, opt => opt.DoNotValidate())
    .ForSourceMember(src => src.IsDeleted, opt => opt.DoNotValidate())

                .ForMember(dest => dest.Img, opt => opt.Ignore());




        }

        private string ConvertFileToBase64(IFormFile file)
{
    using (var memoryStream = new MemoryStream())
    {
        file.CopyTo(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();
        return Convert.ToBase64String(fileBytes);
    }
}


    }
}
