using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAlex.DataModel.Entities;
using System.Data.Entity;
using NAlex.Selling.DTO.Classes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;

namespace NAlex.Selling.DAL.Repositories
{
    public class SalesRepository: DtoRepository<Sale, SaleDTO, int>
    {
        public SalesRepository(DbContext context): base(context)
        {
        }

        protected void FillSaleNavigationProperties(Sale sale, SaleDTO sourceDto, bool isUpdateAction = false)
        {
            if (!isUpdateAction) // при вставке вставляются детки
            {
                // если уже вставляли и не сохраняли
                if (sourceDto.Customer.Id == 0)
                {
                    var cust = _context.Set<Customer>().Local.FirstOrDefault(c => Mapper.Map<CustomerDTO>(c).Equals(sourceDto.Customer));
                    if (cust != null)
                        _context.Entry<Customer>(cust).State = EntityState.Detached;
                }

                if (sourceDto.Product.Id == 0)
                {
                    var prod = _context.Set<Product>().Local.FirstOrDefault(p => Mapper.Map<ProductDTO>(p).Equals(sourceDto.Product));
                    if (prod != null)
                        _context.Entry<Product>(prod).State = EntityState.Detached;
                }

                if (sourceDto.Manager.Id == 0)
                {
                    var man = _context.Set<Manager>().Local.FirstOrDefault(m => Mapper.Map<ManagerDTO>(m).Equals(sourceDto.Manager));
                    if (man != null)
                        _context.Entry<Manager>(man).State = EntityState.Detached;
                }
            }
            // ищем в базе           
            var customer = _context.Set<Customer>().FirstOrDefault(c => c.CustomerName == sourceDto.Customer.CustomerName);
            if (customer != null)
            {
                sale.CustomerId = customer.Id;
                sale.Customer = null;
            }

            var product = _context.Set<Product>().FirstOrDefault(p => p.ProductName == sourceDto.Product.ProductName);
            if (product != null)
            {
                sale.ProductId = product.Id;
                sale.Product = null;
            }

            var manager = _context.Set<Manager>().FirstOrDefault(m => m.LastName == sourceDto.Manager.LastName);
            if (manager != null)
            {
                sale.ManagerId = manager.Id;
                sale.Manager = null;
            }
        }

        public override SaleDTO Add(SaleDTO entity)
        {
            try
            {
                var sale = Mapper.Map<Sale>(entity);

                FillSaleNavigationProperties(sale, entity);

                var added = _context.Set<Sale>().Add(sale);
                
                return Mapper.Map<SaleDTO>(added);
            }
            catch
            {
                return null;
            }
            
        }

        public override bool Update(SaleDTO entity)
        {
            var sale = Mapper.Map<Sale>(entity);

            Sale existing = _context.Set<Sale>().Find(sale.Id);
            if (existing == null)
                return false;

            FillSaleNavigationProperties(sale, entity);

            if (sale.ProductId == 0)
                _context.Set<Product>().Add(sale.Product);
            if (sale.CustomerId == 0)
                _context.Set<Customer>().Add(sale.Customer);
            if (sale.ManagerId == 0)
                _context.Set<Manager>().Add(sale.Manager);

            _context.Entry<Sale>(existing).CurrentValues.SetValues(sale);
            _context.Entry<Sale>(existing).State = EntityState.Modified;

            return true;
        }
    }
}
