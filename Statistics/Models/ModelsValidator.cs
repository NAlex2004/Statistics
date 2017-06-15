using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public static class ModelsValidator
    {
        public static bool IsValid(ChangePasswordViewModel model)
        {
            return model != null
                ? !string.IsNullOrEmpty(model.UserName) 
                    && !string.IsNullOrEmpty(model.OldPassword)
                    && !string.IsNullOrEmpty(model.Password)
                    && model.OldPassword.Equals(model.PasswordConfirm)
                : false;
        }

        public static bool IsValid(UserViewModel model)
        {
            return model != null
                ? !string.IsNullOrEmpty(model.UserName)
                    && !string.IsNullOrEmpty(model.LastName)
                    && !string.IsNullOrEmpty(model.Password)
                    && model.Password.Equals(model.PasswordConfirm)
                : false;
        }
    }
}