using Demo.DAL.Entities.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public string Name { get; set; } = null!;
        public int? Age { get; set; }
        public string? Address { get; set; }
        public decimal Salary { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string? Email { get; set; }
        public DateOnly HiringDate { get; set; }
        public Gender Gender { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public int? DepartmentId { get; set; }


    }
}
