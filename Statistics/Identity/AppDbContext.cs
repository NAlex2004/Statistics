using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Identity
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(): base("Identity")
        {

        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }
    }
}