using HMS.Shared.QueryParameters.Enums;
using HMS.Shared.SharedEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Shared.QueryParameters
{
    public class RoomQueryParams
    {
        //FilterBy:
        public RoomType? RoomType { get; set; }
        public RoomStatus? RoomStatus { get; set; }

        //search:
        public string? Search {  get; set; }

        //sorting:
        public SortingOptions? SortingOptions{ get; set; }

        //Pagination:
        private const int defaultPageSize = 5;
        private const int maxPageSize = 150;

        private int pageSize = defaultPageSize;
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                int target = value <= 0 ? defaultPageSize : value;
                pageSize = target > maxPageSize ? maxPageSize : target;
            }
        }

        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value < 1 ? 1 : value;
            }
        }
    }
}
