using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using NAlex.DataModel.Entities;
using NAlex.Selling.DTO.Classes;

namespace NAlex.Selling.DAL.Repositories
{
    public class CustomersRepository : DtoRepository<Customer, CustomerDTO, int>
    {
        public CustomersRepository(DbContext context): base(context)
        {
        }
    }
}
