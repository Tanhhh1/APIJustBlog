using System.ComponentModel.DataAnnotations;

namespace Application.Models.Auth.DTO;

public class SignInDTO
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

}