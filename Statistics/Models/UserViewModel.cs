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
        public string Id { get; set; }
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
        [Compare(otherProperty: "Password", ErrorMessage = "Passwords are not equal")]
        public string PasswordConfirm { get; set; }
        public IEnumerable<KeyValuePair<string, bool>> Roles { get; set; }
    }
}