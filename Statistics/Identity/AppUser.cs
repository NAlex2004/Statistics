using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Statistics.Identity
{
    public class AppUser: IdentityUser
    {
        [Index(IsUnique = true)]
        public string LastName { get; set; }
    }
}