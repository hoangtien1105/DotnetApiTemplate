﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Commons
{
    public class PaginationParameter
    {
        private const int maxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 5; // DEPENDENCE ON PROJECT
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}