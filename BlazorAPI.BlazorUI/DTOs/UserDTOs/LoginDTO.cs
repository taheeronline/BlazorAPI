using System.ComponentModel.DataAnnotations;

namespace BlazorAPI.BlazorUI.DTOs.UserDTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
