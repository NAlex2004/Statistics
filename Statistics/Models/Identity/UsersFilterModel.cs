using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class UsersFilterModel
    {
        public int? Page { get; set; }
        public string UserName { get; set; }
    }
}