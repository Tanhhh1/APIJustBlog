using System.ComponentModel.DataAnnotations;

namespace Application.Models.Auth.DTO;

public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}