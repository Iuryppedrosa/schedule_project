using System.ComponentModel.DataAnnotations;
using scheduler.Models.Entities;

namespace scheduler.Models.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid Guid { get; set; }

        public LoginResponse(string token, User user)
        {
            Token = token;
            Email = user.Email;
            Name = user.Name;
            Guid = user.Guid;
        }
    }


}
