using AutoMapper;
using HMS.Core.Entiites.RoomModuleEntities;
using HMS.Shared.DTOs.RoomDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services.AutoMapper.RoomModule
{
    internal class RoomMappingProfile:Profile
    {
        public RoomMappingProfile()
        {
            CreateMap<Room, RoomDTO>();

            CreateMap<Room, RoomDetailsDTO>()
                .ForMember(dest => dest.RoomImagesUrls, opt => opt.MapFrom<RoomImageResolver>());

            CreateMap<Room, RoomForAdminDTO>();

            CreateMap<CreateRoomDTO, Room>();

            CreateMap<UpdateRoomDTO, Room>();
        }
    }
}
