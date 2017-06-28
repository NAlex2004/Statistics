using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class SalesListModel
    {
        public IEnumerable<SaleViewModel> Sales { get; set; }
        public SaleFilterModel Filter { get; set; }
        public PagerData Pager { get; set; }
    }
}