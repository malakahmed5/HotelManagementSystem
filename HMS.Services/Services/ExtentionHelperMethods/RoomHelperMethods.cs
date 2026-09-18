using HMS.Core.Entiites.RoomModuleEntities;
using HMS.Core.Entiites.RoomModuleEntities.Enums;
using HMS.Shared.QueryParameters;
using HMS.Shared.QueryParameters.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.Shared.SharedEnums;

namespace HMS.Services.Services.ExtentionHelperMethods
{
    public static class RoomHelperMethods
    {
        public static IQueryable<Room> ApplyFiltration(this IQueryable<Room> query, RoomQueryParams queryParams)
        {
            if (queryParams.RoomType.HasValue)
            {
                var selectedType = queryParams.RoomType.Value switch
                {
                    Shared.SharedEnums.RoomType.Single => Core.Entiites.RoomModuleEntities.Enums.RoomType.Single,
                    Shared.SharedEnums.RoomType.Double => Core.Entiites.RoomModuleEntities.Enums.RoomType.Double,
                    Shared.SharedEnums.RoomType.Triple => Core.Entiites.RoomModuleEntities.Enums.RoomType.Triple,
                    Shared.SharedEnums.RoomType.Suite => Core.Entiites.RoomModuleEntities.Enums.RoomType.Suite,
                    _ => default
                };

                query = query.Where(x => x.RoomType == selectedType);
            }

            if (queryParams.RoomStatus.HasValue)
            {
                var selectedStatus = queryParams.RoomStatus.Value switch
                {
                    Shared.SharedEnums.RoomStatus.Available => Core.Entiites.RoomModuleEntities.Enums.RoomStatus.Available,
                    Shared.SharedEnums.RoomStatus.Reserved => Core.Entiites.RoomModuleEntities.Enums.RoomStatus.Reserved,
                    Shared.SharedEnums.RoomStatus.Maintenance => Core.Entiites.RoomModuleEntities.Enums.RoomStatus.Maintenance,
                    Shared.SharedEnums.RoomStatus.NonExist => Core.Entiites.RoomModuleEntities.Enums.RoomStatus.NonExist,
                    _ => default
                };

                query = query.Where(x => x.RoomStatus == selectedStatus);
            }

            return query;
        }

        public static IQueryable<Room> ApplySearching(this IQueryable<Room> query, RoomQueryParams queryParams)
        {
            if (string.IsNullOrWhiteSpace(queryParams.Search))
                return query;

            var search = queryParams.Search.Trim().ToLower();

            bool isPriceValid = decimal.TryParse(search, out var parsedPrice);
            bool isRoomTypeValid = Enum.TryParse<Core.Entiites.RoomModuleEntities.Enums.RoomType>(search, true, out var parsedRoomType);
            bool isRoomStatusValid = Enum.TryParse<Core.Entiites.RoomModuleEntities.Enums.RoomStatus>(search, true, out var parsedStatus);

            query = query.Where(x =>
                x.Description.ToLower().Contains(search) ||
                x.Amenities.ToLower().Contains(search) ||
                (isPriceValid && x.PricePerNight <= parsedPrice) ||
                (isRoomTypeValid && x.RoomType == parsedRoomType) ||
                (isRoomStatusValid && x.RoomStatus == parsedStatus)
            );

            return query;
        }

        public static IQueryable<Room> ApplySorting(this IQueryable<Room> query, RoomQueryParams queryParams)
        {
            if (queryParams.SortingOptions.HasValue)
            {
                query = queryParams.SortingOptions switch
                {
                    SortingOptions.priceAsc => query.OrderBy(x => x.PricePerNight),
                    SortingOptions.priceDesc => query.OrderByDescending(x => x.PricePerNight),
                    _ => query.OrderBy(x => x.Id)
                };
            }
            return query;
        }



    }
}
