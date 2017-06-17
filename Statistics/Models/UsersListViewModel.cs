using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class UsersListViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public PagerData Pager { get; set; }
    }
}