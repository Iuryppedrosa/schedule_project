using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using scheduler.Business.Interfaces;
using scheduler.Models.DTOs;
using scheduler.Models.Entities;

namespace scheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _business;
        private readonly ILogger<UserController> _logger; 

        public UserController(IUserBusiness business, ILogger<UserController> logger)
        {
            _business = business;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _business.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _business.GetByIdAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreateDTO)
        {
            _logger.LogInformation("Create user request received for email: {Email}", userCreateDTO.Email);

            try
            {
               var response = await _business.CreateAsync(userCreateDTO);
                return CreatedAtAction(nameof(GetById), new { id = response.FederalId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user with email {Email}", userCreateDTO.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/update")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UserCreateDTO user)
        {
            try
            {
                await _business.UpdateAsync(user, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/delete")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _business.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _business.AuthenticateAsync(request.Email, request.Password);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
