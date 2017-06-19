using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Statistics.Identity
{
    public class AppUser: IdentityUser
    {
        [Index(IsUnique = true)]
        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }
    }
}