using HMS.Services.Abstraction;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    public class RoomsController : ApiBaseController
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("public")]
        public async Task<ActionResult<GenericResponse<PaginatedResult<RoomDTO>>>> GetRoomsForGuset([FromQuery] RoomQueryParams queryParams)
        {
            var result = await _roomService.GetAllRoomsForGuestAsync(queryParams);
            return HandelResponse(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDetailsDTO>> GetRoomDetails(int id)
        {
            var result = await _roomService.GetRoomDetailsAsync(id);
            return HandelResponse(result);
        }

        [Authorize(Roles ="Admin,Staff")]
        [HttpGet("admin")]
        public async Task<ActionResult<GenericResponse<PaginatedResult<RoomForAdminDTO>>>> GetRoomsForAdminOrStaff([FromQuery] RoomQueryParams queryParams)
        {
            var result = await _roomService.GetAllRoomsForAdminOrStaffAsync(queryParams);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GenericResponse<bool>>> CreateRoom([FromBody]CreateRoomDTO room)
        {
            var result = await _roomService.CreateRoomAsync(room);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> UpdateRoom([FromRoute] int id , [FromBody]UpdateRoomDTO updateRoom)
        {
            var result = await _roomService.UpdateRoomAsync(id ,updateRoom);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoom([FromRoute] int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{Id}/images")]
        public async Task<ActionResult<GenericResponse<bool>>> UploadRoomImages([FromRoute]int id , [FromForm] List<IFormFile> files)
        {
            var result = await _roomService.UploadRoomImgaesAsync( id, files);
            return HandelResponse(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/images/{imageId}")]
        public async Task<ActionResult<GenericResponse<bool>>> DeleteRoomImage(int id , int imageId)
        {
            var result = await _roomService.DeleteRoomImageAsync(imageId,id);
            return HandelResponse(result);
        }
    }
}
