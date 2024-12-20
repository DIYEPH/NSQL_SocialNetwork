﻿using System.ComponentModel.DataAnnotations;

namespace NoSQLSocialNetwork.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        public string? Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string? Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
