﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KoiManagement.Common
{
    public class BaseFilter
    {
        private int _skip = 0;

        public BaseFilter()
        {
            IsNeedPaging = true;
            ItemsPerPage = 4;
            CurrentPage = 1;
        }

        public int Skip
        {
            get
            {
                if (_skip == 0)
                    _skip = (CurrentPage - 1) * ItemsPerPage;
                return _skip;
            }
            set { _skip = value; }
        }

        public int Limit
        {
            get { return ItemsPerPage; }
            set { ItemsPerPage = value; }
        }

        public bool IsNeedPaging { get; set; }

        public long TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int ItemsPerPage { get; set; }

        public long TotalPagesCount
        {
            get
            {
                return TotalCount/ItemsPerPage +
                       ((TotalCount%ItemsPerPage > 0) ? 1 : 0);
            }
            set { throw new NotImplementedException(); }
        }
    }
}