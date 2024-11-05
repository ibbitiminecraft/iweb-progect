using Microsoft.AspNetCore.Mvc;
using Revent.Common.CommonDtos;
using Revent.Common.CommonModels;
using Revent.Common.Constants;
using Revent.Common.Extensions;
using Revent.EFCore.DataModel.Models;
using Revent.Services.IServices;

namespace Revent.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        ILogger<EventController> _logger;
        IUserService _userService;
        ILookupService _lookupService;
        IEventService _eventService;
        public EventController(ILogger<EventController> logger,IUserService userService, ILookupService lookupService, IEventService eventService)
        {
            _logger = logger;
            _userService = userService;
            _lookupService = lookupService;
            _eventService = eventService;
        }

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent(EventCreateDto eventCreateDto)
        {
            try
            {
                _logger.LogInformation("Starting creating an event");
                if(!ModelState.IsValid)
                {
                    return BadRequest(new ApiError { Message = ModelState.GetValidationErrorsAsCsv(), StatusCode = Constants.BAD_REQUEST_STATUS_CODE});
                }

                // Validating Organizer
                var organizaer = await _userService.FindUserById(eventCreateDto.OrganizerId);
                if (organizaer == null) return BadRequest(new ApiError { Message = "Organizer with given Id not found in database", StatusCode = Constants.BAD_REQUEST_STATUS_CODE });

                //Validating EventType
                var eventTypeLkp = await _lookupService.GetLookupById(eventCreateDto.EventTypeId);
                if (eventTypeLkp == null) return BadRequest(new ApiError { Message = "Event Type not found in database", StatusCode = Constants.BAD_REQUEST_STATUS_CODE });

                eventCreateDto.Organizer = organizaer;
                eventCreateDto.EventType = eventTypeLkp;

                await _eventService.AddEventAsync(eventCreateDto);
                
                _logger.LogInformation("Event Has Been Created");

                return Ok(new ApiResponse<object> {Message="Event Has been Created",StatusCode = Constants.OK_STATUS_CODE });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(Constants.INTERNAL_SERVER_ERROR, "Internal Server Error" + ex.ToString());
            }
        }

        [HttpGet("GetEventById")]
        public async Task<IActionResult> CreateEvent(int eventId)
        {
            try
            {
                _logger.LogInformation("Getting an event");
               
                //Validating EventType
                var eventEntity = await _eventService.GetEventByIdAsync(eventId);
                if (eventEntity == null) return BadRequest(new ApiError { Message = "Event not found in database", StatusCode = Constants.BAD_REQUEST_STATUS_CODE });

                _logger.LogInformation("Event Has Been found");

                return Ok(new ApiResponse<Events> { Data = eventEntity, Message = "Event Has been Found", StatusCode = Constants.OK_STATUS_CODE });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(Constants.INTERNAL_SERVER_ERROR, "Internal Server Error" + ex.ToString());
            }
            
        }
    }
}