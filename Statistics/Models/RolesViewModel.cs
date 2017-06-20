using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class RolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IDictionary<string, bool> Roles { get; set; } 
        
        public string submit { get; set; }       
    }
}