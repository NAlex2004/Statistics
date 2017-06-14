using System;
using System.ComponentModel.DataAnnotations;

namespace NAlex.DataModel.Entities
{
    public class TempSale
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        [Required]
        public double Total { get; set; }
        [Required]
        public string ManagerName { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string ProductName { get; set; }        
    }
}
