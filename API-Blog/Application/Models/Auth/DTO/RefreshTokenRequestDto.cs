using System.ComponentModel.DataAnnotations;

namespace Application.Models.Auth.DTO;

public class RefreshTokenRequestDto
{
    [Required] 
    public string AccessToken { get; set; } = null!;
    [Required] 
    public string RefreshToken { get; set; } = null!;
}