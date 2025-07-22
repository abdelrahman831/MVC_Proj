using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels.Department
{
    public class DepartmentViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Max Length should be less than 50 chars")]

        public string Name { get; set; } = null!;
        [MaxLength(100, ErrorMessage = "Max Length should be less than 100 chars")]
        public string? Description { get; set; }
        [Required]
        //max length 10
        [MaxLength(20, ErrorMessage = "Max Length should be less than 20 chars")]

        public string Code { get; set; } = null!;
        public DateOnly CreationDate { get; set; }
        public int Id { get; set; }
    }
}
