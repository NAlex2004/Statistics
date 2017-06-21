using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Statistics.Identity
{
    public class AppUserValidator : UserValidator<AppUser>
    {
        private AppUserManager _manager;
        public AppUserValidator(AppUserManager manager) : base(manager)
        {
            _manager = manager;
        }

        public override async Task<IdentityResult> ValidateAsync(AppUser item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            var existing = _manager.Users.Where(u => u.LastName.Equals(item.LastName)).FirstOrDefault();
            if (existing != null && !item.Id.Equals(existing.Id))
            {
                var errors = result.Errors.ToList();
                errors.Add(string.Format("User with Last Name '{0}' already exists.", item.LastName));
                result = new IdentityResult(errors);
            }

            return result;
        }
    }
}