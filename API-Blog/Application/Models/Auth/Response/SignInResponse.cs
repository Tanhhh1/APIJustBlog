namespace Application.Models.Auth.Response;

public class SignInResponse
{
    public bool TwoFactorRequired { get; set; } = false;
    public string? Message { get; set; }
}