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

namespace Statistics.BL
{
    public class SalesManager: IDisposable
    {
        private ISalesUoW _unitOfWork;

        public SalesManager(ISalesUoW unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<SaleDTO, SaleViewModel>()
                    .ForMember<string>(dst => dst.Customer, opt => opt.MapFrom(src => src.Customer.CustomerName))
                    .ForMember<string>(dst => dst.Manager, opt => opt.MapFrom(src => src.Manager.LastName))
                    .ForMember<string>(dst => dst.Product, opt => opt.MapFrom(src => src.Product.ProductName));

                cfg.CreateMap<SaleViewModel, SaleDTO>()
                    .ForMember<string>(dst => dst.Customer.CustomerName, opt => opt.MapFrom(src => src.Customer))
                    .ForMember<string>(dst => dst.Manager.LastName, opt => opt.MapFrom(src => src.Manager))
                    .ForMember<string>(dst => dst.Product.ProductName, opt => opt.MapFrom(src => src.Product));                
            });
        }

        public IEnumerable<SaleViewModel> GetSales(Func<SaleViewModel, bool> condition,
            Func<IEnumerable<SaleViewModel>, IOrderedEnumerable<SaleViewModel>> orderBy = null)
        {
            Func<SaleDTO, bool> dtoCondition = Mapper.Map<Func<SaleDTO, bool>>(condition);
            Func<IEnumerable<SaleDTO>, IOrderedEnumerable<SaleDTO>> dtoOrder =
                Mapper.Map<Func<IEnumerable<SaleDTO>, IOrderedEnumerable<SaleDTO>>>(orderBy);

            var sales = _unitOfWork.Sales.Get(dtoCondition, dtoOrder);

            return Mapper.Map<IEnumerable<SaleViewModel>>(sales);
        }

        public IEnumerable<SaleViewModel> GetSales(SaleFilterModel filter, Func<IEnumerable<SaleViewModel>, IOrderedEnumerable<SaleViewModel>> orderBy = null)
        {
            Func<SaleViewModel, bool> condition = m => !string.IsNullOrEmpty(filter.Product) ? m.Product.Contains(filter.Product) : true
                && !string.IsNullOrEmpty(filter.Manager) ? m.Manager.Contains(filter.Manager) : true
                && !string.IsNullOrEmpty(filter.Customer) ? m.Customer.Contains(filter.Customer) : true
                && filter.StartDate.HasValue ? m.SaleDate >= filter.StartDate.Value : true
                && filter.EndDate.HasValue ? m.SaleDate <= filter.EndDate : true;

            return GetSales(condition, orderBy);
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
                    catch
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