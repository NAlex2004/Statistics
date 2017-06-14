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

        public override SaleDTO Add(SaleDTO entity)
        {
            try
            {
                var sale = Mapper.Map<Sale>(entity);

                // если уже вставляли и не сохраняли
                if (entity.Customer.Id == 0)
                {
                    var cust = _context.Set<Customer>().Local.FirstOrDefault(c => Mapper.Map<CustomerDTO>(c).Equals(entity.Customer));
                    if (cust != null)
                        _context.Entry<Customer>(cust).State = EntityState.Detached;
                }                                                    

                if (entity.Product.Id == 0)
                {
                    var prod = _context.Set<Product>().Local.FirstOrDefault(p => Mapper.Map<ProductDTO>(p).Equals(entity.Product));
                    if (prod != null)
                        _context.Entry<Product>(prod).State = EntityState.Detached;
                }
                
                if (entity.Manager.Id == 0)
                {
                    var man = _context.Set<Manager>().Local.FirstOrDefault(m => Mapper.Map<ManagerDTO>(m).Equals(entity.Manager));
                    if (man != null)
                        _context.Entry<Manager>(man).State = EntityState.Detached;
                }                

                // ищем в базе           
                var customer = _context.Set<Customer>().FirstOrDefault(c => c.CustomerName == entity.Customer.CustomerName);
                if (customer != null)
                {
                    sale.CustomerId = customer.Id;
                    sale.Customer = null;
                }

                var product = _context.Set<Product>().FirstOrDefault(p => p.ProductName == entity.Product.ProductName);
                if (product != null)
                {
                    sale.ProductId = product.Id;
                    sale.Product = null;
                }

                var manager = _context.Set<Manager>().FirstOrDefault(m => m.LastName == entity.Manager.LastName);
                if (manager != null)
                {
                    sale.ManagerId = manager.Id;
                    sale.Manager = null;
                }

                var added = _context.Set<Sale>().Add(sale);
                
                return Mapper.Map<SaleDTO>(added);
            }
            catch
            {
                return null;
            }
            
        }
    }
}
