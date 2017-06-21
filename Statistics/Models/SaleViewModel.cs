using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class SaleViewModel
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string Manager { get; set; }
        public string Customer { get; set; }                
        public string Product { get; set; }        
        public double Total { get; set; }

        public SaleFilterModel filter { get; set; }
    }
}