using System.ComponentModel.DataAnnotations;

namespace Application.Models.Auth.DTO;

public class SignInDTO
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";

}