using System.ComponentModel.DataAnnotations;

namespace NewsManagementSystem.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }
        [Required]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }
        [Required]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
