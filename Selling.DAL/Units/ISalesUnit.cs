using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAlex.Selling.DAL.Repositories;
using NAlex.Selling.DAL;
using NAlex.Selling.DTO.Classes;

namespace NAlex.Selling.DAL.Units
{
    public interface ISalesUnit: IDisposable
    {
        IRepository<CustomerDTO, int> Customers { get; }
        IRepository<ProductDTO, int> Products { get; }
        IRepository<ManagerDTO, int> Managers { get; }
        IRepository<SaleDTO, int> Sales { get; }
        IRepository<TempSaleDTO, int> TempSales { get; }

        SpResult CopyTempSalesToSales(Guid sessionId);
        void DeleteTempSales(Guid sessionId);
        int SaveChanges();
    }
}
