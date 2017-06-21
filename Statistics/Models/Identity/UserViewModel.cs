using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Statistics.Identity;
using Microsoft.AspNet.Identity;

namespace Statistics.Models
{
    public enum UserEditMode
    {
        Edit,
        Create
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "User name (login)")]
        public string UserName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(otherProperty: "Password", ErrorMessage = "Passwords are not equal")]
        [Display(Name = "Password confirmation")]
        public string PasswordConfirm { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public UserEditMode EditMode { get; set; }

        public string submit { get; set; }

        public UserViewModel()
        {
            submit = "Cancel";
            Roles = new List<string>();
        }
    }
}