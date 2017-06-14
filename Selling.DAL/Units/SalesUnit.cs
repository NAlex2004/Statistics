using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using NAlex.Selling.DAL.Repositories;
using NAlex.Selling.DTO.Classes;
using System.Data.SqlClient;
using NAlex.DataModel.Entities;

namespace NAlex.Selling.DAL.Units
{
    public class SalesUnit: ISalesUnit
    {
        private bool _disposed = false;
        private DbContext _context;

        IRepository<CustomerDTO, int> _customers;
        IRepository<ProductDTO, int> _products;
        IRepository<ManagerDTO, int> _managers;
        IRepository<SaleDTO, int> _sales;
        IRepository<TempSaleDTO, int> _tempSales;

        public SalesUnit()
        {
            _context = new SalesContext();

            _customers = new CustomersRepository(_context);
            _products = new ProductsRepository(_context);
            _managers = new ManagersRepository(_context);
            _sales = new SalesRepository(_context);
            _tempSales = new TempSalesRepository(_context);
        }

        public IRepository<CustomerDTO, int> Customers
        {
            get { return _customers; }
        }

        public IRepository<ProductDTO, int> Products
        {
            get { return _products; }
        }

        public IRepository<ManagerDTO, int> Managers
        {
            get { return _managers; }
        }

        public IRepository<SaleDTO, int> Sales
        {
            get { return _sales; }
        }

        public IRepository<TempSaleDTO, int> TempSales
        {
            get
            {
                return _tempSales;
            }
        }

        public SpResult CopyTempSalesToSales(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                return new SpResult() { ErrorNumber = -1, ErrorMessage = "sessionId cannot be empty." };

            SqlParameter sessionIdParam = new SqlParameter("@SessionId", System.Data.SqlDbType.UniqueIdentifier);
            sessionIdParam.Value = sessionId;
            SpResult res = _context.Database.SqlQuery<SpResult>("exec Sales.dbo.CopyTempSales @SessionId", sessionIdParam).FirstOrDefault();
            return res;
        }

        public int SaveChanges()
        {            
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {                
                _context.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Delete from database
        /// </summary>        
        public void DeleteTempSales(Guid sessionId)
        {            
            SqlParameter sessionParameter = new SqlParameter("@SessionId", System.Data.SqlDbType.UniqueIdentifier);
            sessionParameter.Value = sessionId;
            _context.Database.ExecuteSqlCommand("delete from TempSales where SessionId = @SessionId", sessionParameter);
        }
    }
}
