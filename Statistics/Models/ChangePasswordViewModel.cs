using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password confirmation")]
        [Compare(otherProperty: "Password", ErrorMessage = "Passwords are not equal.")]
        public string PasswordConfirm { get; set; }
        
        public string ReturnUrl { get; set; }        
    }
}