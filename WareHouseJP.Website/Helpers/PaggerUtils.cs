using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WareHouseJP.Website.Helpers
{
    public class PaggerUtils
    {
        public static bool CheckHashMore(int rowCounts, int PageSize, int PageNo)
        {
            bool result = false;
            int totalPage = (int)Math.Ceiling(1.0 * rowCounts / PageSize);
            if (totalPage > PageNo) { result = true; }
            return result;
        }
        public class Pager<T>
        {
            public static IPagedList<T> CreatePagging(IQueryable<T> entity, int? page,int pageSize = 0)
            {
                // return a 404 if user browses to before the first page
                if (page.HasValue && page < Constant.PageFirst)
                    return null;

                // retrieve list from database/whereverand
                var listUnpaged = entity;

                // page the list
                if (pageSize == 0)
                {
                    pageSize = Constant.PageSize;
                }
                var listPaged = listUnpaged.ToPagedList(page ?? Constant.PageFirst, pageSize);

                // return a 404 if user browses to pages beyond last page. special case first page if no items exist
                if (listPaged.PageNumber != Constant.PageFirst && page.HasValue && page > listPaged.PageCount)
                    return null;

                return listPaged;
            }
            public static IPagedList<int> CreatePagging(int entity, int? page, int pageSize = 0)
            {
                // return a 404 if user browses to before the first page
                if (page.HasValue && page < Constant.PageFirst)
                    return null;

                // retrieve list from database/whereverand
               

                // page the list
                if (pageSize == 0)
                {
                    pageSize = Constant.PageSize;
                }
                List<int> intList = new List<int>();
                for (int i = 0; i < entity; i++)
                {
                    intList.Add(i + 1);
                }
                var listUnpaged = intList.AsQueryable();
                var listPaged = listUnpaged.ToPagedList(page ?? Constant.PageFirst, pageSize);

                // return a 404 if user browses to pages beyond last page. special case first page if no items exist
                if (listPaged.PageNumber != Constant.PageFirst && page.HasValue && page > listPaged.PageCount)
                    return null;

                return listPaged;
            }
        }

        public class PagerApi<T>
        {
            public static IPagedList<T> CreatePagging(IQueryable<T> entity, int? page, int PageSize)
            {
                // return a 404 if user browses to before the first page
                if (page.HasValue && page < Constant.PageFirst)
                    return null;

                // retrieve list from database/whereverand
                var listUnpaged = entity;

                // page the list
                int pageSize = PageSize;
                var listPaged = listUnpaged.ToPagedList(page ?? Constant.PageFirst, pageSize);

                // return a 404 if user browses to pages beyond last page. special case first page if no items exist
                if (listPaged.PageNumber != Constant.PageFirst && page.HasValue && page > listPaged.PageCount)
                    return null;

                return listPaged;
            }

        }
    }
}