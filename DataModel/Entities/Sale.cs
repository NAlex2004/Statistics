using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NAlex.DataModel.Entities
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        public double Total { get; set; }

        [ForeignKey("Manager")]
        public int ManagerId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Manager Manager { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }        
    }
}
