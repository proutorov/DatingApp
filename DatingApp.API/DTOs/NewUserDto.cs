using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class NewUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 4, ErrorMessage = "You must specify password of lenght between 4 and 8 characters")]
        public string Password { get; set; }
    }
}