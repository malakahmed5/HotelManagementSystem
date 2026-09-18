using AutoMapper;
using HMS.Core.Entiites.RoomModuleEntities;
using HMS.Shared.DTOs.RoomDTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services.AutoMapper.RoomModule
{
    public class RoomImageResolver : IValueResolver<Room, RoomDetailsDTO, List<string>>
    {
        private readonly IConfiguration _configuration;

        public RoomImageResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> Resolve(Room source, RoomDetailsDTO destination, List<string> destMember, ResolutionContext context)
        {
            var roomImagesName = source.RoomImages?.Select(x => x.ImageUrl).ToList();
            if (roomImagesName == null || roomImagesName.Count == 0) return [];

            var roomImagesUrl = new List<string>();
            foreach (var imageName in roomImagesName)
            {
                if (string.IsNullOrEmpty(imageName)) continue;

                if (imageName.StartsWith("http" , StringComparison.OrdinalIgnoreCase) || imageName.StartsWith("https" , StringComparison.OrdinalIgnoreCase))
                    roomImagesUrl.Add(imageName);

                else
                    roomImagesUrl.Add($"{_configuration["URLS:BaseUrl"]}/images/rooms/{imageName}");
            }

            return roomImagesUrl;
        }
    }
}
