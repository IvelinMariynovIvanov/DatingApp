
namespace DatingApp.API.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class UserForRegisterDto
    {
        [Required]
        public string Username {get;set;}

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage ="You must enter a password greater than 4")]
        public string Password {get;set;}
    }
}