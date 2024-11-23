using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using scheduler.Business;
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

        public UserController(IUserBusiness business)
        {
            _business = business;
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
        public async Task<IActionResult> Create([FromBody] UserCreateDTO userCreateDTO)
        {
            try
            {
                var user = new User
                {
                    FederalId = userCreateDTO.FederalId,
                    Name = userCreateDTO.Name,
                    Email = userCreateDTO.Email
                };
                
                var createdUser = await _business.CreateAsync(user);

                var response = new UserDTO
                {
                    Id = createdUser.Id,
                    Guid = createdUser.Guid,
                    FederalId = createdUser.FederalId,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    CreatedDate = createdUser.CreatedDate,
                    UpdatedDate = createdUser.UpdatedDate,
                    DeletedDate = createdUser.DeletedDate
                };

                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            try
            {
                user.Id = id;
                await _business.UpdateAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
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
