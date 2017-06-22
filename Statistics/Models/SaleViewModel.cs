using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class SaleViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Sale date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime SaleDate { get; set; }
        [Required]
        public string Manager { get; set; }
        [Required]
        public string Customer { get; set; }                
        [Required]
        public string Product { get; set; }        
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N}")]        
        public double Total { get; set; }

        public string submit { get; set; }

    }
}