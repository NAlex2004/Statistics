using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace NAlex.DataModel.Entities
{
    public class SalesContext: DbContext
    {
        public SalesContext() : base("Sales")
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<TempSale> TempSales { get; set; }
        
    }
}
