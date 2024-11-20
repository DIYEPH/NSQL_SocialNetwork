using System.ComponentModel.DataAnnotations;

namespace NoSQLSocialNetwork.ViewModels
{
    public class SignupVM
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string? Email { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string? Fullname { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string? Password { get; set; }


    }
}
