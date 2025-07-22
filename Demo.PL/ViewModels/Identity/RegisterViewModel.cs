using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels.Identity
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage="First Name is Required")]
        public string FName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]

        public string LName { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]

        public string Email { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Passwords should match")]
        public string ConfirmPassword { get; set; }

        public bool IsAgree { get; set; }


    }
}
