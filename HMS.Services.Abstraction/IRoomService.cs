using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Abstraction
{
    public interface IRoomService
    {
        Task<GenericResponse<PaginatedResult<RoomDTO>>> GetAllRoomsForGuestAsync(RoomQueryParams queryParams);
        Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsForGuestAsync(int id);

        Task<GenericResponse<PaginatedResult<RoomForAdminDTO>>> GetAllRoomsForAdminOrStaffAsync(RoomQueryParams queryParams);
        Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDto);
        Task<GenericResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDTO updateRoom);
        Task<GenericResponse<bool>> DeleteRoomAsync(int id);

        Task<GenericResponse<bool>> UploadRoomImgaesAsync(int roomId, List<IFormFile> files);
        Task<GenericResponse<bool>> DeleteRoomImageAsync(int ImageId, int roomId);

    }
}
