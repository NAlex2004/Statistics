using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NAlex.Selling.DAL;
using NAlex.Selling.DAL.Repositories;
using NAlex.Selling.DAL.Units;
using NAlex.Selling.DTO.Classes;
using Statistics.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;

namespace Statistics.BL
{
    public class SalesManager : ISalesManager
    {
        private ISalesUoW _unitOfWork;

        public SalesManager(ISalesUoW unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.CreateMap<SaleDTO, SaleViewModel>()
                    .ForMember(dst => dst.Customer, opt => opt.MapFrom(src => src.Customer.CustomerName))
                    .ForMember(dst => dst.Manager, opt => opt.MapFrom(src => src.Manager.LastName))
                    .ForMember(dst => dst.Product, opt => opt.MapFrom(src => src.Product.ProductName));

            Mapper.CreateMap<SaleViewModel, SaleDTO>()
                .ForMember(dst => dst.Customer, opt => opt.MapFrom(src => new CustomerDTO() { CustomerName = src.Customer, Id = 0 }))
                .ForMember(dst => dst.Manager, opt => opt.MapFrom(src => new ManagerDTO() { LastName = src.Manager, Id = 0 }))
                .ForMember(dst => dst.Product, opt => opt.MapFrom(src => new ProductDTO() { ProductName = src.Product, Id = 0, Price = 0 }));
        }               

        public SaleViewModel GetSale(int id)
        {
            var sale = _unitOfWork.Sales.Get(id);
            SaleViewModel model = null;
            if (sale != null)
                model = ViewModelFromDTO(sale);
            return model;
        }

        public IEnumerable<SaleViewModel> GetSales(SaleFilterModel filter, Func<IQueryable<SaleDTO>, IOrderedQueryable<SaleDTO>> orderBy = null, PagerData pager = null)
        {            
            var exp = PredicateBuilder.True<SaleDTO>();
            if (!string.IsNullOrEmpty(filter.Customer))
            {
                string customer = filter.Customer.ToLower();
                exp = exp.And(s => s.Customer.CustomerName.ToLower().Contains(customer));
            }
            if (!string.IsNullOrEmpty(filter.Manager))
            {
                string manager = filter.Manager.ToLower();
                exp = exp.And(s => s.Manager.LastName.ToLower().Contains(manager));
            }
            if (!string.IsNullOrEmpty(filter.Product))
            {
                string product = filter.Product.ToLower();
                exp = exp.And(s => s.Product.ProductName.ToLower().Contains(product));
            }
            if (filter.StartDate.HasValue)
            {
                DateTime startDate = filter.StartDate.Value;
                exp = exp.And(s => s.SaleDate >= startDate);
            }
            if (filter.EndDate.HasValue)
            {
                DateTime endDate = filter.EndDate.Value;
                exp = exp.And(s => s.SaleDate <= endDate);
            }

            if (orderBy == null)
                orderBy = s => s.OrderBy(o => o.SaleDate);
           
            var sales = _unitOfWork.Sales.GetAsQueryable(exp, orderBy);

            if (sales != null)
            {
                if (pager != null)
                {
                    int total = sales.Count();
                    int rest = total % pager.ItemsPerPage;
                    pager.TotalPages = total / pager.ItemsPerPage + (rest > 0 ? 1 : 0);
                    int skip = pager.ItemsPerPage * (pager.CurrentPage - 1);
                    sales = sales.Skip(skip).Take(pager.ItemsPerPage);
                }

                return sales.ToArray().Select(s => ViewModelFromDTO(s));
            }

            return new SaleViewModel[0];
        }

        public SaleViewModel ViewModelFromDTO(SaleDTO dto)
        {
            return Mapper.Map<SaleViewModel>(dto);
        }

        private void FillSaleDTOFromModel(SaleDTO sale, SaleViewModel saleModel)
        {
            sale.Manager = new ManagerDTO() { LastName = saleModel.Manager };
            sale.Customer = new CustomerDTO() { CustomerName = saleModel.Customer };
            sale.Product = new ProductDTO() { ProductName = saleModel.Product };
            sale.SaleDate = saleModel.SaleDate;
            sale.Total = saleModel.Total;
        }

        public bool UpdateSale(SaleViewModel saleModel)
        {
            if (saleModel == null)
                return false;

            var sale = _unitOfWork.Sales.Get(saleModel.Id);
            if (sale != null)
            {
                FillSaleDTOFromModel(sale, saleModel);

                if (_unitOfWork.Sales.Update(sale))
                {             
                    try
                    {
                        _unitOfWork.SaveChanges();
                        return true;
                    }   
                    catch (Exception e)
                    {

                    }                        
                }                
            }

            return false;
        }

        public bool CreateSale(SaleViewModel saleModel)
        {
            if (saleModel == null)
                return false;

            SaleDTO sale = Mapper.Map<SaleDTO>(saleModel);

            SaleDTO added = _unitOfWork.Sales.Add(sale);

            if (added != null)
            {
                try
                {
                    _unitOfWork.SaveChanges();
                    var saved = _unitOfWork.Sales.Get(s => s.Customer.Equals(added.Customer)
                        && s.Product.Equals(added.Product) && s.Manager.Equals(added.Manager)
                        && s.Total.Equals(added.Total) && s.SaleDate.Equals(added.SaleDate)).FirstOrDefault();
                    if (saved != null)
                        saleModel.Id = saved.Id;
                    return true;
                }
                catch (Exception e)
                {

                }                
            }

            return false;
        }

        public bool DeleteSale(int id)
        {
            try
            {
                _unitOfWork.Sales.Remove(id);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_unitOfWork != null)
                        _unitOfWork.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {            
            Dispose(true);         
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}