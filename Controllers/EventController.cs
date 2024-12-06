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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var Event = await _business.GetByIdAsync(id);
                return Ok(Event);
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
                return CreatedAtAction(nameof(GetEventById), new { id = response.Guid }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

    }
}
