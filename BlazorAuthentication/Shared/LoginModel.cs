using System.ComponentModel.DataAnnotations;

namespace BlazorAuthentication.Shared
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }
    }
}
