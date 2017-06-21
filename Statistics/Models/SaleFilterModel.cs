using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public struct SaleFilterModel
    {
        public string Customer;
        public string Manager;
        public string Product;
        public DateTime? StartDate;
        public DateTime? EndDate;
    }
}