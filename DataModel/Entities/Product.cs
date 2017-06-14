using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NAlex.DataModel.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        public double Price { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
