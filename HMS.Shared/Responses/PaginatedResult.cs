using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.Responses
{
    public class PaginatedResult<T>
    {
        public int Count { get; set; }
        public int PageSize {  get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; } = [];

        public PaginatedResult(int count, int pageSize, int pageIndex, int totalPages,IEnumerable<T> data)
        {
            Count = count;
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalPages = totalPages;
            Data = data;
        }
    }
}
