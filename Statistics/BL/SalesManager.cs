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

        public IEnumerable<SaleViewModel> GetSales(SaleFilterModel filter, Func<IEnumerable<SaleDTO>, IOrderedEnumerable<SaleDTO>> orderBy = null)
        {
            var exp = PredicateBuilder.True<SaleDTO>();
            if (!string.IsNullOrEmpty(filter.Customer))
                exp = exp.And(s => s.Customer.CustomerName.ToLower().Contains(filter.Customer.ToLower()));
            if (!string.IsNullOrEmpty(filter.Manager))
                exp = exp.And(s => s.Manager.LastName.ToLower().Contains(filter.Manager.ToLower()));
            if (!string.IsNullOrEmpty(filter.Product))
                exp = exp.And(s => s.Product.ProductName.ToLower().Contains(filter.Product.ToLower()));
            if (filter.StartDate.HasValue)
                exp = exp.And(s => s.SaleDate >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                exp = exp.And(s => s.SaleDate <= filter.EndDate.Value);
            var sales = _unitOfWork.Sales.Get(exp.Compile(), orderBy);

            if (sales != null)
                return sales.Select(s => ViewModelFromDTO(s));

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
                    saleModel.Id = added.Id;
                    return true;
                }
                catch { }                
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