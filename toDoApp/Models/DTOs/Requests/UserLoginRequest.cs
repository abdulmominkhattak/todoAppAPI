using System.ComponentModel.DataAnnotations;

namespace toDoApp.Models.DTOs.Requests
{
    public class UserLoginRequest
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
