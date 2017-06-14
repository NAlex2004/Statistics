using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace NAlex.DataModel.Entities
{
    public class Manager
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LastName { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }
    }
}
