using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entiites.RoomModuleEntities;
using HMS.Core.Entiites.RoomModuleEntities.Enums;
using HMS.Services.Abstraction;
using HMS.Services.Services.ExtentionHelperMethods;
using HMS.Shared.DTOs.RoomDTOs;
using HMS.Shared.QueryParameters;
using HMS.Shared.Responses;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HMS.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IAttachmentService _attachmentService;

        public RoomService(IUnitOfWork unitOfWork , IMapper mapper , ILogger<RoomService> logger,
            IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachmentService = attachmentService;
        }

        public async Task<GenericResponse<PaginatedResult<RoomDTO>>> GetAllRoomsForGuestAsync(RoomQueryParams queryParams)
        {
            var genericResponse = new GenericResponse<PaginatedResult<RoomDTO>>();

            var roomRepo = _unitOfWork.GetRepository<int, Room>();

            var rooms = await roomRepo.GetAllAsync(q =>
            q.Where(r => r.RoomStatus == RoomStatus.Available || r.RoomStatus == RoomStatus.Reserved)
            .ApplyFiltration(queryParams)
            .ApplySearching(queryParams)
            .ApplySorting(queryParams)
            .ApplyPagination(queryParams.PageSize, queryParams.PageIndex));

            if (rooms is null || !rooms.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Rooms.NotFound";
                return genericResponse;
            }
            
            var mappedRooms = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

            int countRooms = await roomRepo.CountAsync(q => q.Where(r => r.RoomStatus == RoomStatus.Available || r.RoomStatus == RoomStatus.Reserved)
            .ApplyFiltration(queryParams).ApplySearching(queryParams));
            int totalPages = (int)Math.Ceiling((double)countRooms / queryParams.PageSize);
            var paginatedResult = new PaginatedResult<RoomDTO>(countRooms, mappedRooms.Count(), queryParams.PageIndex, totalPages, mappedRooms);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Room Retrieved Successfully";
            genericResponse.Data = paginatedResult;

            return genericResponse;

        }

        public async Task<GenericResponse<RoomDetailsDTO>> GetRoomDetailsAsync(int id)
        {
            //if room exist or not && status = (ava || reserved)
            var genericResponse = new GenericResponse<RoomDetailsDTO>();

            var roomRepo = _unitOfWork.GetRepository<int, Room>();
            var room = await roomRepo.GetByIdAsync(id , q =>
                q.Where(r => r.RoomStatus == RoomStatus.Available || r.RoomStatus == RoomStatus.Reserved),
                includeExpressions: r => r.RoomImages);
            
            if(room is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = $"Room With This Id = {id}, Is Not Found Or Under Maintenance";
                return genericResponse;
            }

            var roomMapped = _mapper.Map<RoomDetailsDTO>(room);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = $"Room Retrived Successfully";
            genericResponse.Data = roomMapped;

            return genericResponse;
        }

        public async Task<GenericResponse<PaginatedResult<RoomForAdminDTO>>> GetAllRoomsForAdminOrStaffAsync(RoomQueryParams queryParams)
        {
            var genericResponse = new GenericResponse<PaginatedResult<RoomForAdminDTO>>();
            var roomRepo = _unitOfWork.GetRepository<int, Room>();
            var rooms = await roomRepo.GetAllAsync(q =>
            q.ApplyFiltration(queryParams).ApplySearching(queryParams)
            .ApplySorting(queryParams).ApplyPagination(queryParams.PageSize, queryParams.PageIndex));

            if(rooms is null || !rooms.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Rooms.NotFound";
                return genericResponse;
            }

            var roomsMapped = _mapper.Map<IEnumerable<RoomForAdminDTO>>(rooms);
            
            int totalCountOfRooms = await roomRepo.CountAsync(q =>q.ApplyFiltration(queryParams)
            .ApplySearching(queryParams).ApplySorting(queryParams));
            var totalPages = (int)Math.Ceiling((double) totalCountOfRooms / queryParams.PageSize);
            var paginatedResult = new PaginatedResult<RoomForAdminDTO>(totalCountOfRooms, roomsMapped.Count(), queryParams.PageIndex, totalPages, roomsMapped);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Rooms Retrieved Successfully";
            genericResponse.Data = paginatedResult;

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> CreateRoomAsync(CreateRoomDTO createRoomDto)
        {
            var genericResponse = new GenericResponse<bool>();
            try
            {
                if (createRoomDto is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Room Data";
                    genericResponse.Data = false;
                    return genericResponse;
                }

                var roomToBeCreated = _mapper.Map<Room>(createRoomDto);
                await _unitOfWork.GetRepository<int, Room>().AddAsync(roomToBeCreated);
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Room Created Successfully";
                    genericResponse.Data = true;
                }

                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = "Room Failed To Create";
                    genericResponse.Data = false;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"An Excepected Error Occurred When Create Room, {ex.Message}");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An Excepected Error Occurred When Create Room";
                genericResponse.Data = false;

                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDTO updateRoom)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var roomRepo = _unitOfWork.GetRepository<int, Room>();
                var room = await roomRepo.GetByIdAsync(id);

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Room With This Id = {id} NotFount To Update";
                    genericResponse.Data = false;
                    return genericResponse;
                }

                var roomAfterUpdated = _mapper.Map(updateRoom, room);
                roomAfterUpdated.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetRepository<int, Room>().Update(roomAfterUpdated);
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room Updated Successfully";
                    genericResponse.Data = result;
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"Room Failed To Update";
                    genericResponse.Data = result;
                }

                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An Excepected Error Occurred When Update The Room, {ex.Message}");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An Excepected Error Occurred When Update Room";
                genericResponse.Data = false;

                return genericResponse;
            }
        }

        public async Task<GenericResponse<bool>> DeleteRoomAsync(int id)
        {
            //Can't Delete Room: [Already nonExist + Have Future Booking]
            var genericResponse = new GenericResponse<bool>();
            try
            {
                var roomRepo = _unitOfWork.GetRepository<int, Room>();

                //Not Completed Logic [Rooms Without Future Booking]
                var room = await roomRepo.GetByIdAsync(id,x => x.Where(r => r.RoomStatus != RoomStatus.NonExist));

                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Room With This Id = {id} Not Found To Delete";
                    genericResponse.Data = false;
                    return genericResponse;
                }

                room.RoomStatus = RoomStatus.NonExist;
                room.UpdatedAt = DateTime.Now;
                roomRepo.Update(room);
                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room Deleted Successfully";
                }
                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"Room Failed To Delete";
                }
                genericResponse.Data = result;
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Un Excepected Error Happened When Try To Delete Room, {ex.Message}");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An Excepected Error Occurred When Delete Room";
                genericResponse.Data = false;

                return genericResponse;
            }
                
        }

        public async Task<GenericResponse<bool>> UploadRoomImgaesAsync(int roomId, List<IFormFile> files)
        {
            var genericResponse = new GenericResponse<bool>();

            try
            {
                var room = await _unitOfWork.GetRepository<int, Room>().GetByIdAsync(roomId);
                if (room is null || roomId <= 0 || files.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"Room With Id = {roomId} Is Not Found To Images For It.";
                    genericResponse.Data = false;
                }

                foreach (var file in files)
                {
                    var fileName = await _attachmentService.UploadFile(file, "rooms");
                    if (fileName is null)
                        continue;
                    await _unitOfWork.GetRepository<int, RoomImage>().AddAsync(new RoomImage() { ImageUrl = fileName!, RoomId = roomId });
                }

                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room Images Uploaded Successfully";
                }

                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"Room Images Failed To Upload";
                }

                genericResponse.Data = result;
                return genericResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Un Excepected Error Happened When Try To Upload Room Images, {ex.Message}");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An Excepected Error Occurred When Upload Room Images";
                genericResponse.Data = false;

                return genericResponse;
            }

        }

        public async Task<GenericResponse<bool>> DeleteRoomImageAsync(int ImageId, int roomId)
        {
            var genericResponse = new GenericResponse<bool>();
            try
            {
                var room = await _unitOfWork.GetRepository<int, Room>()
                    .GetByIdAsync(roomId , null , r => r.RoomImages);
                if (room is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"No Image With Id = {ImageId} Found For Room With Id = {roomId}";
                    genericResponse.Data = false;
                    return genericResponse;
                }

                if(room.RoomImages is null || room.RoomImages.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "No Images Found To This Room";
                    return genericResponse;
                }

                var roomImage = room.RoomImages.Where(x => x.Id == ImageId).FirstOrDefault();

                if (roomImage is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = $"No Images For This Room With id = {roomId}";
                    return genericResponse;
                }

                _unitOfWork.GetRepository<int, RoomImage>().Delete(roomImage);

                var isDeleted = _attachmentService.DeleteFile("rooms", roomImage.ImageUrl);
                if (!isDeleted)
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"An UnExpected Error Happened With Try Delete Image With Id = {ImageId} For Room With Id {roomId}";
                    return genericResponse;
                }

                var result = await _unitOfWork.SaveChangesAsync() > 0;

                if (result)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"Room Image Deleted Successfully";
                }

                else
                {
                    genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                    genericResponse.Message = $"Room Image Failed To Delete";
                }

                genericResponse.Data = result;
                return genericResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Un Excepected Error Happened When Try To Delete Room Image, {ex.Message}");
                genericResponse.StatusCode = StatusCodes.Status500InternalServerError;
                genericResponse.Message = "An Excepected Error Occurred When Delete Room Image";
                genericResponse.Data = false;

                return genericResponse;
            }


        }
    }

}
