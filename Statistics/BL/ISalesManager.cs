using System;
using System.Collections.Generic;
using System.Linq;
using NAlex.Selling.DTO.Classes;
using Statistics.Models;

namespace Statistics.BL
{
    public interface ISalesManager: IDisposable
    {
        bool CreateSale(SaleViewModel saleModel);
        bool DeleteSale(int id);
        SaleViewModel GetSale(int id);
        IEnumerable<SaleViewModel> GetSales(SaleFilterModel filter, Func<IQueryable<SaleDTO>, IOrderedQueryable<SaleDTO>> orderBy = null, PagerData pager = null);
        bool UpdateSale(SaleViewModel saleModel);
        SaleViewModel ViewModelFromDTO(SaleDTO dto);
    }
}