namespace Application.Models.Auth.DTO;

public class SignUpDTO
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public string? Password { get; set; } = null;
    public string? ConfirmPassword { get; set; } = null;
}