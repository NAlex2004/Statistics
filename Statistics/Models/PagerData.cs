using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Statistics.Models
{
    public class PagerData
    {
        int _totalPages = int.MaxValue;
        int _currentPage = 1;
        int _itemsPerPage = int.MaxValue;

        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = Math.Max(1, value);
                _currentPage = Math.Max(1, Math.Min(value, _totalPages));
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = Math.Max(1, Math.Min(value, _totalPages));
            }
        }

        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set
            {
                _itemsPerPage = Math.Max(1, value);
            }
        }
    }
}