using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Statistics.Identity;
using Microsoft.AspNet.Identity;

namespace Statistics.Models
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        public IDictionary<IRole, bool> Roles { get; set; }
    }
}