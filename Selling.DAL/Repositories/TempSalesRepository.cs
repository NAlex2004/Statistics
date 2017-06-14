using NAlex.DataModel.Entities;
using NAlex.Selling.DTO.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NAlex.Selling.DAL.Repositories
{
    public class TempSalesRepository : DtoRepository<TempSale, TempSaleDTO, int>
    {
        public TempSalesRepository(DbContext context) : base(context)
        {
        }
    }
}
