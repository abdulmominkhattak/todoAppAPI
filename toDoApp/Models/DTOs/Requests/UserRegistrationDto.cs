using System.ComponentModel.DataAnnotations;

namespace toDoApp.Models.DTOs.Requests
{
    public class UserRegistrationDto
    {
        [Required]
        public string  username { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
