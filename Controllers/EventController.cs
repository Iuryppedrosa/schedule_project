using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scheduler.Business.Interfaces;
using scheduler.Models.DTOs;

namespace scheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventBusiness _business;

        public EventController(IEventBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllEvent()
        {
            try
            {
                var events = await _business.GetAllAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetAllActiveEvents()
        {
            try
            {
                var events = await _business.GetActiveEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{guid}")]
        [Authorize]
        public async Task<IActionResult> GetEventByGuid(Guid guid)
        {
            try
            {
                var @event = await _business.GetByGuidAsync(guid);
                return Ok(@event);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] EventDTO eventDTO)
        {
            try
            {
                var response = await _business.CreateAsync(eventDTO);
                return CreatedAtAction(nameof(GetEventByGuid), new { guid = response.Guid }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateEvent(Guid guid, [FromBody] EventDTO eventDTO)
        {
            try
            {
                var response = await _business.UpdateAsync(guid, eventDTO);
                return CreatedAtAction(nameof(GetEventByGuid), new { guid = response.Guid }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("delete/{guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(Guid guid)
        {
            try
            {
                await _business.UpdateDeleteAsync(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

    }
}
