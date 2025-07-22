using Demo.DAL.Entities.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length should be less than 50 chars")]
        [MinLength(3, ErrorMessage = "Min Length should be less than 3 chars")]
        public string Name { get; set; } = null!;
        [Required]
        [Range(18, 60)]
        public int? Age { get; set; }
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$",
            ErrorMessage = "Invalid Address Format , You should like 123-street-city-country")]

        public string? Address { get; set; }
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }
        [EmailAddress]

        public string? Email { get; set; }
        [Display(Name = "Hiring Date")]

        public DateOnly HiringDate { get; set; }
        public Gender Gender { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public int? DepartmentId { get; set; }
        public int Id { get; set; }
        public string? ImagePath { get; set; }

        public IFormFile? Image { get; set; }



    }
}



